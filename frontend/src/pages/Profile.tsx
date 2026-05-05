import { useState, useEffect } from 'react';
import { useNavigate } from 'react-router-dom';
import { User, Settings, LogOut, Dumbbell, Activity, Save, Loader2, Camera, Shield, Bell, Clock, Plus, Trash2 } from 'lucide-react';
import { communicationService } from '../services/communicationService';

const Profile = () => {
  const navigate = useNavigate();
  const [isLoading, setIsLoading] = useState(false);
  const [isSaving, setIsSaving] = useState(false);
  const [activeTab, setActiveTab] = useState('personal'); // 'personal', 'fitness', 'notifications'

  const [profileData, setProfileData] = useState({
    firstName: 'Alex',
    lastName: 'Johnson',
    email: 'alex@example.com',
    username: 'alexj',
    dateOfBirth: '1995-05-15',
    gender: 'Male',
    fitnessGoal: 'Muscle Gain',
    roles: [] as string[]
  });

  const [notifSettings, setNotifSettings] = useState({
    nutritionEnabled: true,
    workoutEnabled: true,
    schedules: [] as { time: string, label: string, type: string }[]
  });

  useEffect(() => {
    const fetchProfile = async () => {
      setIsLoading(true);
      const token = localStorage.getItem('accessToken');
      const userId = localStorage.getItem('userId');
      if (!token) {
        navigate('/login');
        return;
      }

      try {
        // Fetch User Profile
        const profileRes = await fetch('http://localhost:5000/api/account/profile', {
          headers: { 'Authorization': `Bearer ${token}` }
        });

        if (profileRes.ok) {
          const data = await profileRes.json();
          setProfileData({
            firstName: data.firstName || '',
            lastName: data.lastName || '',
            email: data.email || '',
            username: data.username || '',
            dateOfBirth: data.dateOfBirth ? data.dateOfBirth.split('T')[0] : '',
            gender: data.gender || 'NotSpecified',
            fitnessGoal: data.fitnessGoal || 'NotSpecified',
            roles: data.roles || []
          });
        }

        // Fetch Notification Preferences
        if (userId) {
          try {
            const data = await communicationService.getPreferences(userId);
            setNotifSettings({
              nutritionEnabled: data.nutritionEnabled,
              workoutEnabled: data.workoutEnabled,
              schedules: data.schedules.map((s: any) => ({
                time: s.reminderTime.substring(0, 5),
                label: s.label,
                type: s.type
              }))
            });
          } catch (err) {
            console.error('Failed to load notifications', err);
          }
        }
      } catch (error) {
        console.error('Failed to load profile or settings', error);
      } finally {
        setIsLoading(false);
      }
    };

    fetchProfile();
  }, [navigate]);

  const handleChange = (e: React.ChangeEvent<HTMLInputElement | HTMLSelectElement>) => {
    setProfileData({ ...profileData, [e.target.name]: e.target.value });
  };

  const handleNotifToggle = (field: 'nutritionEnabled' | 'workoutEnabled') => {
    setNotifSettings({ ...notifSettings, [field]: !notifSettings[field] });
  };

  const addSchedule = () => {
    setNotifSettings({
      ...notifSettings,
      schedules: [...notifSettings.schedules, { time: '09:00', label: 'New Reminder', type: 'Nutrition' }]
    });
  };

  const removeSchedule = (index: number) => {
    setNotifSettings({
      ...notifSettings,
      schedules: notifSettings.schedules.filter((_, i) => i !== index)
    });
  };

  const updateSchedule = (index: number, field: string, value: string) => {
    const newSchedules = [...notifSettings.schedules];
    (newSchedules[index] as any)[field] = value;
    setNotifSettings({ ...notifSettings, schedules: newSchedules });
  };

  const handleSave = async (e: React.FormEvent) => {
    e.preventDefault();
    setIsSaving(true);

    const token = localStorage.getItem('accessToken');
    const userId = localStorage.getItem('userId');
    
    try {
      if (activeTab === 'personal' || activeTab === 'fitness') {
        const response = await fetch('http://localhost:5000/api/account/profile', {
          method: 'PUT',
          headers: {
            'Content-Type': 'application/json',
            'Authorization': `Bearer ${token}`
          },
          body: JSON.stringify({
            firstName: profileData.firstName,
            lastName: profileData.lastName,
            dateOfBirth: profileData.dateOfBirth || null,
            gender: profileData.gender,
            fitnessGoal: profileData.fitnessGoal
          })
        });
        if (!response.ok) throw new Error('Failed to update profile');
      }

      if (activeTab === 'notifications' && userId) {
        await communicationService.updatePreferences(userId, {
          nutritionEnabled: notifSettings.nutritionEnabled,
          workoutEnabled: notifSettings.workoutEnabled,
          schedules: notifSettings.schedules
        });
      }

      alert('Changes saved successfully!');
    } catch (error: any) {
      alert(error.message || 'Failed to save changes');
    } finally {
      setIsSaving(false);
    }
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
          <div style={{ width: '48px', height: '48px', borderRadius: '14px', background: 'linear-gradient(135deg, var(--primary), #8b5cf6)', display: 'flex', alignItems: 'center', justifyContent: 'center', boxShadow: '0 8px 16px rgba(59, 130, 246, 0.3)' }}>
            <User size={24} color="white" />
          </div>
          <div>
            <h1 style={{ fontSize: '1.75rem', fontWeight: '800', margin: 0 }}>Особистий кабінет</h1>
            <p style={{ color: 'var(--text-muted)', margin: 0, fontSize: '0.9rem' }}>Керування вашим профілем та налаштуваннями</p>
          </div>
          {profileData.roles?.includes('Admin') && (
            <button onClick={() => navigate('/admin')} className="btn btn-outline" style={{ marginLeft: 'auto', width: 'auto', padding: '0.5rem 1rem', color: '#8b5cf6', borderColor: 'rgba(139, 92, 246, 0.5)' }}>
              <Shield size={16} className="mr-2" /> Панель адміністратора
            </button>
          )}
        </div>
      </header>

      <main style={{ maxWidth: '1000px', margin: '0 auto', display: 'grid', gridTemplateColumns: '1fr 2.5fr', gap: '2rem' }}>
        {/* Sidebar */}
        <aside className="glass-panel" style={{ padding: '1.5rem' }}>
          <div style={{ textAlign: 'center', marginBottom: '2rem' }}>
            <div style={{ position: 'relative', width: '100px', height: '100px', margin: '0 auto 1rem' }}>
              <div style={{ width: '100%', height: '100%', borderRadius: '50%', background: 'linear-gradient(135deg, var(--primary) 0%, #8b5cf6 100%)', display: 'flex', alignItems: 'center', justifyContent: 'center', fontSize: '2.5rem', fontWeight: '700' }}>
                {profileData.firstName[0]}{profileData.lastName[0]}
              </div>
            </div>
            <h2 style={{ fontSize: '1.25rem', fontWeight: '600' }}>{profileData.firstName} {profileData.lastName}</h2>
            <p className="text-muted" style={{ fontSize: '0.875rem' }}>@{profileData.username}</p>
          </div>

          <nav style={{ display: 'flex', flexDirection: 'column', gap: '0.5rem' }}>
            <button 
              onClick={() => setActiveTab('personal')}
              style={{ display: 'flex', alignItems: 'center', gap: '0.75rem', padding: '0.75rem 1rem', borderRadius: '8px', background: activeTab === 'personal' ? 'rgba(59, 130, 246, 0.1)' : 'transparent', color: activeTab === 'personal' ? 'var(--primary)' : 'var(--text-muted)', border: 'none', cursor: 'pointer', textAlign: 'left', width: '100%', fontWeight: activeTab === 'personal' ? '600' : '400' }}
            >
              <User size={18} /> Personal Info
            </button>
            <button 
              onClick={() => setActiveTab('notifications')}
              style={{ display: 'flex', alignItems: 'center', gap: '0.75rem', padding: '0.75rem 1rem', borderRadius: '8px', background: activeTab === 'notifications' ? 'rgba(59, 130, 246, 0.1)' : 'transparent', color: activeTab === 'notifications' ? 'var(--primary)' : 'var(--text-muted)', border: 'none', cursor: 'pointer', textAlign: 'left', width: '100%', fontWeight: activeTab === 'notifications' ? '600' : '400' }}
            >
              <Bell size={18} /> Notifications
            </button>
            <button 
              onClick={() => setActiveTab('fitness')}
              style={{ display: 'flex', alignItems: 'center', gap: '0.75rem', padding: '0.75rem 1rem', borderRadius: '8px', background: activeTab === 'fitness' ? 'rgba(59, 130, 246, 0.1)' : 'transparent', color: activeTab === 'fitness' ? 'var(--primary)' : 'var(--text-muted)', border: 'none', cursor: 'pointer', textAlign: 'left', width: '100%', fontWeight: activeTab === 'fitness' ? '600' : '400' }}
            >
              <Activity size={18} /> Fitness Goals
            </button>
          </nav>
        </aside>

        {/* Main Content */}
        <div className="glass-panel" style={{ padding: '2rem' }}>
          {activeTab === 'personal' && (
            <>
              <h2 style={{ fontSize: '1.5rem', fontWeight: '600', margin: '0 0 1.5rem' }}>Personal Information</h2>
              <form onSubmit={handleSave}>
                <div style={{ display: 'grid', gridTemplateColumns: '1fr 1fr', gap: '1.5rem', marginBottom: '1.5rem' }}>
                  <div className="form-group">
                    <label className="form-label">First Name</label>
                    <input type="text" name="firstName" className="form-input" value={profileData.firstName} onChange={handleChange} required />
                  </div>
                  <div className="form-group">
                    <label className="form-label">Last Name</label>
                    <input type="text" name="lastName" className="form-input" value={profileData.lastName} onChange={handleChange} required />
                  </div>
                </div>
                <div className="form-group" style={{ marginBottom: '1.5rem' }}>
                  <label className="form-label">Date of Birth</label>
                  <input type="date" name="dateOfBirth" className="form-input" value={profileData.dateOfBirth} onChange={handleChange} />
                </div>
                <div style={{ display: 'flex', justifyContent: 'flex-end' }}>
                  <button type="submit" className="btn btn-primary" style={{ width: 'auto' }} disabled={isSaving}>
                    {isSaving ? <Loader2 className="animate-spin" size={20} /> : <><Save size={18} style={{ marginRight: '0.5rem' }} /> Save Changes</>}
                  </button>
                </div>
              </form>
            </>
          )}

          {activeTab === 'notifications' && (
            <>
              <div style={{ display: 'flex', justifyContent: 'space-between', alignItems: 'center', marginBottom: '1.5rem' }}>
                <h2 style={{ fontSize: '1.5rem', fontWeight: '600', margin: 0 }}>Notification Preferences</h2>
                <button 
                  type="button"
                  className="btn btn-outline" 
                  style={{ width: 'auto', padding: '0.5rem 1rem' }}
                  onClick={async () => {
                    const token = localStorage.getItem('accessToken');
                    await fetch('http://localhost:5003/api/Notification/test', {
                      method: 'POST',
                      headers: { 'Authorization': `Bearer ${token}` }
                    });
                  }}
                >
                  <Bell size={18} style={{ marginRight: '0.5rem' }} />
                  Test Live Notification
                </button>
              </div>
              <div style={{ marginBottom: '2rem', display: 'flex', flexDirection: 'column', gap: '1rem' }}>
                <div style={{ display: 'flex', alignItems: 'center', justifyContent: 'space-between', padding: '1rem', background: 'rgba(255,255,255,0.03)', borderRadius: '12px' }}>
                  <div>
                    <div style={{ fontWeight: '600' }}>Nutrition Reminders</div>
                    <div style={{ fontSize: '0.8rem', color: 'var(--text-muted)' }}>Get notified about meals and hydration</div>
                  </div>
                  <input type="checkbox" checked={notifSettings.nutritionEnabled} onChange={() => handleNotifToggle('nutritionEnabled')} style={{ width: '20px', height: '20px' }} />
                </div>
                <div style={{ display: 'flex', alignItems: 'center', justifyContent: 'space-between', padding: '1rem', background: 'rgba(255,255,255,0.03)', borderRadius: '12px' }}>
                  <div>
                    <div style={{ fontWeight: '600' }}>Workout Reminders</div>
                    <div style={{ fontSize: '0.8rem', color: 'var(--text-muted)' }}>Get notified about scheduled training sessions</div>
                  </div>
                  <input type="checkbox" checked={notifSettings.workoutEnabled} onChange={() => handleNotifToggle('workoutEnabled')} style={{ width: '20px', height: '20px' }} />
                </div>
              </div>

              <div style={{ display: 'flex', justifyContent: 'space-between', alignItems: 'center', marginBottom: '1rem' }}>
                <h3 style={{ fontSize: '1.1rem', fontWeight: '600' }}>Reminder Schedule</h3>
                <button onClick={addSchedule} className="btn btn-outline" style={{ width: 'auto', padding: '0.4rem 0.8rem', fontSize: '0.8rem' }}>
                  <Plus size={16} className="mr-1" /> Add New
                </button>
              </div>

              <div style={{ display: 'flex', flexDirection: 'column', gap: '0.75rem', marginBottom: '2rem' }}>
                {notifSettings.schedules.map((s, index) => (
                  <div key={index} style={{ display: 'grid', gridTemplateColumns: '100px 1fr 120px 40px', gap: '1rem', alignItems: 'center', padding: '0.75rem', background: 'rgba(255,255,255,0.02)', borderRadius: '10px' }}>
                    <input type="time" className="form-input" value={s.time} onChange={(e) => updateSchedule(index, 'time', e.target.value)} style={{ padding: '0.4rem' }} />
                    <input type="text" className="form-input" placeholder="Label" value={s.label} onChange={(e) => updateSchedule(index, 'label', e.target.value)} style={{ padding: '0.4rem' }} />
                    <select className="form-input" value={s.type} onChange={(e) => updateSchedule(index, 'type', e.target.value)} style={{ padding: '0.4rem' }}>
                      <option value="Nutrition">Nutrition</option>
                      <option value="Workout">Workout</option>
                    </select>
                    <button onClick={() => removeSchedule(index)} style={{ background: 'none', border: 'none', color: '#ef4444', cursor: 'pointer' }}>
                      <Trash2 size={18} />
                    </button>
                  </div>
                ))}
                {notifSettings.schedules.length === 0 && <p style={{ textAlign: 'center', color: 'var(--text-muted)', fontSize: '0.9rem', padding: '1rem' }}>No reminders scheduled yet.</p>}
              </div>

              <div style={{ display: 'flex', justifyContent: 'flex-end' }}>
                <button onClick={handleSave} className="btn btn-primary" style={{ width: 'auto' }} disabled={isSaving}>
                  {isSaving ? <Loader2 className="animate-spin" size={20} /> : <><Save size={18} style={{ marginRight: '0.5rem' }} /> Save Notifications</>}
                </button>
              </div>
            </>
          )}

          {activeTab === 'fitness' && (
            <>
              <h2 style={{ fontSize: '1.5rem', fontWeight: '600', margin: '0 0 1.5rem' }}>Fitness Goals</h2>
              <form onSubmit={handleSave}>
                <div className="form-group" style={{ marginBottom: '2rem' }}>
                  <label className="form-label">Current Goal</label>
                  <select name="fitnessGoal" className="form-input" value={profileData.fitnessGoal} onChange={handleChange}>
                    <option value="Fitness">General Fitness</option>
                    <option value="Bodybuilding">Bodybuilding</option>
                    <option value="Powerlifting">Powerlifting</option>
                    <option value="WeightLoss">Weight Loss</option>
                    <option value="Endurance">Endurance</option>
                    <option value="Rehabilitation">Rehabilitation</option>
                  </select>
                </div>
                <div style={{ display: 'flex', justifyContent: 'flex-end' }}>
                  <button type="submit" className="btn btn-primary" style={{ width: 'auto' }} disabled={isSaving}>
                    {isSaving ? <Loader2 className="animate-spin" size={20} /> : <><Save size={18} style={{ marginRight: '0.5rem' }} /> Save Goals</>}
                  </button>
                </div>
              </form>
            </>
          )}
        </div>
      </main>
    </div>
  );
};

export default Profile;
