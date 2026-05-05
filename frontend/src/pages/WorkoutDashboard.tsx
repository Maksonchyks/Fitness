import { useState, useEffect } from 'react';
import { useNavigate } from 'react-router-dom';
import { Dumbbell, Activity, Calendar, Play, CheckCircle, ChevronDown, Loader2, Trophy, History as HistoryIcon, Trash2, AlertTriangle, RefreshCw } from 'lucide-react';

interface Exercise {
  exerciseType: string;
  weight: number;
  reps: number;
  sets: number;
}

interface TrainingDay {
  id: string;
  dayNumber: number;
  exercises: Exercise[];
}

interface Program {
  id: string;
  createdAt: string;
  goal: string;
  intensity: string;
  trainingDays: TrainingDay[];
}

const GOAL_INFO: Record<string, { label: string; color: string; bg: string; emoji: string }> = {
  Fitness: { label: 'General Fitness', color: '#3b82f6', bg: 'rgba(59,130,246,0.12)', emoji: '🏃' },
  Bodybuilding: { label: 'Bodybuilding', color: '#8b5cf6', bg: 'rgba(139,92,246,0.12)', emoji: '💪' },
  Powerlifting: { label: 'Powerlifting', color: '#f59e0b', bg: 'rgba(245,158,11,0.12)', emoji: '🏋️' },
  WeightLoss: { label: 'Weight Loss', color: '#10b981', bg: 'rgba(16,185,129,0.12)', emoji: '🔥' },
  Endurance: { label: 'Endurance', color: '#06b6d4', bg: 'rgba(6,182,212,0.12)', emoji: '🫀' },
  Rehabilitation: { label: 'Rehabilitation', color: '#ec4899', bg: 'rgba(236,72,153,0.12)', emoji: '🩺' },
};
const GOAL_NUM_TO_KEY: Record<number, string> = { 1: 'Fitness', 2: 'Bodybuilding', 3: 'Powerlifting', 4: 'WeightLoss', 5: 'Endurance', 6: 'Rehabilitation' };
const INTENSITY_LABEL: Record<string, string> = { Low: '2 days/week', Moderate: '3 days/week', High: '4-5 days/week' };

