import { useState, useEffect } from 'react';
import { useNavigate } from 'react-router-dom';
import { Users, Shield, ShieldOff, CheckCircle, XCircle, Loader2, UserCog } from 'lucide-react';

interface User {
  id: string;
  username: string;
  email: string;
  firstName: string | null;
  lastName: string | null;
  status: string;
  roles: string[];
  createdAt: string;
}

const AdminDashboard = () => {
  const navigate = useNavigate();
  const [users, setUsers] = useState<User[]>([]);
  const [isLoading, setIsLoading] = useState(true);
  const [actionLoading, setActionLoading] = useState<string | null>(null);

  const fetchUsers = async () => {
    setIsLoading(true);
    const token = localStorage.getItem('accessToken');
    if (!token) {
      navigate('/login');
      return;
    }

    try {
      const response = await fetch('http://localhost:5000/api/users', {
        headers: {
          'Authorization': `Bearer ${token}`
        }
      });

      if (response.ok) {
        const data = await response.json();
        setUsers(data);
      } else if (response.status === 401 || response.status === 403) {
        alert('You do not have permission to view this page.');
        navigate('/profile');
      }
    } catch (error) {
      console.error('Failed to load users', error);
    } finally {
      setIsLoading(false);
    }
  };

  useEffect(() => {
    fetchUsers();
  }, [navigate]);

  const handleStatusChange = async (userId: string, currentStatus: string) => {
    const action = currentStatus === 'Active' ? 'deactivate' : 'activate';
    if (!confirm(`Are you sure you want to ${action} this user?`)) return;

    setActionLoading(userId);
    const token = localStorage.getItem('accessToken');
    
    try {
      const response = await fetch(`http://localhost:5000/api/users/${userId}/${action}`, {
        method: 'POST',
        headers: {
          'Authorization': `Bearer ${token}`
        }
      });

      if (!response.ok) {
        const err = await response.json().catch(() => null);
        throw new Error(err?.message || `Failed to ${action} user`);
      }

      // Refresh users list
      await fetchUsers();
    } catch (error: any) {
      alert(error.message);
    } finally {
      setActionLoading(null);
    }
  };

  const handleRoleChange = async (userId: string, currentRole: string) => {
    const newRole = currentRole === 'Admin' ? 'User' : 'Admin';
    if (!confirm(`Are you sure you want to change role to ${newRole}?`)) return;

    setActionLoading(userId);
    const token = localStorage.getItem('accessToken');
    
    try {
      const response = await fetch(`http://localhost:5000/api/users/${userId}/roles`, {
        method: 'POST',
        headers: {
          'Content-Type': 'application/json',
          'Authorization': `Bearer ${token}`
        },
        body: JSON.stringify({ roleName: newRole })
      });

      if (!response.ok) {
        const err = await response.json().catch(() => null);
        throw new Error(err?.message || 'Failed to change role');
      }

      await fetchUsers();
    } catch (error: any) {
      alert(error.message);
    } finally {
      setActionLoading(null);
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
    <div style={{ minHeight: '100vh', padding: '2rem' }}>
      <header style={{ maxWidth: '1200px', margin: '0 auto 2rem', display: 'flex', justifyContent: 'space-between', alignItems: 'center' }}>
        <div style={{ display: 'flex', alignItems: 'center', gap: '0.75rem' }}>
          <div style={{ width: '40px', height: '40px', borderRadius: '12px', background: 'rgba(30, 41, 59, 0.7)', border: '1px solid var(--glass-border)', display: 'flex', alignItems: 'center', justifyContent: 'center' }}>
            <Shield size={20} color="var(--primary)" />
          </div>
          <h1 style={{ fontSize: '1.5rem', fontWeight: '700' }}>Admin Dashboard</h1>
        </div>
        
        <button onClick={() => navigate('/profile')} className="btn btn-outline" style={{ padding: '0.5rem 1rem', width: 'auto' }}>
          Back to Profile
        </button>
      </header>

      <main style={{ maxWidth: '1200px', margin: '0 auto' }}>
        <div className="glass-panel" style={{ padding: '2rem', overflowX: 'auto' }}>
          <div style={{ display: 'flex', alignItems: 'center', gap: '0.5rem', marginBottom: '1.5rem' }}>
            <Users size={20} color="var(--primary)" />
            <h2 style={{ fontSize: '1.25rem', fontWeight: '600', margin: 0 }}>Manage Users</h2>
          </div>

          <table style={{ width: '100%', borderCollapse: 'collapse', textAlign: 'left' }}>
            <thead>
              <tr style={{ borderBottom: '1px solid var(--glass-border)', color: 'var(--text-muted)' }}>
                <th style={{ padding: '1rem', fontWeight: '500' }}>User</th>
                <th style={{ padding: '1rem', fontWeight: '500' }}>Email</th>
                <th style={{ padding: '1rem', fontWeight: '500' }}>Status</th>
                <th style={{ padding: '1rem', fontWeight: '500' }}>Role</th>
                <th style={{ padding: '1rem', fontWeight: '500', textAlign: 'right' }}>Actions</th>
              </tr>
            </thead>
            <tbody>
              {users.map(user => {
                const isAdmin = user.roles.includes('Admin');
                const isActive = user.status === 'Active';
                
                return (
                  <tr key={user.id} style={{ borderBottom: '1px solid rgba(255,255,255,0.05)' }}>
                    <td style={{ padding: '1rem' }}>
                      <div style={{ fontWeight: '600' }}>{user.username}</div>
                      <div style={{ fontSize: '0.85rem', color: 'var(--text-muted)' }}>
                        {user.firstName || user.lastName ? `${user.firstName || ''} ${user.lastName || ''}` : 'No Name'}
                      </div>
                    </td>
                    <td style={{ padding: '1rem', color: 'var(--text-muted)' }}>{user.email}</td>
                    <td style={{ padding: '1rem' }}>
                      <span style={{ 
                        padding: '0.25rem 0.75rem', 
                        borderRadius: '999px', 
                        fontSize: '0.85rem',
                        fontWeight: '500',
                        background: isActive ? 'rgba(16, 185, 129, 0.1)' : 'rgba(239, 68, 68, 0.1)',
                        color: isActive ? '#10b981' : '#ef4444',
                        display: 'inline-flex',
                        alignItems: 'center',
                        gap: '0.25rem'
                      }}>
                        {isActive ? <CheckCircle size={14} /> : <XCircle size={14} />}
                        {user.status}
                      </span>
                    </td>
                    <td style={{ padding: '1rem' }}>
                      <span style={{ 
                        padding: '0.25rem 0.75rem', 
                        borderRadius: '999px', 
                        fontSize: '0.85rem',
                        fontWeight: '500',
                        background: isAdmin ? 'rgba(139, 92, 246, 0.1)' : 'rgba(59, 130, 246, 0.1)',
                        color: isAdmin ? '#8b5cf6' : 'var(--primary)',
                      }}>
                        {isAdmin ? 'Admin' : 'User'}
                      </span>
                    </td>
                    <td style={{ padding: '1rem', textAlign: 'right' }}>
                      <div style={{ display: 'flex', gap: '0.5rem', justifyContent: 'flex-end' }}>
                        <button 
                          onClick={() => handleRoleChange(user.id, isAdmin ? 'Admin' : 'User')}
                          disabled={actionLoading === user.id}
                          className="btn btn-outline"
                          style={{ 
                            padding: '0.4rem 0.75rem', 
                            width: 'auto',
                            fontSize: '0.85rem',
                            display: 'flex',
                            gap: '0.4rem'
                          }}
                        >
                          <UserCog size={14} />
                          {isAdmin ? 'Make User' : 'Make Admin'}
                        </button>
                        
                        <button 
                          onClick={() => handleStatusChange(user.id, user.status)}
                          disabled={actionLoading === user.id}
                          style={{ 
                            padding: '0.4rem 0.75rem', 
                            borderRadius: '8px',
                            border: 'none',
                            background: isActive ? 'rgba(239, 68, 68, 0.1)' : 'rgba(16, 185, 129, 0.1)',
                            color: isActive ? '#ef4444' : '#10b981',
                            cursor: actionLoading === user.id ? 'not-allowed' : 'pointer',
                            fontSize: '0.85rem',
                            fontWeight: '500',
                            display: 'flex',
                            alignItems: 'center',
                            gap: '0.4rem'
                          }}
                        >
                          {actionLoading === user.id ? (
                            <Loader2 size={14} className="animate-spin" />
                          ) : isActive ? (
                            <><ShieldOff size={14} /> Deactivate</>
                          ) : (
                            <><Shield size={14} /> Activate</>
                          )}
                        </button>
                      </div>
                    </td>
                  </tr>
                );
              })}
            </tbody>
          </table>
          
          {users.length === 0 && (
            <div style={{ textAlign: 'center', padding: '3rem', color: 'var(--text-muted)' }}>
              No users found.
            </div>
          )}
        </div>
      </main>
    </div>
  );
};

export default AdminDashboard;
