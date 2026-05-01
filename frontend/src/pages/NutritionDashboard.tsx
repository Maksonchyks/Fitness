import { useState, useEffect } from 'react';
import { useNavigate } from 'react-router-dom';
import './NutritionDashboard.css';
import * as api from '../services/nutritionApi';
import type { DailyProgress, MealPlanTemplate, WeightLogEntry } from '../services/nutritionApi';

const MEAL_TYPES: Record<string, { label: string; emoji: string; color: string }> = {
  Breakfast: { label: 'Сніданок', emoji: '🌅', color: '#f59e0b' },
  Lunch: { label: 'Обід', emoji: '☀️', color: '#10b981' },
  Dinner: { label: 'Вечеря', emoji: '🌙', color: '#8b5cf6' },
  Snack: { label: 'Перекус', emoji: '🍎', color: '#06b6d4' },
};

const GOAL_OPTIONS = [
  { value: 0, label: 'Схуднення', emoji: '🔥' },
  { value: 1, label: 'Набір маси', emoji: '💪' },
  { value: 2, label: 'Підтримка', emoji: '⚖️' },
];

const NutritionDashboard = () => {
  const navigate = useNavigate();
  const [tab, setTab] = useState<'diary' | 'templates' | 'weight' | 'target'>('diary');
  const [progress, setProgress] = useState<DailyProgress | null>(null);
  const [template, setTemplate] = useState<MealPlanTemplate | null>(null);
  const [weightHistory, setWeightHistory] = useState<WeightLogEntry[]>([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState('');

  // Log meal form
  const [mealName, setMealName] = useState('');
  const [mealType, setMealType] = useState(0);
  const [cal, setCal] = useState('');
  const [prot, setProt] = useState('');
  const [fat, setFat] = useState('');
  const [carb, setCarb] = useState('');
  const [submitting, setSubmitting] = useState(false);
  const [showMealForm, setShowMealForm] = useState(false);

  // Weight form
  const [weightVal, setWeightVal] = useState('');

  // Target form
  const [tGoal, setTGoal] = useState(0);
  const [tCal, setTCal] = useState('');
  const [tProt, setTProt] = useState('');
  const [tFat, setTFat] = useState('');
  const [tCarb, setTCarb] = useState('');

  const loadProgress = async () => {
    try {
      const data = await api.fetchDailyProgress();
      setProgress(data);
    } catch (e: any) {
      if (e.message === '401') { navigate('/login'); return; }
      setError('Не вдалося завантажити прогрес');
    }
  };

  useEffect(() => {
    setLoading(true);
    Promise.all([loadProgress(), api.getWeightHistory(30).then(setWeightHistory).catch(() => {})])
      .finally(() => setLoading(false));
  }, []);

  const handleLogMeal = async (e: React.FormEvent) => {
    e.preventDefault();
    setSubmitting(true); setError('');
    try {
      await api.logMeal({ mealName, mealType, calories: +cal, proteins: +prot, fats: +fat, carbs: +carb });
      setMealName(''); setCal(''); setProt(''); setFat(''); setCarb(''); setShowMealForm(false);
      await loadProgress();
    } catch { setError('Помилка при додаванні запису'); }
    finally { setSubmitting(false); }
  };

  const handleDeleteMeal = async (id: string) => {
    if (!confirm('Видалити цей запис?')) return;
    try { await api.deleteMealLog(id); await loadProgress(); } catch { setError('Помилка видалення'); }
  };

  const handleNextTemplate = async () => {
    setError('');
    try { const t = await api.getNextTemplate(); setTemplate(t); } catch { setError('Немає доступних шаблонів'); }
  };

  const handleResetQueue = async () => {
    try { await api.resetTemplateQueue(); setTemplate(null); } catch {}
  };

  const handleLogWeight = async (e: React.FormEvent) => {
    e.preventDefault(); setSubmitting(true);
    try {
      await api.logWeight(+weightVal);
      setWeightVal('');
      const h = await api.getWeightHistory(30);
      setWeightHistory(h);
      await loadProgress();
    } catch { setError('Помилка запису ваги'); }
    finally { setSubmitting(false); }
  };

  const handleSetTarget = async (e: React.FormEvent) => {
    e.preventDefault(); setSubmitting(true);
    try {
      await api.setDailyTarget({ goal: tGoal, calories: +tCal, proteins: +tProt, fats: +tFat, carbs: +tCarb });
      await loadProgress();
      setTab('diary');
    } catch { setError('Помилка збереження цілі'); }
    finally { setSubmitting(false); }
  };

  const handleApplyAdjustment = async () => {
    if (!progress?.adjustment) return;
    const s = progress.adjustment.suggestedTarget;
    try {
      await api.setDailyTarget({ goal: 0, calories: s.calories, proteins: s.proteins, fats: s.fats, carbs: s.carbs });
      await loadProgress();
    } catch { setError('Помилка застосування рекомендації'); }
  };

  const pctColor = (pct: number) => pct >= 100 ? '#ef4444' : pct >= 75 ? '#f59e0b' : '#10b981';
  const fmtDate = (d: string) => new Date(d).toLocaleDateString('uk-UA', { day: 'numeric', month: 'short', hour: '2-digit', minute: '2-digit' });
  const fmtDateShort = (d: string) => new Date(d).toLocaleDateString('uk-UA', { day: 'numeric', month: 'short' });

  if (loading) return (
    <div style={{ display: 'flex', justifyContent: 'center', alignItems: 'center', height: '100vh' }}>
      <div style={{ width: 48, height: 48, border: '3px solid var(--glass-border)', borderTopColor: 'var(--primary)', borderRadius: '50%', animation: 'spin 1s linear infinite' }} />
      <style>{`@keyframes spin { to { transform: rotate(360deg); } }`}</style>
    </div>
  );

  return (
    <div className="nutrition-page">
      <header className="nutrition-header">
        <h1>
          <span style={{ width: 40, height: 40, borderRadius: 12, background: 'rgba(30,41,59,0.7)', border: '1px solid var(--glass-border)', display: 'inline-flex', alignItems: 'center', justifyContent: 'center' }}>🥗</span>
          Харчування
        </h1>
        <button onClick={() => navigate('/profile')} className="btn btn-outline" style={{ padding: '0.5rem 1rem', width: 'auto' }}>
          ← Профіль
        </button>
      </header>

      <main className="nutrition-main">
        <div className="nutrition-tabs">
          {([['diary', '📊', 'Щоденник'], ['templates', '🍽️', 'Шаблони'], ['weight', '⚖️', 'Вага'], ['target', '🎯', 'Ціль КБЖВ']] as const).map(([key, icon, label]) => (
            <button key={key} className={tab === key ? 'active' : ''} onClick={() => setTab(key as any)}>
              {icon} {label}
            </button>
          ))}
        </div>

        {error && (
          <div style={{ padding: '1rem', background: 'rgba(239,68,68,0.1)', border: '1px solid rgba(239,68,68,0.3)', borderRadius: 12, color: '#fca5a5', marginBottom: '1.5rem' }}>
            {error}
          </div>
        )}

        {/* ── DIARY TAB ── */}
        {tab === 'diary' && progress && (
          <div style={{ display: 'flex', flexDirection: 'column', gap: '1.5rem' }}>
            {/* Adjustment Banner */}
            {progress.adjustment && (
              <div className={`adjustment-banner ${progress.adjustment.adjustmentPercent < 0 ? 'loss' : 'gain'}`}>
                <span style={{ fontSize: '1.5rem' }}>{progress.adjustment.adjustmentPercent < 0 ? '📉' : '📈'}</span>
                <div style={{ flex: 1 }}>
                  <p>{progress.adjustment.message}</p>
                  <p style={{ fontSize: '0.82rem', color: 'var(--text-muted)', margin: '0 0 0.75rem' }}>
                    Рекомендована зміна: <strong style={{ color: progress.adjustment.adjustmentPercent < 0 ? '#f59e0b' : '#3b82f6' }}>
                    {progress.adjustment.adjustmentPercent > 0 ? '+' : ''}{progress.adjustment.adjustmentPercent.toFixed(1)}%</strong> →{' '}
                    {progress.adjustment.suggestedTarget.calories.toFixed(0)} kcal
                  </p>
                  <button onClick={handleApplyAdjustment} className="btn btn-primary" style={{ width: 'auto', padding: '0.5rem 1.25rem', fontSize: '0.85rem' }}>
                    ✅ Застосувати
                  </button>
                </div>
              </div>
            )}

            {/* Progress Cards */}
            <div className="glass-panel" style={{ padding: '1.5rem' }}>
              <h2 style={{ fontSize: '1.2rem', fontWeight: 600, marginBottom: '1rem', display: 'flex', alignItems: 'center', gap: '0.5rem' }}>
                📊 Денний прогрес
              </h2>
              <div className="progress-grid">
                {[
                  { label: 'Калорії', val: progress.consumed.calories, tgt: progress.target?.calories, pct: progress.progress?.caloriesPercent, unit: 'kcal', color: '#f59e0b' },
                  { label: 'Білки', val: progress.consumed.proteins, tgt: progress.target?.proteins, pct: progress.progress?.proteinsPercent, unit: 'г', color: '#ef4444' },
                  { label: 'Жири', val: progress.consumed.fats, tgt: progress.target?.fats, pct: progress.progress?.fatsPercent, unit: 'г', color: '#f59e0b' },
                  { label: 'Вуглеводи', val: progress.consumed.carbs, tgt: progress.target?.carbs, pct: progress.progress?.carbsPercent, unit: 'г', color: '#3b82f6' },
                ].map(m => (
                  <div className="progress-card" key={m.label}>
                    <div className="label">{m.label}</div>
                    <div className="value" style={{ color: m.color }}>{m.val.toFixed(0)}<span style={{ fontSize: '0.7rem', color: 'var(--text-muted)' }}> {m.unit}</span></div>
                    {m.tgt != null && <div className="target">з {m.tgt.toFixed(0)} {m.unit}</div>}
                    {m.pct != null && (
                      <div className="progress-bar-bg">
                        <div className="progress-bar-fill" style={{ width: `${Math.min(m.pct, 100)}%`, background: pctColor(m.pct) }} />
                      </div>
                    )}
                    {m.pct != null && <div style={{ fontSize: '0.7rem', color: pctColor(m.pct), marginTop: 4 }}>{m.pct.toFixed(0)}%</div>}
                  </div>
                ))}
              </div>
              {!progress.target && (
                <button onClick={() => setTab('target')} className="btn btn-outline" style={{ width: 'auto', padding: '0.5rem 1rem', fontSize: '0.85rem' }}>
                  🎯 Встановити ціль КБЖВ
                </button>
              )}
            </div>

            {/* Add Meal */}
            <div className="glass-panel" style={{ padding: '1.5rem' }}>
              <div style={{ display: 'flex', justifyContent: 'space-between', alignItems: 'center', marginBottom: '1rem' }}>
                <h2 style={{ fontSize: '1.2rem', fontWeight: 600, display: 'flex', alignItems: 'center', gap: '0.5rem' }}>🍽️ Прийоми їжі</h2>
                <button onClick={() => setShowMealForm(!showMealForm)} className="btn btn-primary" style={{ width: 'auto', padding: '0.5rem 1rem', fontSize: '0.85rem' }}>
                  {showMealForm ? '✕ Закрити' : '+ Додати'}
                </button>
              </div>

              {showMealForm && (
                <form onSubmit={handleLogMeal} className="log-form" style={{ marginBottom: '1.5rem', padding: '1.25rem', background: 'rgba(59,130,246,0.05)', borderRadius: 14, border: '1px solid rgba(59,130,246,0.15)' }}>
                  <div className="full-width form-group" style={{ marginBottom: 0 }}>
                    <label className="form-label">Назва страви</label>
                    <input className="form-input" value={mealName} onChange={e => setMealName(e.target.value)} placeholder="напр. Вівсянка з бананом" required />
                  </div>
                  <div className="form-group" style={{ marginBottom: 0 }}>
                    <label className="form-label">Тип прийому</label>
                    <select className="form-input" value={mealType} onChange={e => setMealType(+e.target.value)}>
                      <option value={0}>🌅 Сніданок</option>
                      <option value={1}>☀️ Обід</option>
                      <option value={2}>🌙 Вечеря</option>
                      <option value={3}>🍎 Перекус</option>
                    </select>
                  </div>
                  <div className="form-group" style={{ marginBottom: 0 }}>
                    <label className="form-label">Калорії (kcal)</label>
                    <input type="number" className="form-input" value={cal} onChange={e => setCal(e.target.value)} placeholder="350" required min="0" />
                  </div>
                  <div className="form-group" style={{ marginBottom: 0 }}>
                    <label className="form-label">Білки (г)</label>
                    <input type="number" className="form-input" value={prot} onChange={e => setProt(e.target.value)} placeholder="20" required min="0" />
                  </div>
                  <div className="form-group" style={{ marginBottom: 0 }}>
                    <label className="form-label">Жири (г)</label>
                    <input type="number" className="form-input" value={fat} onChange={e => setFat(e.target.value)} placeholder="12" required min="0" />
                  </div>
                  <div className="form-group" style={{ marginBottom: 0 }}>
                    <label className="form-label">Вуглеводи (г)</label>
                    <input type="number" className="form-input" value={carb} onChange={e => setCarb(e.target.value)} placeholder="45" required min="0" />
                  </div>
                  <div className="full-width">
                    <button type="submit" className="btn btn-primary" disabled={submitting} style={{ marginTop: '0.5rem' }}>
                      {submitting ? '⏳ Зберігаю...' : '✅ Зберегти запис'}
                    </button>
                  </div>
                </form>
              )}

              <div className="meal-list">
                {progress.meals.length === 0 ? (
                  <p style={{ color: 'var(--text-muted)', textAlign: 'center', padding: '2rem 0' }}>Ще немає записів за сьогодні. Додайте перший прийом їжі! 🍽️</p>
                ) : progress.meals.map(meal => {
                  const mt = MEAL_TYPES[meal.mealType] || { label: meal.mealType, emoji: '🍽️', color: '#6b7280' };
                  return (
                    <div className="meal-item" key={meal.id}>
                      <div className="meal-info">
                        <h4>{mt.emoji} {meal.mealName}</h4>
                        <div className="meal-meta">
                          <span style={{ padding: '0.1rem 0.5rem', borderRadius: 999, background: `${mt.color}18`, color: mt.color, fontSize: '0.75rem', fontWeight: 600 }}>
                            {mt.label}
                          </span>
                          <span style={{ marginLeft: '0.5rem' }}>{fmtDate(meal.loggedAt)}</span>
                        </div>
                      </div>
                      <div style={{ display: 'flex', alignItems: 'center', gap: '1rem' }}>
                        <div className="meal-macros">
                          <span>🔥 <strong>{meal.nutrition.calories.toFixed(0)}</strong></span>
                          <span>Б: <strong>{meal.nutrition.proteins.toFixed(0)}</strong></span>
                          <span>Ж: <strong>{meal.nutrition.fats.toFixed(0)}</strong></span>
                          <span>В: <strong>{meal.nutrition.carbs.toFixed(0)}</strong></span>
                        </div>
                        <button className="delete-btn" onClick={() => handleDeleteMeal(meal.id)} title="Видалити">🗑️</button>
                      </div>
                    </div>
                  );
                })}
              </div>
            </div>
          </div>
        )}

        {/* ── TEMPLATES TAB ── */}
        {tab === 'templates' && (
          <div style={{ display: 'flex', flexDirection: 'column', gap: '1.5rem' }}>
            <div className="glass-panel" style={{ padding: '1.5rem' }}>
              <div style={{ display: 'flex', justifyContent: 'space-between', alignItems: 'center', marginBottom: '1.5rem', flexWrap: 'wrap', gap: '0.75rem' }}>
                <h2 style={{ fontSize: '1.2rem', fontWeight: 600, margin: 0 }}>🍽️ Шаблони харчування</h2>
                <div style={{ display: 'flex', gap: '0.5rem' }}>
                  <button onClick={handleNextTemplate} className="btn btn-primary" style={{ width: 'auto', padding: '0.5rem 1.25rem', fontSize: '0.85rem' }}>
                    🔀 Наступний шаблон
                  </button>
                  <button onClick={handleResetQueue} className="btn btn-outline" style={{ width: 'auto', padding: '0.5rem 1rem', fontSize: '0.85rem' }}>
                    🔄 Скинути
                  </button>
                </div>
              </div>

              {!template ? (
                <div style={{ textAlign: 'center', padding: '3rem 1rem' }}>
                  <div style={{ fontSize: '3rem', marginBottom: '1rem' }}>🍽️</div>
                  <h3 style={{ fontWeight: 600, marginBottom: '0.5rem' }}>Отримайте ідею для харчування</h3>
                  <p style={{ color: 'var(--text-muted)', marginBottom: '1.5rem' }}>Натисніть «Наступний шаблон» щоб побачити план на день з 30 доступних варіантів</p>
                </div>
              ) : (
                <div className="template-card" style={{ padding: 0 }}>
                  <div className="template-header">
                    <div>
                      <h3 className="template-name">{template.name}</h3>
                      <p className="template-desc">{template.description}</p>
                    </div>
                    <div style={{ textAlign: 'right' }}>
                      <div className="remaining-badge">📋 Залишилось: {template.remainingTemplates}</div>
                      <div style={{ fontSize: '0.78rem', color: 'var(--text-muted)', marginTop: '0.5rem' }}>
                        Σ {template.totalNutrition.calories.toFixed(0)} kcal · Б{template.totalNutrition.proteins.toFixed(0)} · Ж{template.totalNutrition.fats.toFixed(0)} · В{template.totalNutrition.carbs.toFixed(0)}
                      </div>
                    </div>
                  </div>

                  <div className="template-meals">
                    {template.meals.map((m, i) => {
                      const mt = MEAL_TYPES[m.mealType] || { label: m.mealType, emoji: '🍽️', color: '#6b7280' };
                      return (
                        <div className="template-meal-card" key={i}>
                          <span className="meal-type-badge" style={{ background: `${mt.color}18`, color: mt.color }}>{mt.emoji} {mt.label}</span>
                          <h4>{m.dishName}</h4>
                          <p>{m.description}</p>
                          <div className="macro-row">
                            <span>🔥 {m.nutrition.calories.toFixed(0)}</span>
                            <span>Б: {m.nutrition.proteins.toFixed(0)}г</span>
                            <span>Ж: {m.nutrition.fats.toFixed(0)}г</span>
                            <span>В: {m.nutrition.carbs.toFixed(0)}г</span>
                          </div>
                        </div>
                      );
                    })}
                  </div>
                </div>
              )}
            </div>
          </div>
        )}

        {/* ── WEIGHT TAB ── */}
        {tab === 'weight' && (
          <div style={{ display: 'grid', gridTemplateColumns: '1fr 1fr', gap: '1.5rem' }}>
            <div className="glass-panel" style={{ padding: '1.5rem' }}>
              <h2 style={{ fontSize: '1.2rem', fontWeight: 600, marginBottom: '1rem' }}>⚖️ Записати вагу</h2>
              <form onSubmit={handleLogWeight}>
                <div className="form-group">
                  <label className="form-label">Вага (кг)</label>
                  <input type="number" step="0.1" className="form-input" value={weightVal} onChange={e => setWeightVal(e.target.value)} placeholder="напр. 75.5" required min="20" max="400" />
                </div>
                <button type="submit" className="btn btn-primary" disabled={submitting}>
                  {submitting ? '⏳ Зберігаю...' : '✅ Записати'}
                </button>
              </form>
            </div>
            <div className="glass-panel" style={{ padding: '1.5rem' }}>
              <h2 style={{ fontSize: '1.2rem', fontWeight: 600, marginBottom: '1rem' }}>📈 Історія ваги (30 днів)</h2>
              {weightHistory.length === 0 ? (
                <p style={{ color: 'var(--text-muted)', textAlign: 'center', padding: '2rem 0' }}>Ще немає записів. Почніть відстежувати вагу!</p>
              ) : (
                <div className="weight-list">
                  {weightHistory.map(w => (
                    <div className="weight-item" key={w.id}>
                      <span>{fmtDateShort(w.loggedAt)}</span>
                      <span className="weight-val">{w.weight.toFixed(1)} кг</span>
                    </div>
                  ))}
                </div>
              )}
            </div>
          </div>
        )}

        {/* ── TARGET TAB ── */}
        {tab === 'target' && (() => {
          // Auto-calc: best-practice macros by goal
          const RATIOS: Record<number, { p: number; f: number; c: number; label: string; desc: string }> = {
            0: { p: 30, f: 25, c: 45, label: 'Схуднення', desc: 'Високий білок для збереження м\'язів при дефіциті' },
            1: { p: 30, f: 20, c: 50, label: 'Набір маси', desc: 'Більше вуглеводів для енергії та відновлення' },
            2: { p: 25, f: 30, c: 45, label: 'Підтримка', desc: 'Збалансоване співвідношення для здоров\'я' },
          };
          const ratio = RATIOS[tGoal] || RATIOS[2];
          const cals = parseFloat(tCal) || 0;
          const recProt = cals > 0 ? Math.round((cals * ratio.p / 100) / 4) : 0;
          const recFat = cals > 0 ? Math.round((cals * ratio.f / 100) / 9) : 0;
          const recCarb = cals > 0 ? Math.round((cals * ratio.c / 100) / 4) : 0;

          const applyRec = () => { setTProt(String(recProt)); setTFat(String(recFat)); setTCarb(String(recCarb)); };

          return (
          <div style={{ display: 'grid', gridTemplateColumns: '1.2fr 1fr', gap: '1.5rem', alignItems: 'start' }}>
            {/* Left: Form */}
            <div className="glass-panel" style={{ padding: '2rem' }}>
              <div style={{ textAlign: 'center', marginBottom: '1.5rem' }}>
                <div style={{ fontSize: '2.5rem', marginBottom: '0.5rem' }}>🎯</div>
                <h2 style={{ fontSize: '1.3rem', fontWeight: 600 }}>Встановити ціль КБЖВ</h2>
                <p style={{ color: 'var(--text-muted)', fontSize: '0.9rem' }}>Денна норма калорій та макронутрієнтів</p>
              </div>
              <form onSubmit={handleSetTarget}>
                <div className="form-group">
                  <label className="form-label">Ціль</label>
                  <select className="form-input" value={tGoal} onChange={e => setTGoal(+e.target.value)}>
                    {GOAL_OPTIONS.map(g => <option key={g.value} value={g.value}>{g.emoji} {g.label}</option>)}
                  </select>
                </div>
                <div className="form-group">
                  <label className="form-label">Калорії (kcal)</label>
                  <input type="number" className="form-input" value={tCal} onChange={e => setTCal(e.target.value)} placeholder="2000" required min="500" />
                </div>
                <div style={{ display: 'grid', gridTemplateColumns: '1fr 1fr 1fr', gap: '0.75rem' }}>
                  <div className="form-group">
                    <label className="form-label">Білки (г)</label>
                    <input type="number" className="form-input" value={tProt} onChange={e => setTProt(e.target.value)} placeholder={String(recProt || '150')} required min="0" />
                  </div>
                  <div className="form-group">
                    <label className="form-label">Жири (г)</label>
                    <input type="number" className="form-input" value={tFat} onChange={e => setTFat(e.target.value)} placeholder={String(recFat || '65')} required min="0" />
                  </div>
                  <div className="form-group">
                    <label className="form-label">Вуглеводи (г)</label>
                    <input type="number" className="form-input" value={tCarb} onChange={e => setTCarb(e.target.value)} placeholder={String(recCarb || '250')} required min="0" />
                  </div>
                </div>
                {cals > 0 && (
                  <button type="button" onClick={applyRec} className="btn btn-outline" style={{ marginBottom: '1rem', fontSize: '0.85rem', padding: '0.5rem 1rem' }}>
                    ✨ Застосувати рекомендовані БЖВ
                  </button>
                )}
                <button type="submit" className="btn btn-primary" disabled={submitting}>
                  {submitting ? '⏳ Зберігаю...' : '💾 Зберегти ціль'}
                </button>
              </form>
            </div>

            {/* Right: Recommendation panel */}
            <div className="glass-panel" style={{ padding: '1.5rem', border: '1px solid rgba(16,185,129,0.25)' }}>
              <h3 style={{ fontSize: '1.1rem', fontWeight: 600, marginBottom: '1rem', display: 'flex', alignItems: 'center', gap: '0.5rem' }}>
                🏋️ Рекомендації для спортсменів
              </h3>

              {cals > 0 ? (
                <div style={{ display: 'flex', flexDirection: 'column', gap: '1rem' }}>
                  <div style={{ padding: '1rem', background: 'rgba(16,185,129,0.06)', borderRadius: 12, border: '1px solid rgba(16,185,129,0.15)' }}>
                    <div style={{ fontSize: '0.78rem', color: '#10b981', fontWeight: 600, marginBottom: '0.5rem' }}>
                      {ratio.label} · {ratio.p}/{ratio.f}/{ratio.c}%
                    </div>
                    <p style={{ fontSize: '0.82rem', color: 'var(--text-muted)', margin: '0 0 0.75rem', lineHeight: 1.5 }}>
                      {ratio.desc}
                    </p>
                    <div style={{ display: 'flex', flexDirection: 'column', gap: '0.5rem' }}>
                      {[
                        { label: 'Білки', val: recProt, unit: 'г', pct: ratio.p, color: '#ef4444', info: `${ratio.p}% × ${cals} kcal ÷ 4` },
                        { label: 'Жири', val: recFat, unit: 'г', pct: ratio.f, color: '#f59e0b', info: `${ratio.f}% × ${cals} kcal ÷ 9` },
                        { label: 'Вуглеводи', val: recCarb, unit: 'г', pct: ratio.c, color: '#3b82f6', info: `${ratio.c}% × ${cals} kcal ÷ 4` },
                      ].map(m => (
                        <div key={m.label} style={{ display: 'flex', justifyContent: 'space-between', alignItems: 'center', padding: '0.5rem 0.75rem', background: 'rgba(255,255,255,0.02)', borderRadius: 8 }}>
                          <div>
                            <span style={{ fontWeight: 600, color: m.color }}>{m.label}</span>
                            <span style={{ fontSize: '0.7rem', color: 'var(--text-muted)', marginLeft: '0.5rem' }}>{m.info}</span>
                          </div>
                          <span style={{ fontWeight: 700, color: 'white' }}>{m.val}{m.unit}</span>
                        </div>
                      ))}
                    </div>
                  </div>

                  <div style={{ fontSize: '0.78rem', color: 'var(--text-muted)', lineHeight: 1.6 }}>
                    <strong style={{ color: 'var(--text-main)' }}>Формула:</strong><br />
                    Білки = {ratio.p}% від kcal ÷ 4 kcal/г<br />
                    Жири = {ratio.f}% від kcal ÷ 9 kcal/г<br />
                    Вуглеводи = {ratio.c}% від kcal ÷ 4 kcal/г
                  </div>
                </div>
              ) : (
                <div style={{ textAlign: 'center', padding: '2rem 0.5rem' }}>
                  <div style={{ fontSize: '2rem', marginBottom: '0.5rem' }}>📊</div>
                  <p style={{ color: 'var(--text-muted)', fontSize: '0.85rem', lineHeight: 1.6 }}>
                    Введіть кількість калорій зліва, і ми автоматично розрахуємо оптимальне співвідношення БЖВ для вашої цілі
                  </p>
                </div>
              )}

              <div style={{ marginTop: '1rem', padding: '0.75rem', background: 'rgba(59,130,246,0.06)', borderRadius: 10, border: '1px solid rgba(59,130,246,0.15)' }}>
                <div style={{ fontSize: '0.78rem', fontWeight: 600, color: '#3b82f6', marginBottom: '0.3rem' }}>💡 Відсоткові норми</div>
                <div style={{ fontSize: '0.75rem', color: 'var(--text-muted)', lineHeight: 1.6 }}>
                  <strong>Схуднення:</strong> Б30/Ж25/В45 — більше білка<br />
                  <strong>Набір маси:</strong> Б30/Ж20/В50 — більше вуглеводів<br />
                  <strong>Підтримка:</strong> Б25/Ж30/В45 — баланс
                </div>
              </div>
            </div>
          </div>
          );
        })()}
      </main>
    </div>
  );
};

export default NutritionDashboard;
