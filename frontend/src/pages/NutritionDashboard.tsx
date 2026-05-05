import { useState, useEffect } from 'react';
import { useNavigate } from 'react-router-dom';
import { 
  Activity, 
  Apple, 
  LayoutDashboard, 
  Target, 
  Scale, 
  Plus, 
  Trash2, 
  ChevronLeft,
  Flame,
  Utensils,
  History,
  TrendingUp,
  TrendingDown,
  Info,
  CheckCircle2,
  Calendar,
  X
} from 'lucide-react';
import './NutritionDashboard.css';
import * as api from '../services/nutritionApi';
import type { DailyProgress, MealPlanTemplate, WeightLogEntry } from '../services/nutritionApi';

const MEAL_TYPES: Record<string, { label: string; icon: any; color: string }> = {
  Breakfast: { label: 'Сніданок', icon: Apple, color: '#f59e0b' },
  Lunch: { label: 'Обід', icon: Utensils, color: '#10b981' },
  Dinner: { label: 'Вечеря', icon: Activity, color: '#8b5cf6' },
  Snack: { label: 'Перекус', icon: Flame, color: '#06b6d4' },
};

const GOAL_OPTIONS = [
  { value: 0, label: 'Схуднення', icon: Flame },
  { value: 1, label: 'Набір маси', icon: TrendingUp },
  { value: 2, label: 'Підтримка', icon: Scale },
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
    <div style={{ display: 'flex', justifyContent: 'center', alignItems: 'center', height: '100vh', background: 'var(--bg-darker)' }}>
      <div className="animate-spin" style={{ width: 48, height: 48, border: '3px solid var(--glass-border)', borderTopColor: 'var(--primary)', borderRadius: '50%' }} />
    </div>
  );

  return (
    <div className="nutrition-page">
      <header className="nutrition-header animate-fade-in" style={{ marginBottom: '2rem' }}>
        <div>
          <h1 className="flex items-center gap-4">
            <div style={{ 
              width: 52, 
              height: 52, 
              borderRadius: 16, 
              background: 'linear-gradient(135deg, var(--primary), #60a5fa)', 
              display: 'flex', 
              alignItems: 'center', 
              justifyContent: 'center',
              boxShadow: '0 10px 20px -5px rgba(59, 130, 246, 0.5)'
            }}>
              <Apple className="text-white" size={28} />
            </div>
            Харчування
          </h1>
          <p className="text-muted mt-2" style={{ marginLeft: 68 }}>Ваш особистий щоденник нутрієнтів</p>
        </div>
      </header>

      <main className="nutrition-main">
        <div className="nutrition-tabs animate-fade-in delay-100">
          {[
            { key: 'diary', icon: LayoutDashboard, label: 'Щоденник' },
            { key: 'templates', icon: Utensils, label: 'Шаблони' },
            { key: 'weight', icon: Scale, label: 'Вага' },
            { key: 'target', icon: Target, label: 'Ціль КБЖВ' }
          ].map(({ key, icon: Icon, label }) => (
            <button key={key} className={tab === key ? 'active' : ''} onClick={() => setTab(key as any)}>
              <Icon size={18} /> {label}
            </button>
          ))}
        </div>

        {error && (
          <div className="animate-fade-in" style={{ padding: '1rem 1.5rem', background: 'rgba(239,68,68,0.1)', border: '1px solid rgba(239,68,68,0.2)', borderRadius: 16, color: '#fca5a5', marginBottom: '2rem', display: 'flex', alignItems: 'center', gap: '0.75rem' }}>
            <X size={18} /> {error}
          </div>
        )}

        {/* ── DIARY TAB ── */}
        {tab === 'diary' && progress && (
          <div className="flex flex-col gap-8 animate-fade-in delay-200">
            {/* Adjustment Banner */}
            {progress.adjustment && (
              <div className={`adjustment-banner ${progress.adjustment.adjustmentPercent < 0 ? 'loss' : 'gain'}`}>
                <div style={{ 
                  width: 48, height: 48, borderRadius: 12, 
                  background: progress.adjustment.adjustmentPercent < 0 ? 'rgba(245,158,11,0.2)' : 'rgba(59,130,246,0.2)',
                  display: 'flex', alignItems: 'center', justifyContent: 'center'
                }}>
                  {progress.adjustment.adjustmentPercent < 0 ? <TrendingDown className="text-orange-400" /> : <TrendingUp className="text-blue-400" />}
                </div>
                <div style={{ flex: 1 }}>
                  <p>{progress.adjustment.message}</p>
                  <div className="flex items-center gap-4 mt-3">
                    <span style={{ fontSize: '0.9rem', color: 'var(--text-muted)' }}>
                      Рекомендована зміна: <strong style={{ color: progress.adjustment.adjustmentPercent < 0 ? '#f59e0b' : '#3b82f6' }}>
                      {progress.adjustment.adjustmentPercent > 0 ? '+' : ''}{progress.adjustment.adjustmentPercent.toFixed(1)}%</strong>
                    </span>
                    <button onClick={handleApplyAdjustment} className="btn btn-primary" style={{ width: 'auto', padding: '0.4rem 1rem', fontSize: '0.85rem', borderRadius: 10 }}>
                      <CheckCircle2 size={16} className="mr-2" /> Застосувати {progress.adjustment.suggestedTarget.calories.toFixed(0)} ккал
                    </button>
                  </div>
                </div>
              </div>
            )}

            {/* Progress Cards */}
            <div className="glass-panel" style={{ padding: '2rem' }}>
              <div className="flex items-center justify-between mb-8">
                <h2 className="flex items-center gap-3" style={{ fontSize: '1.4rem', fontWeight: 700 }}>
                  <Activity className="text-primary" size={24} /> Денний прогрес
                </h2>
                <div className="text-muted text-sm flex items-center gap-2">
                  <Calendar size={14} /> Сьогодні, {new Date().toLocaleDateString('uk-UA', { day: 'numeric', month: 'long' })}
                </div>
              </div>
              
              <div className="progress-grid">
                {[
                  { label: 'Калорії', val: progress.consumed.calories, tgt: progress.target?.calories, pct: progress.progress?.caloriesPercent, unit: 'ккал', color: '#f59e0b', icon: Flame },
                  { label: 'Білки', val: progress.consumed.proteins, tgt: progress.target?.proteins, pct: progress.progress?.proteinsPercent, unit: 'г', color: '#ef4444', icon: Activity },
                  { label: 'Жири', val: progress.consumed.fats, tgt: progress.target?.fats, pct: progress.progress?.fatsPercent, unit: 'г', color: '#fbbf24', icon: Apple },
                  { label: 'Вуглеводи', val: progress.consumed.carbs, tgt: progress.target?.carbs, pct: progress.progress?.carbsPercent, unit: 'г', color: '#3b82f6', icon: Utensils },
                ].map(m => (
                  <div className="progress-card" key={m.label}>
                    <div className="label flex items-center gap-2">
                      <m.icon size={14} style={{ color: m.color }} />
                      {m.label}
                    </div>
                    <div className="value">{m.val.toFixed(0)}<span style={{ fontSize: '0.9rem', color: 'var(--text-muted)', fontWeight: 400 }}> {m.unit}</span></div>
                    {m.tgt != null && <div className="target">Ціль: {m.tgt.toFixed(0)} {m.unit}</div>}
                    {m.pct != null && (
                      <div className="progress-bar-bg">
                        <div className="progress-bar-fill" style={{ width: `${Math.min(m.pct, 100)}%`, background: m.color }} />
                      </div>
                    )}
                    {m.pct != null && (
                      <div className="flex justify-between mt-2">
                        <span style={{ fontSize: '0.75rem', color: pctColor(m.pct), fontWeight: 700 }}>{m.pct.toFixed(0)}%</span>
                        <span style={{ fontSize: '0.75rem', color: 'var(--text-muted)' }}>{Math.max(0, (m.tgt || 0) - m.val).toFixed(0)} залишилось</span>
                      </div>
                    )}
                  </div>
                ))}
              </div>
              {!progress.target && (
                <div style={{ textAlign: 'center', padding: '1rem', background: 'rgba(59,130,246,0.05)', borderRadius: 16, border: '1px dashed rgba(59,130,246,0.3)' }}>
                  <p className="text-muted mb-4">Ціль КБЖВ ще не встановлена. Це допоможе нам краще відстежувати ваш прогрес.</p>
                  <button onClick={() => setTab('target')} className="btn btn-primary" style={{ width: 'auto' }}>
                    <Target size={18} className="mr-2" /> Встановити ціль
                  </button>
                </div>
              )}
            </div>

            {/* Add Meal */}
            <div className="glass-panel" style={{ padding: '2rem' }}>
              <div style={{ display: 'flex', justifyContent: 'space-between', alignItems: 'center', marginBottom: '2rem' }}>
                <h2 className="flex items-center gap-3" style={{ fontSize: '1.4rem', fontWeight: 700 }}>
                  <Utensils className="text-primary" size={24} /> Прийоми їжі
                </h2>
                <button onClick={() => setShowMealForm(!showMealForm)} className="btn btn-primary" style={{ width: 'auto', padding: '0.6rem 1.25rem', borderRadius: 14 }}>
                  {showMealForm ? <><X size={18} className="mr-2" /> Закрити</> : <><Plus size={18} className="mr-2" /> Додати страву</>}
                </button>
              </div>

              {showMealForm && (
                <form onSubmit={handleLogMeal} className="log-form animate-fade-in" style={{ marginBottom: '2rem', padding: '2rem', background: 'rgba(255,255,255,0.02)', borderRadius: 24, border: '1px solid var(--glass-border)' }}>
                  <div className="full-width form-group">
                    <label className="form-label">Назва страви</label>
                    <input className="form-input" value={mealName} onChange={e => setMealName(e.target.value)} placeholder="Введіть назву страви..." required />
                  </div>
                  <div className="form-group">
                    <label className="form-label">Тип прийому</label>
                    <select className="form-input" value={mealType} onChange={e => setMealType(+e.target.value)}>
                      <option value={0}>🌅 Сніданок</option>
                      <option value={1}>☀️ Обід</option>
                      <option value={2}>🌙 Вечеря</option>
                      <option value={3}>🍎 Перекус</option>
                    </select>
                  </div>
                  <div className="form-group">
                    <label className="form-label">Калорії (ккал)</label>
                    <input type="number" className="form-input" value={cal} onChange={e => setCal(e.target.value)} placeholder="0" required min="0" />
                  </div>
                  <div className="form-group">
                    <label className="form-label">Білки (г)</label>
                    <input type="number" className="form-input" value={prot} onChange={e => setProt(e.target.value)} placeholder="0" required min="0" />
                  </div>
                  <div className="form-group">
                    <label className="form-label">Жири (г)</label>
                    <input type="number" className="form-input" value={fat} onChange={e => setFat(e.target.value)} placeholder="0" required min="0" />
                  </div>
                  <div className="form-group">
                    <label className="form-label">Вуглеводи (г)</label>
                    <input type="number" className="form-input" value={carb} onChange={e => setCarb(e.target.value)} placeholder="0" required min="0" />
                  </div>
                  <div className="full-width mt-4">
                    <button type="submit" className="btn btn-primary" disabled={submitting}>
                      {submitting ? 'Зберігаю...' : 'Зберегти страву'}
                    </button>
                  </div>
                </form>
              )}

              <div className="meal-list">
                {progress.meals.length === 0 ? (
                  <div style={{ textAlign: 'center', padding: '4rem 0', background: 'rgba(255,255,255,0.01)', borderRadius: 24, border: '1px dashed var(--glass-border)' }}>
                    <div style={{ fontSize: '3rem', marginBottom: '1rem', opacity: 0.5 }}>🍽️</div>
                    <h3 className="text-muted">Ще немає записів за сьогодні</h3>
                    <p className="text-sm text-muted mt-2">Ваш денний раціон з\'явиться тут після додавання страв</p>
                  </div>
                ) : progress.meals.map(meal => {
                  const mt = MEAL_TYPES[meal.mealType] || { label: meal.mealType, icon: Utensils, color: '#6b7280' };
                  return (
                    <div className="meal-item" key={meal.id}>
                      <div className="flex items-center gap-4">
                        <div style={{ 
                          width: 44, height: 44, borderRadius: 12, 
                          background: `${mt.color}15`, color: mt.color,
                          display: 'flex', alignItems: 'center', justifyContent: 'center'
                        }}>
                          <mt.icon size={20} />
                        </div>
                        <div className="meal-info">
                          <h4>{meal.mealName}</h4>
                          <div className="meal-meta flex items-center gap-3">
                            <span style={{ fontWeight: 600, color: mt.color }}>{mt.label}</span>
                            <span className="flex items-center gap-1"><History size={12} /> {fmtDate(meal.loggedAt)}</span>
                          </div>
                        </div>
                      </div>
                      <div className="flex items-center gap-6">
                        <div className="meal-macros">
                          <div className="flex flex-col items-center">
                            <span className="text-xs text-muted">ккал</span>
                            <strong>{meal.nutrition.calories.toFixed(0)}</strong>
                          </div>
                          <div className="flex flex-col items-center">
                            <span className="text-xs text-muted">Б</span>
                            <strong>{meal.nutrition.proteins.toFixed(0)}</strong>
                          </div>
                          <div className="flex flex-col items-center">
                            <span className="text-xs text-muted">Ж</span>
                            <strong>{meal.nutrition.fats.toFixed(0)}</strong>
                          </div>
                          <div className="flex flex-col items-center">
                            <span className="text-xs text-muted">В</span>
                            <strong>{meal.nutrition.carbs.toFixed(0)}</strong>
                          </div>
                        </div>
                        <button className="delete-btn" onClick={() => handleDeleteMeal(meal.id)} style={{ padding: '0.6rem', background: 'rgba(239,68,68,0.05)', border: '1px solid rgba(239,68,68,0.1)', borderRadius: 12, color: '#ef4444' }}>
                          <Trash2 size={16} />
                        </button>
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
          <div className="flex flex-col gap-6 animate-fade-in delay-200">
            <div className="glass-panel" style={{ padding: '2rem' }}>
              <div style={{ display: 'flex', justifyContent: 'space-between', alignItems: 'center', marginBottom: '2.5rem', flexWrap: 'wrap', gap: '1rem' }}>
                <h2 className="flex items-center gap-3" style={{ fontSize: '1.4rem', fontWeight: 700 }}>
                  <Utensils className="text-primary" size={24} /> Шаблони харчування
                </h2>
                <div style={{ display: 'flex', gap: '0.75rem' }}>
                  <button onClick={handleNextTemplate} className="btn btn-primary" style={{ width: 'auto', padding: '0.6rem 1.25rem' }}>
                    <Plus size={18} className="mr-2" /> Наступний шаблон
                  </button>
                  <button onClick={handleResetQueue} className="btn btn-outline" style={{ width: 'auto', padding: '0.6rem 1rem' }}>
                    <History size={18} className="mr-2" /> Скинути чергу
                  </button>
                </div>
              </div>

              {!template ? (
                <div style={{ textAlign: 'center', padding: '5rem 1rem' }}>
                  <div style={{ 
                    width: 80, height: 80, borderRadius: 24, 
                    background: 'rgba(59,130,246,0.05)', 
                    display: 'flex', alignItems: 'center', justifyContent: 'center',
                    margin: '0 auto 2rem'
                  }}>
                    <Utensils className="text-primary" size={40} />
                  </div>
                  <h3 style={{ fontSize: '1.5rem', fontWeight: 700, marginBottom: '1rem' }}>Знайдіть ідеальну дієту</h3>
                  <p className="text-muted" style={{ maxWidth: 400, margin: '0 auto 2rem' }}>Ми підготували понад 30 збалансованих шаблонів для різних цілей та смаків.</p>
                  <button onClick={handleNextTemplate} className="btn btn-primary" style={{ width: 'auto' }}>Почати огляд</button>
                </div>
              ) : (
                <div className="template-card animate-fade-in">
                  <div className="template-header">
                    <div>
                      <h3 className="template-name">{template.name}</h3>
                      <p className="text-muted">{template.description}</p>
                    </div>
                    <div style={{ textAlign: 'right' }}>
                      <div className="remaining-badge" style={{ background: 'rgba(59,130,246,0.1)', color: 'var(--primary)', border: '1px solid rgba(59,130,246,0.2)' }}>
                        <LayoutDashboard size={14} className="mr-1" /> Залишилось: {template.remainingTemplates}
                      </div>
                      <div style={{ fontSize: '0.9rem', fontWeight: 700, marginTop: '1rem', color: 'white' }}>
                        Загалом: {template.totalNutrition.calories.toFixed(0)} ккал
                      </div>
                      <div style={{ fontSize: '0.8rem', color: 'var(--text-muted)', marginTop: '0.25rem' }}>
                        Б{template.totalNutrition.proteins.toFixed(0)} · Ж{template.totalNutrition.fats.toFixed(0)} · В{template.totalNutrition.carbs.toFixed(0)}
                      </div>
                    </div>
                  </div>

                  <div className="template-meals">
                    {template.meals.map((m, i) => {
                      const mt = MEAL_TYPES[m.mealType] || { label: m.mealType, icon: Utensils, color: '#6b7280' };
                      return (
                        <div className="template-meal-card" key={i}>
                          <div className="flex items-center gap-2 mb-4">
                            <mt.icon size={16} style={{ color: mt.color }} />
                            <span style={{ fontSize: '0.8rem', fontWeight: 700, color: mt.color, textTransform: 'uppercase', letterSpacing: '0.05em' }}>{mt.label}</span>
                          </div>
                          <h4>{m.dishName}</h4>
                          <p style={{ minHeight: 40 }}>{m.description}</p>
                          <div className="macro-row">
                            <span className="flex items-center gap-1"><Flame size={14} /> {m.nutrition.calories.toFixed(0)}</span>
                            <div className="flex gap-3">
                              <span>Б: <strong>{m.nutrition.proteins.toFixed(0)}</strong>г</span>
                              <span>Ж: <strong>{m.nutrition.fats.toFixed(0)}</strong>г</span>
                              <span>В: <strong>{m.nutrition.carbs.toFixed(0)}</strong>г</span>
                            </div>
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
          <div style={{ display: 'grid', gridTemplateColumns: '1fr 1.5fr', gap: '2rem' }} className="animate-fade-in delay-200">
            <div className="glass-panel" style={{ padding: '2rem' }}>
              <h2 className="flex items-center gap-3 mb-6" style={{ fontSize: '1.4rem', fontWeight: 700 }}>
                <Scale className="text-primary" size={24} /> Записати вагу
              </h2>
              <form onSubmit={handleLogWeight}>
                <div className="form-group">
                  <label className="form-label">Вага (кг)</label>
                  <input type="number" step="0.1" className="form-input" value={weightVal} onChange={e => setWeightVal(e.target.value)} placeholder="напр. 75.5" required min="20" max="400" />
                </div>
                <button type="submit" className="btn btn-primary" disabled={submitting}>
                  {submitting ? 'Зберігаю...' : 'Зберегти запис'}
                </button>
              </form>
            </div>
            <div className="glass-panel" style={{ padding: '2rem' }}>
              <h2 className="flex items-center gap-3 mb-6" style={{ fontSize: '1.4rem', fontWeight: 700 }}>
                <History className="text-primary" size={24} /> Історія (30 днів)
              </h2>
              {weightHistory.length === 0 ? (
                <div style={{ textAlign: 'center', padding: '3rem 0' }}>
                  <Scale className="text-muted" size={40} style={{ opacity: 0.3, marginBottom: '1rem' }} />
                  <p className="text-muted">Ще немає записів ваги</p>
                </div>
              ) : (
                <div className="weight-list">
                  {weightHistory.map(w => (
                    <div className="weight-item" key={w.id}>
                      <span className="flex items-center gap-2"><Calendar size={14} className="text-muted" /> {fmtDateShort(w.loggedAt)}</span>
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
          const RATIOS: Record<number, { p: number; f: number; c: number; label: string; desc: string; icon: any }> = {
            0: { p: 35, f: 25, c: 40, label: 'Схуднення', desc: 'Високий білок для захисту м\'язів та тривалого насичення.', icon: Flame },
            1: { p: 25, f: 20, c: 55, label: 'Набір маси', desc: 'Пріоритет вуглеводам для енергії на тренуваннях та росту.', icon: TrendingUp },
            2: { p: 30, f: 30, c: 40, label: 'Підтримка', desc: 'Оптимальний баланс для здоров\'я та стабільної енергії.', icon: Scale },
          };
          const ratio = RATIOS[tGoal] || RATIOS[2];
          const cals = parseFloat(tCal) || 0;
          const recProt = cals > 0 ? Math.round((cals * ratio.p / 100) / 4) : 0;
          const recFat = cals > 0 ? Math.round((cals * ratio.f / 100) / 9) : 0;
          const recCarb = cals > 0 ? Math.round((cals * ratio.c / 100) / 4) : 0;

          const applyRec = () => { setTProt(String(recProt)); setTFat(String(recFat)); setTCarb(String(recCarb)); };

          return (
          <div style={{ display: 'grid', gridTemplateColumns: '1.2fr 1fr', gap: '2rem', alignItems: 'start' }} className="animate-fade-in delay-200">
            <div className="glass-panel" style={{ padding: '2.5rem' }}>
              <div style={{ textAlign: 'center', marginBottom: '2.5rem' }}>
                <div style={{ 
                  width: 64, height: 64, borderRadius: 20, 
                  background: 'rgba(59,130,246,0.1)', 
                  display: 'flex', alignItems: 'center', justifyContent: 'center',
                  margin: '0 auto 1.5rem'
                }}>
                  <Target className="text-primary" size={32} />
                </div>
                <h2 style={{ fontSize: '1.6rem', fontWeight: 800 }}>Ціль КБЖВ</h2>
                <p className="text-muted">Налаштуйте денні норми для досягнення результату</p>
              </div>
              <form onSubmit={handleSetTarget}>
                <div className="form-group">
                  <label className="form-label">Ваша мета</label>
                  <div style={{ display: 'grid', gridTemplateColumns: 'repeat(3, 1fr)', gap: '0.5rem' }}>
                    {GOAL_OPTIONS.map(g => (
                      <button 
                        key={g.value} 
                        type="button"
                        onClick={() => setTGoal(g.value)}
                        style={{ 
                          padding: '0.75rem 0.5rem', 
                          borderRadius: 12, 
                          border: '1px solid',
                          borderColor: tGoal === g.value ? 'var(--primary)' : 'var(--glass-border)',
                          background: tGoal === g.value ? 'rgba(59,130,246,0.1)' : 'transparent',
                          color: tGoal === g.value ? 'white' : 'var(--text-muted)',
                          fontSize: '0.85rem',
                          fontWeight: 600,
                          cursor: 'pointer',
                          transition: 'all 0.2s'
                        }}
                      >
                        <g.icon size={16} style={{ marginBottom: 4, display: 'block', margin: '0 auto' }} />
                        {g.label}
                      </button>
                    ))}
                  </div>
                </div>
                <div className="form-group">
                  <label className="form-label">Денна калорійність (ккал)</label>
                  <input type="number" className="form-input" value={tCal} onChange={e => setTCal(e.target.value)} placeholder="напр. 2200" required min="500" />
                </div>
                <div style={{ display: 'grid', gridTemplateColumns: '1fr 1fr 1fr', gap: '1rem' }}>
                  <div className="form-group">
                    <label className="form-label">Білки (г)</label>
                    <input type="number" className="form-input" value={tProt} onChange={e => setTProt(e.target.value)} placeholder={String(recProt || '0')} required min="0" />
                  </div>
                  <div className="form-group">
                    <label className="form-label">Жири (г)</label>
                    <input type="number" className="form-input" value={tFat} onChange={e => setTFat(e.target.value)} placeholder={String(recFat || '0')} required min="0" />
                  </div>
                  <div className="form-group">
                    <label className="form-label">Вуглев. (г)</label>
                    <input type="number" className="form-input" value={tCarb} onChange={e => setTCarb(e.target.value)} placeholder={String(recCarb || '0')} required min="0" />
                  </div>
                </div>
                {cals > 0 && (
                  <button type="button" onClick={applyRec} className="btn btn-outline" style={{ marginBottom: '1.5rem', fontSize: '0.85rem', padding: '0.75rem', borderColor: 'rgba(59,130,246,0.3)', color: '#60a5fa' }}>
                    <Activity size={16} className="mr-2" /> Розрахувати за рекомендацією
                  </button>
                )}
                <button type="submit" className="btn btn-primary" disabled={submitting}>
                  {submitting ? 'Збереження...' : 'Зберегти цілі'}
                </button>
              </form>
            </div>

            <div className="glass-panel" style={{ padding: '2rem', border: '1px solid rgba(59,130,246,0.2)' }}>
              <div className="flex items-center gap-3 mb-6">
                <div style={{ width: 40, height: 40, borderRadius: 10, background: 'rgba(59,130,246,0.1)', display: 'flex', alignItems: 'center', justifyContent: 'center' }}>
                  <Info className="text-primary" size={20} />
                </div>
                <h3 style={{ fontSize: '1.2rem', fontWeight: 700 }}>Поради дієтолога</h3>
              </div>

              {cals > 0 ? (
                <div className="flex flex-col gap-6">
                  <div style={{ padding: '1.5rem', background: 'rgba(255,255,255,0.02)', borderRadius: 20, border: '1px solid var(--glass-border)' }}>
                    <div className="flex items-center gap-2 mb-3">
                      <ratio.icon size={18} className="text-primary" />
                      <span style={{ fontWeight: 700, fontSize: '1rem' }}>{ratio.label}</span>
                    </div>
                    <p style={{ fontSize: '0.9rem', color: 'var(--text-muted)', lineHeight: 1.6, marginBottom: '1.5rem' }}>
                      {ratio.desc}
                    </p>
                    <div className="flex flex-col gap-3">
                      {[
                        { label: 'Білки', val: recProt, pct: ratio.p, color: '#ef4444' },
                        { label: 'Жири', val: recFat, pct: ratio.f, color: '#fbbf24' },
                        { label: 'Вуглеводи', val: recCarb, pct: ratio.c, color: '#3b82f6' },
                      ].map(m => (
                        <div key={m.label} className="flex justify-between items-center p-3" style={{ background: 'rgba(255,255,255,0.02)', borderRadius: 12 }}>
                          <div>
                            <div style={{ fontSize: '0.85rem', fontWeight: 700 }}>{m.label}</div>
                            <div style={{ fontSize: '0.75rem', color: 'var(--text-muted)' }}>{m.pct}% від раціону</div>
                          </div>
                          <div style={{ fontSize: '1.1rem', fontWeight: 800, color: m.color }}>{m.val}г</div>
                        </div>
                      ))}
                    </div>
                  </div>
                  
                  <div style={{ padding: '1rem', background: 'rgba(59,130,246,0.05)', borderRadius: 14, fontSize: '0.8rem', color: 'var(--text-muted)', lineHeight: 1.6 }}>
                    <strong>Як ми рахуємо:</strong><br />
                    1г білка = 4 ккал | 1г вуглеводів = 4 ккал | 1г жиру = 9 ккал
                  </div>
                </div>
              ) : (
                <div style={{ textAlign: 'center', padding: '4rem 1rem' }}>
                  <Target size={48} className="text-muted" style={{ opacity: 0.2, marginBottom: '1.5rem' }} />
                  <p className="text-muted">Введіть калорійність, щоб отримати персоналізовані розрахунки</p>
                </div>
              )}
            </div>
          </div>
          );
        })()}
      </main>
    </div>
  );
};

export default NutritionDashboard;

