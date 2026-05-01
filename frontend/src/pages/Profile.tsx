import { useState, useEffect } from 'react';
import { useNavigate } from 'react-router-dom';
import { User, Settings, LogOut, Dumbbell, Activity, Save, Loader2, Camera, Shield } from 'lucide-react';

const Profile = () => {
  const navigate = useNavigate();
  const [isLoading, setIsLoading] = useState(false);
  const [isSaving, setIsSaving] = useState(false);

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

  useEffect(() => {
    const fetchProfile = async () => {
      setIsLoading(true);
      const token = localStorage.getItem('accessToken');
      if (!token) {
        navigate('/login');
        return;
      }

      try {
        const response = await fetch('http://localhost:5000/api/account/profile', {
          headers: {
            'Authorization': `Bearer ${token}`
          }
        });

        if (response.ok) {
          const data = await response.json();
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
        } else if (response.status === 401) {
          localStorage.removeItem('accessToken');
          navigate('/login');
        }
      } catch (error) {
        console.error('Failed to load profile', error);
      } finally {
        setIsLoading(false);
      }
    };

    fetchProfile();
  }, [navigate]);

  const handleChange = (e: React.ChangeEvent<HTMLInputElement | HTMLSelectElement>) => {
    setProfileData({ ...profileData, [e.target.name]: e.target.value });
  };

  const handleSave = async (e: React.FormEvent) => {
    e.preventDefault();
    setIsSaving(true);

    const token = localStorage.getItem('accessToken');
    try {
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

      if (!response.ok) {
        const errorData = await response.json().catch(() => null);
        throw new Error(errorData?.message || errorData?.title || 'Failed to update profile');
      }

      alert('Profile updated successfully!');
    } catch (error: any) {
      console.error('Failed to update profile', error);
      alert(error.message || 'Failed to update profile');
    } finally {
      setIsSaving(false);
    }
  };

  const handleLogout = () => {
    localStorage.removeItem('accessToken');
    localStorage.removeItem('userId');
    navigate('/login');
  };

  if (isLoading) {
    return (
      <div style={{ display: 'flex', justifyContent: 'center', alignItems: 'center', height: '100vh', background: 'linear-gradient(135deg, var(--bg-darker) 0%, var(--bg-dark) 100%)' }}>
        <Loader2 className="animate-spin" size={48} color="var(--primary)" />
      </div>
    );
  }

  return (
    <div style={{ minHeight: '100vh', padding: '2rem' }}>
      <header style={{ maxWidth: '1000px', margin: '0 auto 2rem', display: 'flex', justifyContent: 'space-between', alignItems: 'center' }}>
        <div style={{ display: 'flex', alignItems: 'center', gap: '0.75rem' }}>
          <div style={{ width: '40px', height: '40px', borderRadius: '12px', background: 'rgba(30, 41, 59, 0.7)', border: '1px solid var(--glass-border)', display: 'flex', alignItems: 'center', justifyContent: 'center' }}>
            <Dumbbell size={20} color="var(--primary)" />
          </div>
          <h1 style={{ fontSize: '1.5rem', fontWeight: '700' }}>FitnessApp</h1>
        </div>

        <div style={{ display: 'flex', gap: '1rem' }}>
          {profileData.roles?.includes('Admin') && (
            <button onClick={() => navigate('/admin')} className="btn btn-outline" style={{ padding: '0.5rem 1rem', width: 'auto', color: '#8b5cf6', borderColor: 'rgba(139, 92, 246, 0.5)' }}>
              <Shield size={16} style={{ marginRight: '0.5rem' }} /> Admin Panel
            </button>
          )}
          <button onClick={() => navigate('/workout')} className="btn btn-primary" style={{ padding: '0.5rem 1rem', width: 'auto' }}>
            <Activity size={16} style={{ marginRight: '0.5rem' }} /> My Workout
          </button>
          <button onClick={() => navigate('/nutrition')} className="btn btn-primary" style={{ padding: '0.5rem 1rem', width: 'auto', background: '#10b981', boxShadow: '0 4px 14px rgba(16,185,129,0.4)' }}>
            🥗 Харчування
          </button>
          <button onClick={handleLogout} className="btn btn-outline" style={{ padding: '0.5rem 1rem', width: 'auto' }}>
            <LogOut size={16} style={{ marginRight: '0.5rem' }} /> Logout
          </button>
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
              <button style={{ position: 'absolute', bottom: '0', right: '0', width: '32px', height: '32px', borderRadius: '50%', background: 'var(--bg-dark)', border: '1px solid var(--glass-border)', color: 'white', display: 'flex', alignItems: 'center', justifyContent: 'center', cursor: 'pointer' }}>
                <Camera size={14} />
              </button>
            </div>
            <h2 style={{ fontSize: '1.25rem', fontWeight: '600' }}>{profileData.firstName} {profileData.lastName}</h2>
            <p className="text-muted" style={{ fontSize: '0.875rem' }}>@{profileData.username}</p>
          </div>

          <nav style={{ display: 'flex', flexDirection: 'column', gap: '0.5rem' }}>
            <a href="#" style={{ display: 'flex', alignItems: 'center', gap: '0.75rem', padding: '0.75rem 1rem', borderRadius: '8px', background: 'rgba(59, 130, 246, 0.1)', color: 'var(--primary)', fontWeight: '500' }}>
              <User size={18} /> Personal Info
            </a>
            <a href="#" style={{ display: 'flex', alignItems: 'center', gap: '0.75rem', padding: '0.75rem 1rem', borderRadius: '8px', color: 'var(--text-muted)' }}>
              <Activity size={18} /> Fitness Goals
            </a>
            <a href="#" style={{ display: 'flex', alignItems: 'center', gap: '0.75rem', padding: '0.75rem 1rem', borderRadius: '8px', color: 'var(--text-muted)' }}>
              <Settings size={18} /> Account Settings
            </a>
          </nav>
        </aside>

        {/* Main Content */}
        <div className="glass-panel" style={{ padding: '2rem' }}>
          <h2 style={{ fontSize: '1.5rem', fontWeight: '600', margin: '0 0 1.5rem' }}>Personal Information</h2>

          <form onSubmit={handleSave}>
            <div style={{ display: 'grid', gridTemplateColumns: '1fr 1fr', gap: '1.5rem', marginBottom: '1.5rem' }}>
              <div className="form-group" style={{ marginBottom: 0 }}>
                <label className="form-label">First Name</label>
                <input type="text" name="firstName" className="form-input" value={profileData.firstName} onChange={handleChange} required />
              </div>
              <div className="form-group" style={{ marginBottom: 0 }}>
                <label className="form-label">Last Name</label>
                <input type="text" name="lastName" className="form-input" value={profileData.lastName} onChange={handleChange} required />
              </div>
            </div>

            <div style={{ display: 'grid', gridTemplateColumns: '1fr 1fr', gap: '1.5rem', marginBottom: '1.5rem' }}>
              <div className="form-group" style={{ marginBottom: 0 }}>
                <label className="form-label">Email</label>
                <input type="email" className="form-input" value={profileData.email} disabled style={{ opacity: 0.7, cursor: 'not-allowed' }} />
              </div>
              <div className="form-group" style={{ marginBottom: 0 }}>
                <label className="form-label">Username</label>
                <input type="text" className="form-input" value={profileData.username} disabled style={{ opacity: 0.7, cursor: 'not-allowed' }} />
              </div>
            </div>

            <div style={{ display: 'grid', gridTemplateColumns: '1fr 1fr', gap: '1.5rem', marginBottom: '2rem' }}>
              <div className="form-group" style={{ marginBottom: 0 }}>
                <label className="form-label">Date of Birth</label>
                <input type="date" name="dateOfBirth" className="form-input" value={profileData.dateOfBirth} onChange={handleChange} />
              </div>
              <div className="form-group" style={{ marginBottom: 0 }}>
                <label className="form-label">Fitness Goal</label>
                <select name="fitnessGoal" className="form-input" value={profileData.fitnessGoal} onChange={handleChange}>
                  <option value="Fitness">General Fitness</option>
                  <option value="Bodybuilding">Bodybuilding</option>
                  <option value="Powerlifting">Powerlifting</option>
                  <option value="WeightLoss">Weight Loss</option>
                  <option value="Endurance">Endurance</option>
                  <option value="Rehabilitation">Rehabilitation</option>
                </select>
              </div>
            </div>

            <div style={{ display: 'flex', justifyContent: 'flex-end', paddingTop: '1rem', borderTop: '1px solid var(--glass-border)' }}>
              <button type="submit" className="btn btn-primary" style={{ width: 'auto' }} disabled={isSaving}>
                {isSaving ? <Loader2 className="animate-spin" size={20} /> : (
                  <>
                    <Save size={18} style={{ marginRight: '0.5rem' }} /> Save Changes
                  </>
                )}
              </button>
            </div>
          </form>
        </div>
      </main>
    </div>
  );
};

export default Profile;
