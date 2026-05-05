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
    // Показуємо спінер тільки при першому завантаженні
    if (users.length === 0) setIsLoading(true);
    
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

      // Миттєво оновлюємо статус локально
      const newStatus = action === 'activate' ? 'Active' : 'Inactive';
      setUsers(prev => prev.map(u => u.id === userId ? { ...u, status: newStatus } : u));
      
      // Оновлюємо список фоново для синхронізації
      fetchUsers();
    } catch (error: any) {
      alert(error.message);
    } finally {
      setActionLoading(null);
    }
  };

  const handleRoleChange = async (userId: string, targetRole: string) => {
    if (!confirm(`Ви впевнені, що хочете змінити роль на ${targetRole}?`)) return;

    setActionLoading(userId);
    const token = localStorage.getItem('accessToken');
    
    try {
      const response = await fetch(`http://localhost:5000/api/users/${userId}/roles`, {
        method: 'POST',
        headers: {
          'Content-Type': 'application/json',
          'Authorization': `Bearer ${token}`
        },
        body: JSON.stringify({ roleName: targetRole })
      });

      if (!response.ok) {
        const err = await response.json().catch(() => null);
        throw new Error(err?.message || 'Failed to change role');
      }

      // Миттєво оновлюємо роль локально
      setUsers(prev => prev.map(u => u.id === userId ? { ...u, roles: [targetRole] } : u));

      // Оновлюємо список фоново для синхронізації
      fetchUsers();
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
    <div style={{ padding: '2rem 1rem' }}>
      <header style={{ maxWidth: '1200px', margin: '0 auto 2.5rem' }}>
        <div style={{ display: 'flex', alignItems: 'center', gap: '1rem' }}>
          <div style={{ width: '48px', height: '48px', borderRadius: '14px', background: 'rgba(30, 41, 59, 0.7)', border: '1px solid var(--glass-border)', display: 'flex', alignItems: 'center', justifyContent: 'center' }}>
            <Shield size={24} color="var(--primary)" />
          </div>
          <div>
            <h1 style={{ fontSize: '1.75rem', fontWeight: '800', margin: 0 }}>Панель адміністратора</h1>
            <p style={{ color: 'var(--text-muted)', margin: 0, fontSize: '0.9rem' }}>Керування користувачами та правами доступу</p>
          </div>
        </div>
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
                        background: user.roles?.includes('Admin') ? 'rgba(139, 92, 246, 0.1)' : user.roles?.includes('Trainer') ? 'rgba(16, 185, 129, 0.1)' : 'rgba(59, 130, 246, 0.1)',
                        color: user.roles?.includes('Admin') ? '#8b5cf6' : user.roles?.includes('Trainer') ? '#10b981' : 'var(--primary)',
                      }}>
                        {user.roles?.[0] || 'User'}
                      </span>
                    </td>
                    <td style={{ padding: '1rem', textAlign: 'right' }}>
                      <div style={{ display: 'flex', gap: '0.5rem', justifyContent: 'flex-end' }}>
                        <div style={{ display: 'flex', background: 'rgba(30, 41, 59, 0.5)', padding: '0.25rem', borderRadius: '10px', border: '1px solid var(--glass-border)' }}>
                            <button 
                            onClick={() => handleRoleChange(user.id, 'User')}
                            disabled={actionLoading === user.id}
                            className={`btn-role ${user.roles?.includes('User') ? 'active' : ''}`}
                            title="Set as User"
                            >
                            User
                            </button>
                            <button 
                            onClick={() => handleRoleChange(user.id, 'Trainer')}
                            disabled={actionLoading === user.id}
                            className={`btn-role ${user.roles?.includes('Trainer') ? 'active' : ''}`}
                            title="Set as Coach"
                            >
                            Coach
                            </button>
                            <button 
                            onClick={() => handleRoleChange(user.id, 'Admin')}
                            disabled={actionLoading === user.id}
                            className={`btn-role ${user.roles?.includes('Admin') ? 'active' : ''}`}
                            title="Set as Admin"
                            >
                            Admin
                            </button>
                        </div>
                        
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