const WorkoutDashboard = () => {
  const navigate = useNavigate();
  const [activeProgram, setActiveProgram] = useState<Program | null>(null);
  const [isLoading, setIsLoading] = useState(true);
  const [isGenerating, setIsGenerating] = useState(false);
  const [selectedDay, setSelectedDay] = useState<TrainingDay | null>(null);
  const [sessionResults, setSessionResults] = useState<{ [key: number]: { weight: number, reps: number, sets: number } }>({});
  const [isSubmitting, setIsSubmitting] = useState(false);
  const [error, setError] = useState<string | null>(null);

  // Form states for generation
  const [goal, setGoal] = useState(1); // 1 = Fitness
  const [intensity, setIntensity] = useState(3); // 3 = Moderate
  const [squat, setSquat] = useState('');
  const [bench, setBench] = useState('');
  const [deadlift, setDeadlift] = useState('');

  // Tabs and History
  const [activeTab, setActiveTab] = useState<'current' | 'history'>('current');
  const [sessionHistory, setSessionHistory] = useState<any[]>([]);
  const [programHistory, setProgramHistory] = useState<any[]>([]);
  const [isHistoryLoading, setIsHistoryLoading] = useState(false);
  const [expandedSessions, setExpandedSessions] = useState<Set<string>>(new Set());
  const [showConfirmModal, setShowConfirmModal] = useState(false);

  const needsManualMetrics = (g: number) => g === 2 || g === 3;

  const fetchProgram = async () => {
    setIsLoading(true);
    setError(null);
    const token = localStorage.getItem('accessToken');
    if (!token) {
      navigate('/login');
      return;
    }

    try {
      const response = await fetch('http://localhost:5001/api/programs/active', {
        headers: { 'Authorization': `Bearer ${token}` }
      });

      if (response.ok) {
        const text = await response.text();
        const data = text ? JSON.parse(text) : null;
        setActiveProgram(data);
      } else if (response.status === 401) {
        navigate('/login');
      } else if (response.status === 404 || response.status === 204) {
        setActiveProgram(null);
      } else {
        const errData = await response.json().catch(() => ({}));
        setError(`Failed to load program: ${errData.detail || response.statusText}`);
        setActiveProgram(null);
      }
    } catch (err: any) {
      console.error('Failed to load program', err);
      setError(`Network error: ${err.message}. Make sure the Workout API is running.`);
      setActiveProgram(null);
    } finally {
      setIsLoading(false);
    }
  };

  const fetchHistory = async () => {
    setIsHistoryLoading(true);
    const token = localStorage.getItem('accessToken');
    try {
      const [progRes, sessRes] = await Promise.all([
        fetch('http://localhost:5001/api/programs/history', { headers: { 'Authorization': `Bearer ${token}` } }),
        fetch('http://localhost:5001/api/sessions/history', { headers: { 'Authorization': `Bearer ${token}` } })
      ]);

      if (progRes.ok) {
        const text = await progRes.text();
        setProgramHistory(text ? JSON.parse(text) : []);
      }
      if (sessRes.ok) {
        const text = await sessRes.text();
        setSessionHistory(text ? JSON.parse(text) : []);
      }
    } catch (err) {
      console.error('Failed to load history', err);
    } finally {
      setIsHistoryLoading(false);
    }
  };

  const handleDeleteProgram = async (id: string) => {
    if (!window.confirm('Are you sure you want to delete this program template?')) return;
    const token = localStorage.getItem('accessToken');
    try {
      const res = await fetch(`http://localhost:5001/api/programs/${id}`, {
        method: 'DELETE',
        headers: { 'Authorization': `Bearer ${token}` }
      });
      if (res.ok) {
        setProgramHistory(prev => prev.filter(p => p.id !== id));
        if (activeProgram?.id === id) setActiveProgram(null);
      }
    } catch (err) {
      console.error(err);
    }
  };

  const handleDeleteSession = async (id: string) => {
    if (!window.confirm('Are you sure you want to delete this session? This will affect your progressive overload.')) return;
    const token = localStorage.getItem('accessToken');
    try {
      const res = await fetch(`http://localhost:5001/api/sessions/${id}`, {
        method: 'DELETE',
        headers: { 'Authorization': `Bearer ${token}` }
      });
      if (res.ok) {
        setSessionHistory(prev => prev.filter(s => s.id !== id));
      }
    } catch (err) {
      console.error(err);
    }
  };

  useEffect(() => {
    fetchProgram();
    fetchHistory();
  }, [navigate]);

  const handleGenerate = async (e: React.FormEvent) => {
    e.preventDefault();
    // If there's already an active program, show confirmation modal first
    if (activeProgram) {
      setShowConfirmModal(true);
      return;
    }
    await doGenerate();
  };

  const doGenerate = async () => {
    setShowConfirmModal(false);
    setIsGenerating(true);
    setError(null);
    const token = localStorage.getItem('accessToken');

    const body: any = {
      fitnessGoal: Number(goal),
      intensity: Number(intensity)
    };

    if (squat && bench && deadlift) {
      body.powerMetrics = {
        squatWeight: Number(squat),
        benchPressWeight: Number(bench),
        deadliftWeight: Number(deadlift)
      };
    }

    try {
      const response = await fetch('http://localhost:5001/api/programs/generate', {
        method: 'POST',
        headers: {
          'Content-Type': 'application/json',
          'Authorization': `Bearer ${token}`
        },
        body: JSON.stringify(body)
      });

      if (response.ok) {
        setSquat(''); setBench(''); setDeadlift('');
        await fetchProgram();
        await fetchHistory();
      } else {
        const errData = await response.json().catch(() => ({}));
        setError(`Failed to generate program: ${errData.detail || errData.title || response.statusText}`);
      }
    } catch (err: any) {
      console.error(err);
      setError(`Network error: ${err.message}. Make sure the Workout API is running.`);
    } finally {
      setIsGenerating(false);
    }
  };

  const handleStartSession = (day: TrainingDay) => {
    setSelectedDay(day);
    // Initialize results state
    const initialResults: any = {};
    day.exercises.forEach((ex, idx) => {
      initialResults[idx] = { weight: ex.weight, reps: ex.reps, sets: ex.sets };
    });
    setSessionResults(initialResults);
  };

  const submitSession = async () => {
    if (!selectedDay) return;
    setIsSubmitting(true);
    setError(null);
    const token = localStorage.getItem('accessToken');

    const results = selectedDay.exercises.map((ex, idx) => ({
      exerciseType: ex.exerciseType,
      weight: sessionResults[idx].weight,
      reps: sessionResults[idx].reps,
      sets: sessionResults[idx].sets
    }));

    try {
      const response = await fetch('http://localhost:5001/api/sessions', {
        method: 'POST',
        headers: {
          'Content-Type': 'application/json',
          'Authorization': `Bearer ${token}`
        },
        body: JSON.stringify({
          trainingDayId: selectedDay.id,
          results
        })
      });

      if (response.ok) {
        alert('Session recorded successfully! Awesome job!');
        setSelectedDay(null);
        // We could fetch program again or just close
      } else {
        const errData = await response.json().catch(() => ({}));
        setError(`Failed to record session: ${errData.detail || response.statusText}`);
      }
    } catch (err: any) {
      console.error(err);
      setError(`Network error: ${err.message}. Make sure the Workout API is running.`);
    } finally {
      setIsSubmitting(false);
    }
  };

  const formatExerciseName = (type: any): string => {
    const name = String(type);
    // Split PascalCase: "BenchPress" → "Bench Press"
    return name.replace(/([A-Z])/g, ' $1').trim();
  };

  const formatDate = (dateStr: string | null | undefined): string => {
    if (!dateStr) return 'Unknown date';
    const d = new Date(dateStr);
    if (isNaN(d.getTime())) return 'Unknown date';
    return d.toLocaleDateString('uk-UA', { year: 'numeric', month: 'short', day: 'numeric' });
  };

  if (isLoading) {
    return (
      <div style={{ display: 'flex', justifyContent: 'center', alignItems: 'center', height: '100vh', background: 'linear-gradient(135deg, var(--bg-darker) 0%, var(--bg-dark) 100%)' }}>
        <Loader2 className="animate-spin" size={48} color="var(--primary)" />
      </div>
    );
  }

  return (
    <div style={{ padding: '2rem 1rem' }}>
      <header style={{ maxWidth: '1000px', margin: '0 auto 2.5rem' }}>
        <div style={{ display: 'flex', alignItems: 'center', gap: '1rem' }}>
          <div style={{ width: '48px', height: '48px', borderRadius: '14px', background: 'linear-gradient(135deg, #8b5cf6, #3b82f6)', display: 'flex', alignItems: 'center', justifyContent: 'center', boxShadow: '0 8px 16px rgba(139, 92, 246, 0.3)' }}>
            <Activity size={24} color="white" />
          </div>
          <div>
            <h1 style={{ fontSize: '1.75rem', fontWeight: '800', margin: 0 }}>Мої тренування</h1>
            <p style={{ color: 'var(--text-muted)', margin: 0, fontSize: '0.9rem' }}>Плануйте та відстежуйте свій прогрес</p>
          </div>
        </div>
      </header>

      <main style={{ maxWidth: '1000px', margin: '0 auto' }}>
        <div style={{ display: 'flex', gap: '1rem', marginBottom: '2rem', borderBottom: '1px solid var(--glass-border)', paddingBottom: '1rem' }}>
          <button
            className={`btn ${activeTab === 'current' ? 'btn-primary' : 'btn-outline'}`}
            onClick={() => setActiveTab('current')}
            style={{ width: 'auto', padding: '0.5rem 1rem' }}
          >
            <Activity size={18} style={{ marginRight: '0.5rem' }} /> Current Workout
          </button>
          <button
            className={`btn ${activeTab === 'history' ? 'btn-primary' : 'btn-outline'}`}
            onClick={() => setActiveTab('history')}
            style={{ width: 'auto', padding: '0.5rem 1rem' }}
          >
            <HistoryIcon size={18} style={{ marginRight: '0.5rem' }} /> History & Records
          </button>
        </div>

        {error && (
          <div style={{ padding: '1rem', background: 'rgba(239, 68, 68, 0.1)', border: '1px solid rgba(239, 68, 68, 0.3)', borderRadius: '12px', color: '#fca5a5', marginBottom: '2rem', display: 'flex', alignItems: 'center', gap: '0.75rem' }}>
            <span>{error}</span>
          </div>
        )}

        {activeTab === 'history' ? (
          <div style={{ display: 'grid', gridTemplateColumns: '1fr', gap: '2rem' }}>
            <div className="glass-panel" style={{ padding: '2rem' }}>
              <h2 style={{ fontSize: '1.5rem', fontWeight: '600', marginBottom: '1.5rem', display: 'flex', alignItems: 'center', gap: '0.5rem' }}>
                <CheckCircle size={24} color="#10b981" /> Actual Sessions Completed
              </h2>
              {isHistoryLoading ? <Loader2 className="animate-spin" /> : sessionHistory.length === 0 ? (
                <p className="text-muted">No sessions recorded yet.</p>
              ) : (() => {
                const dayToProgram = new Map<string, any>();
                programHistory.forEach(prog => { prog.trainingDays?.forEach((day: any) => dayToProgram.set(day.id, prog)); });
                const groups = new Map<string, { prog: any; sessions: any[] }>();
                const unlinked: any[] = [];
                sessionHistory.forEach((s: any) => {
                  const p = dayToProgram.get(s.trainingDayId);
                  if (p) { if (!groups.has(p.id)) groups.set(p.id, { prog: p, sessions: [] }); groups.get(p.id)!.sessions.push(s); }
                  else { unlinked.push(s); }
                });
                const sorted = [...groups.values()].sort((a, b) => new Date(b.prog.createdAt).getTime() - new Date(a.prog.createdAt).getTime());
                const renderCard = (session: any) => {
                  const isExp = expandedSessions.has(session.id);
                  const toggle = () => setExpandedSessions(prev => { const n = new Set(prev); n.has(session.id) ? n.delete(session.id) : n.add(session.id); return n; });
                  return (
                    <div key={session.id} style={{ background: 'rgba(255,255,255,0.02)', borderRadius: '10px', border: `1px solid ${isExp ? 'rgba(59,130,246,0.4)' : 'var(--glass-border)'}`, overflow: 'hidden', transition: 'border-color 0.2s' }}>
                      <div style={{ display: 'flex', justifyContent: 'space-between', alignItems: 'center', padding: '0.85rem 1rem' }}>
                        <button onClick={toggle} style={{ background: 'none', border: 'none', cursor: 'pointer', display: 'flex', alignItems: 'center', gap: '0.65rem', flex: 1, textAlign: 'left' }}>
                          <div style={{ width: '32px', height: '32px', borderRadius: '8px', background: 'rgba(16,185,129,0.15)', display: 'flex', alignItems: 'center', justifyContent: 'center', flexShrink: 0 }}><CheckCircle size={16} color="#10b981" /></div>
                          <div>
                            <h4 style={{ fontWeight: '600', color: 'white', margin: 0, fontSize: '0.9rem' }}>Session · {formatDate(session.date)}</h4>
                            <p style={{ fontSize: '0.78rem', color: 'var(--text-muted)', margin: 0 }}>{session.performedExercises?.length || 0} exercises</p>
                          </div>
                          <ChevronDown size={16} color="var(--text-muted)" style={{ marginLeft: 'auto', marginRight: '0.5rem', transform: isExp ? 'rotate(180deg)' : 'rotate(0deg)', transition: 'transform 0.25s ease' }} />
                        </button>
                        <button onClick={() => handleDeleteSession(session.id)} className="btn btn-outline" style={{ width: 'auto', padding: '0.35rem', color: '#ef4444', borderColor: 'rgba(239,68,68,0.3)', flexShrink: 0 }}><Trash2 size={14} /></button>
                      </div>
                      {isExp && (
                        <div style={{ borderTop: '1px solid var(--glass-border)', padding: '0.875rem 1rem' }}>
                          {session.performedExercises?.length > 0 ? (
                            <table style={{ width: '100%', borderCollapse: 'collapse', fontSize: '0.85rem' }}>
                              <thead><tr>
                                <th style={{ textAlign: 'left', color: 'var(--text-muted)', fontWeight: '500', paddingBottom: '0.4rem', width: '50%' }}>Exercise</th>
                                <th style={{ textAlign: 'center', color: 'var(--text-muted)', fontWeight: '500', paddingBottom: '0.4rem' }}>Sets</th>
                                <th style={{ textAlign: 'center', color: 'var(--text-muted)', fontWeight: '500', paddingBottom: '0.4rem' }}>Reps</th>
                                <th style={{ textAlign: 'right', color: 'var(--text-muted)', fontWeight: '500', paddingBottom: '0.4rem' }}>Weight</th>
                              </tr></thead>
                              <tbody>{session.performedExercises.map((ex: any, i: number) => (
                                <tr key={i} style={{ borderTop: '1px solid rgba(255,255,255,0.04)' }}>
                                  <td style={{ padding: '0.4rem 0', color: 'white', fontWeight: '500' }}>{formatExerciseName(ex.exerciseType)}</td>
                                  <td style={{ textAlign: 'center', color: 'var(--text-muted)', padding: '0.4rem 0' }}>{ex.sets}</td>
                                  <td style={{ textAlign: 'center', color: 'var(--text-muted)', padding: '0.4rem 0' }}>{ex.reps}</td>
                                  <td style={{ textAlign: 'right', padding: '0.4rem 0' }}><span style={{ color: 'var(--primary)', fontWeight: '600' }}>{ex.weight} kg</span></td>
                                </tr>
                              ))}</tbody>
                            </table>
                          ) : <p style={{ color: 'var(--text-muted)', fontSize: '0.85rem', margin: 0 }}>No exercise data.</p>}
                        </div>
                      )}
                    </div>
                  );
                };
                return (
                  <div style={{ display: 'flex', flexDirection: 'column', gap: '1.25rem' }}>
                    {sorted.map(({ prog, sessions }) => {
                      const gi = GOAL_INFO[prog.goal] ?? { label: prog.goal || 'Program', color: '#6b7280', bg: 'rgba(107,114,128,0.1)', emoji: '📋' };
                      return (
                        <div key={prog.id}>
                          <div style={{ display: 'flex', alignItems: 'center', gap: '0.6rem', marginBottom: '0.6rem', padding: '0.5rem 0.875rem', background: gi.bg, borderRadius: '8px', border: `1px solid ${gi.color}30` }}>
                            <span style={{ fontSize: '1rem' }}>{gi.emoji}</span>
                            <span style={{ fontWeight: '700', color: gi.color, fontSize: '0.9rem' }}>{gi.label}</span>
                            <span style={{ color: 'var(--text-muted)', fontSize: '0.78rem' }}>· {formatDate(prog.createdAt)}</span>
                            <span style={{ marginLeft: 'auto', fontSize: '0.75rem', color: 'var(--text-muted)' }}>{sessions.length} session{sessions.length !== 1 ? 's' : ''}</span>
                          </div>
                          <div style={{ display: 'flex', flexDirection: 'column', gap: '0.5rem', paddingLeft: '0.75rem', borderLeft: `2px solid ${gi.color}35` }}>
                            {sessions.map(renderCard)}
                          </div>
                        </div>
                      );
                    })}
                    {unlinked.length > 0 && (
                      <div>
                        <div style={{ display: 'flex', alignItems: 'center', gap: '0.6rem', marginBottom: '0.6rem', padding: '0.5rem 0.875rem', background: 'rgba(107,114,128,0.08)', borderRadius: '8px', border: '1px solid rgba(107,114,128,0.2)' }}>
                          <span>📂</span><span style={{ fontWeight: '700', color: 'var(--text-muted)', fontSize: '0.9rem' }}>Other Sessions</span>
                        </div>
                        <div style={{ display: 'flex', flexDirection: 'column', gap: '0.5rem', paddingLeft: '0.75rem', borderLeft: '2px solid rgba(107,114,128,0.3)' }}>
                          {unlinked.map(renderCard)}
                        </div>
                      </div>
                    )}
                  </div>
                );
              })()}
            </div>

            <div className="glass-panel" style={{ padding: '2rem' }}>

              <h2 style={{ fontSize: '1.5rem', fontWeight: '600', marginBottom: '1.5rem', display: 'flex', alignItems: 'center', gap: '0.5rem' }}>
                <Calendar size={24} color="#3b82f6" /> Generated Templates
              </h2>
              {isHistoryLoading ? <Loader2 className="animate-spin" /> : programHistory.length === 0 ? (
                <p className="text-muted">No templates generated yet.</p>
              ) : (
                <div style={{ display: 'flex', flexDirection: 'column', gap: '1rem' }}>
                  {programHistory.map(prog => {
                    const gi = GOAL_INFO[prog.goal] ?? { label: prog.goal || 'Program', color: '#6b7280', bg: 'rgba(107,114,128,0.12)', emoji: '📋' };
                    const intLabel = INTENSITY_LABEL[prog.intensity] ?? prog.intensity ?? '';
                    return (
                      <div key={prog.id} style={{ padding: '1.1rem 1.25rem', background: 'rgba(255,255,255,0.02)', borderRadius: '12px', border: `1px solid ${gi.color}30`, display: 'flex', justifyContent: 'space-between', alignItems: 'center', gap: '1rem' }}>
                        <div style={{ flex: 1, minWidth: 0 }}>
                          <div style={{ display: 'flex', alignItems: 'center', gap: '0.5rem', flexWrap: 'wrap', marginBottom: '0.35rem' }}>
                            <span style={{ fontSize: '1rem' }}>{gi.emoji}</span>
                            <span style={{ fontWeight: '700', color: gi.color }}>{gi.label}</span>
                            {intLabel && <span style={{ fontSize: '0.73rem', padding: '0.12rem 0.5rem', borderRadius: '999px', background: 'rgba(255,255,255,0.06)', color: 'var(--text-muted)' }}>{intLabel}</span>}
                          </div>
                          <p style={{ fontSize: '0.82rem', color: 'var(--text-muted)', margin: 0 }}>
                            {formatDate(prog.createdAt)} · {prog.trainingDays?.length || 0} days planned
                          </p>
                        </div>
                        <button onClick={() => handleDeleteProgram(prog.id)} className="btn btn-outline" style={{ width: 'auto', padding: '0.5rem', color: '#ef4444', borderColor: 'rgba(239,68,68,0.3)', flexShrink: 0 }}>
                          <Trash2 size={16} />
                        </button>
                      </div>
                    );
                  })}
                </div>
              )}
            </div>
          </div>
        ) : selectedDay ? (
          // Active Session View
          <div className="glass-panel" style={{ padding: '2rem' }}>
            <div style={{ display: 'flex', justifyContent: 'space-between', alignItems: 'center', marginBottom: '2rem' }}>
              <div>
                <h2 style={{ fontSize: '1.5rem', fontWeight: '600' }}>Day {selectedDay.dayNumber} Session</h2>
                <p className="text-muted">Record your actual performance</p>
              </div>
              <button onClick={() => setSelectedDay(null)} className="btn btn-outline" style={{ width: 'auto', padding: '0.5rem 1rem' }}>Cancel</button>
            </div>

            <div style={{ display: 'flex', flexDirection: 'column', gap: '1.5rem' }}>
              {selectedDay.exercises.map((ex, idx) => (
                <div key={idx} style={{ padding: '1.5rem', background: 'rgba(255,255,255,0.02)', borderRadius: '12px', border: '1px solid var(--glass-border)' }}>
                  <div style={{ display: 'flex', justifyContent: 'space-between', alignItems: 'center', marginBottom: '1rem' }}>
                    <h3 style={{ fontSize: '1.1rem', fontWeight: '600', color: 'var(--primary)' }}>
                      {formatExerciseName(ex.exerciseType)}
                    </h3>
                    <span style={{ fontSize: '0.9rem', color: 'var(--text-muted)' }}>Target: {ex.sets} sets x {ex.reps} reps @ {ex.weight}kg</span>
                  </div>

                  <div style={{ display: 'grid', gridTemplateColumns: '1fr 1fr 1fr', gap: '1rem' }}>
                    <div className="form-group" style={{ marginBottom: 0 }}>
                      <label className="form-label">Actual Weight (kg)</label>
                      <input
                        type="number"
                        className="form-input"
                        value={sessionResults[idx]?.weight || ''}
                        onChange={(e) => setSessionResults({ ...sessionResults, [idx]: { ...sessionResults[idx], weight: Number(e.target.value) } })}
                      />
                    </div>
                    <div className="form-group" style={{ marginBottom: 0 }}>
                      <label className="form-label">Actual Reps</label>
                      <input
                        type="number"
                        className="form-input"
                        value={sessionResults[idx]?.reps || ''}
                        onChange={(e) => setSessionResults({ ...sessionResults, [idx]: { ...sessionResults[idx], reps: Number(e.target.value) } })}
                      />
                    </div>
                    <div className="form-group" style={{ marginBottom: 0 }}>
                      <label className="form-label">Actual Sets</label>
                      <input
                        type="number"
                        className="form-input"
                        value={sessionResults[idx]?.sets || ''}
                        onChange={(e) => setSessionResults({ ...sessionResults, [idx]: { ...sessionResults[idx], sets: Number(e.target.value) } })}
                      />
                    </div>
                  </div>
                </div>
              ))}
            </div>

            <div style={{ marginTop: '2rem', display: 'flex', justifyContent: 'flex-end' }}>
              <button onClick={submitSession} className="btn btn-primary" style={{ width: 'auto', padding: '0.75rem 2rem' }} disabled={isSubmitting}>
                {isSubmitting ? <Loader2 className="animate-spin" size={20} /> : <><CheckCircle size={20} style={{ marginRight: '0.5rem' }} /> Complete Session</>}
              </button>
            </div>
          </div>
        ) : activeProgram ? (
          // Active Program View
          <div className="glass-panel" style={{ padding: '2rem' }}>
            <div style={{ display: 'flex', justifyContent: 'space-between', alignItems: 'flex-start', marginBottom: '2rem' }}>
              <div>
                <h2 style={{ fontSize: '1.5rem', fontWeight: '600', display: 'flex', alignItems: 'center', gap: '0.5rem' }}>
                  <Trophy size={24} color="#f59e0b" /> Current Program
                </h2>
                <p className="text-muted" style={{ marginTop: '0.25rem' }}>Generated on {formatDate(activeProgram.createdAt)}</p>
              </div>
              <button onClick={() => setActiveProgram(null)} className="btn btn-outline" style={{ width: 'auto', padding: '0.5rem 1rem', fontSize: '0.85rem' }}>
                Generate New
              </button>
            </div>

            <div style={{ display: 'grid', gridTemplateColumns: 'repeat(auto-fill, minmax(300px, 1fr))', gap: '1.5rem' }}>
              {activeProgram.trainingDays.map(day => (
                <div key={day.id} style={{ padding: '1.5rem', background: 'rgba(30, 41, 59, 0.4)', borderRadius: '16px', border: '1px solid var(--glass-border)', display: 'flex', flexDirection: 'column' }}>
                  <div style={{ display: 'flex', justifyContent: 'space-between', alignItems: 'center', marginBottom: '1rem' }}>
                    <div style={{ display: 'flex', alignItems: 'center', gap: '0.5rem' }}>
                      <Calendar size={18} color="var(--primary)" />
                      <h3 style={{ fontSize: '1.1rem', fontWeight: '600' }}>Day {day.dayNumber}</h3>
                    </div>
                    <span style={{ fontSize: '0.8rem', background: 'rgba(59, 130, 246, 0.1)', color: 'var(--primary)', padding: '0.2rem 0.6rem', borderRadius: '999px' }}>
                      {day.exercises.length} Exercises
                    </span>
                  </div>

                  <div style={{ flex: 1, marginBottom: '1.5rem' }}>
                    <ul style={{ listStyle: 'none', padding: 0, margin: 0, display: 'flex', flexDirection: 'column', gap: '0.5rem' }}>
                      {day.exercises.slice(0, 3).map((ex, idx) => (
                        <li key={idx} style={{ fontSize: '0.9rem', color: 'var(--text-muted)', display: 'flex', justifyContent: 'space-between' }}>
                          <span>{formatExerciseName(ex.exerciseType)}</span>
                          <span style={{ color: 'white' }}>{ex.weight}kg</span>
                        </li>
                      ))}
                      {day.exercises.length > 3 && (
                        <li style={{ fontSize: '0.85rem', color: 'var(--primary)', fontStyle: 'italic', marginTop: '0.25rem' }}>
                          + {day.exercises.length - 3} more...
                        </li>
                      )}
                    </ul>
                  </div>

                  <button
                    onClick={() => handleStartSession(day)}
                    className="btn btn-primary"
                    style={{ width: '100%', padding: '0.75rem', display: 'flex', justifyContent: 'center', gap: '0.5rem' }}
                  >
                    <Play size={18} /> Start Workout
                  </button>
                </div>
              ))}
            </div>
          </div>
        ) : (
          // Generate Program Form
          <div className="glass-panel" style={{ padding: '2rem', maxWidth: '600px', margin: '0 auto' }}>
            <div style={{ textAlign: 'center', marginBottom: '2rem' }}>
              <Dumbbell size={48} color="var(--primary)" style={{ margin: '0 auto 1rem' }} />
              <h2 style={{ fontSize: '1.5rem', fontWeight: '600' }}>Generate New Program</h2>
              <p className="text-muted" style={{ marginTop: '0.5rem' }}>We'll create a custom workout plan based on your goals and past performance.</p>
            </div>

            <form onSubmit={handleGenerate}>
              <div className="form-group">
                <label className="form-label">Fitness Goal</label>
                <select className="form-input" value={goal} onChange={(e) => setGoal(Number(e.target.value))}>
                  <option value={1}>General Fitness</option>
                  <option value={2}>Bodybuilding</option>
                  <option value={3}>Powerlifting</option>
                  <option value={4}>Weight Loss</option>
                  <option value={5}>Endurance</option>
                  <option value={6}>Rehabilitation</option>
                </select>
              </div>

              <div className="form-group">
                <label className="form-label">Intensity Level</label>
                <select className="form-input" value={intensity} onChange={(e) => setIntensity(Number(e.target.value))}>
                  <option value={2}>Low (2 days/week)</option>
                  <option value={3}>Moderate (3 days/week)</option>
                  <option value={4}>High (4-5 days/week)</option>
                </select>
              </div>

              {/* Always show 1RM fields if: no sessions, OR goal requires it (Bodybuilding/Powerlifting) */}
              {(sessionHistory.length === 0 || needsManualMetrics(goal)) && (
                <div style={{ padding: '1.5rem', background: 'rgba(59, 130, 246, 0.05)', borderRadius: '12px', border: '1px solid rgba(59, 130, 246, 0.1)', marginBottom: '2rem' }}>
                  <h3 style={{ fontSize: '1rem', fontWeight: '500', marginBottom: '1rem', color: 'var(--primary)' }}>Current Maxes (1RM)</h3>
                  <p style={{ fontSize: '0.85rem', color: 'var(--text-muted)', marginBottom: '1rem' }}>
                    {sessionHistory.length === 0
                      ? 'First workout? Enter your estimated 1RM. Future plans will use your actual performance!'
                      : 'Bodybuilding & Powerlifting use your 1RM to calibrate weights. Leave blank to auto-calculate from your last sessions.'}
                  </p>
                  <div style={{ display: 'grid', gridTemplateColumns: '1fr 1fr 1fr', gap: '1rem' }}>
                    <div className="form-group" style={{ marginBottom: 0 }}>
                      <label className="form-label" style={{ fontSize: '0.8rem' }}>Squat (kg)</label>
                      <input type="number" className="form-input" value={squat} onChange={(e) => setSquat(e.target.value)} placeholder="e.g. 100" />
                    </div>
                    <div className="form-group" style={{ marginBottom: 0 }}>
                      <label className="form-label" style={{ fontSize: '0.8rem' }}>Bench (kg)</label>
                      <input type="number" className="form-input" value={bench} onChange={(e) => setBench(e.target.value)} placeholder="e.g. 80" />
                    </div>
                    <div className="form-group" style={{ marginBottom: 0 }}>
                      <label className="form-label" style={{ fontSize: '0.8rem' }}>Deadlift (kg)</label>
                      <input type="number" className="form-input" value={deadlift} onChange={(e) => setDeadlift(e.target.value)} placeholder="e.g. 120" />
                    </div>
                  </div>
                </div>
              )}

              <button type="submit" className="btn btn-primary" disabled={isGenerating}>
                {isGenerating ? <Loader2 className="animate-spin" size={20} /> : 'Generate My Plan'}
              </button>
            </form>
          </div>
        )}
      </main>

      {/* ── Confirmation Modal ────────────────────────── */}
      {showConfirmModal && (
        <div style={{ position: 'fixed', inset: 0, background: 'rgba(0,0,0,0.65)', backdropFilter: 'blur(6px)', display: 'flex', alignItems: 'center', justifyContent: 'center', zIndex: 1000, padding: '1rem' }}>
          <div className="glass-panel" style={{ padding: '2rem', maxWidth: '480px', width: '100%', borderRadius: '20px', border: '1px solid rgba(245,158,11,0.3)' }}>
            <div style={{ display: 'flex', alignItems: 'center', gap: '0.75rem', marginBottom: '1.25rem' }}>
              <div style={{ width: '44px', height: '44px', borderRadius: '12px', background: 'rgba(245,158,11,0.15)', display: 'flex', alignItems: 'center', justifyContent: 'center', flexShrink: 0 }}>
                <AlertTriangle size={22} color="#f59e0b" />
              </div>
              <div>
                <h3 style={{ fontWeight: '700', fontSize: '1.1rem', margin: 0 }}>Generate New Program?</h3>
                <p style={{ fontSize: '0.82rem', color: 'var(--text-muted)', margin: 0, marginTop: '0.2rem' }}>You already have an active program</p>
              </div>
            </div>

            {activeProgram && GOAL_NUM_TO_KEY[goal] !== activeProgram.goal && (
              <div style={{ padding: '0.875rem 1rem', background: 'rgba(239,68,68,0.08)', border: '1px solid rgba(239,68,68,0.25)', borderRadius: '10px', marginBottom: '1.25rem', display: 'flex', gap: '0.6rem' }}>
                <RefreshCw size={16} color="#f87171" style={{ flexShrink: 0, marginTop: '2px' }} />
                <p style={{ fontSize: '0.85rem', color: '#fca5a5', margin: 0 }}>
                  <strong>Goal change detected:</strong> switching from <strong>{GOAL_INFO[activeProgram.goal]?.label ?? 'previous goal'}</strong> → <strong>{GOAL_INFO[GOAL_NUM_TO_KEY[goal]]?.label ?? 'new goal'}</strong>.
                  The new program will use your provided 1RM values, or auto-calculate from your last sessions.
                </p>
              </div>
            )}

            <p style={{ fontSize: '0.9rem', color: 'var(--text-muted)', marginBottom: '1.5rem', lineHeight: '1.6' }}>
              Your current program will be moved to history. Progressive overload will be recalculated based on your recent sessions {squat || bench || deadlift ? 'and the 1RM values you entered' : '(auto-calculated)'}.
            </p>

            <div style={{ display: 'flex', gap: '0.75rem' }}>
              <button
                onClick={() => setShowConfirmModal(false)}
                className="btn btn-outline"
                style={{ flex: 1 }}
                disabled={isGenerating}
              >
                Cancel
              </button>
              <button
                onClick={doGenerate}
                className="btn btn-primary"
                style={{ flex: 1 }}
                disabled={isGenerating}
              >
                {isGenerating ? <Loader2 className="animate-spin" size={18} /> : <><RefreshCw size={16} style={{ marginRight: '0.4rem' }} /> Yes, Generate</>}
              </button>
            </div>
          </div>
        </div>
      )}
    </div>
  );
};

export default WorkoutDashboard;
