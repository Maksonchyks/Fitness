import { useState } from 'react';
import { Link, useNavigate } from 'react-router-dom';
import { Mail, Lock, LogIn, Loader2 } from 'lucide-react';

const Login = () => {
  const navigate = useNavigate();
  const [isLoading, setIsLoading] = useState(false);
  const [formData, setFormData] = useState({
    email: '',
    password: ''
  });

  const handleChange = (e: React.ChangeEvent<HTMLInputElement>) => {
    setFormData({ ...formData, [e.target.name]: e.target.value });
  };

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();
    setIsLoading(true);

    try {
      const response = await fetch('http://localhost:5000/api/auth/login', {
        method: 'POST',
        headers: { 'Content-Type': 'application/json' },
        body: JSON.stringify(formData)
      });

      if (!response.ok) {
        const err = await response.json();
        throw new Error(err.message || 'Login failed');
      }

      const data = await response.json();
      localStorage.setItem('accessToken', data.accessToken);
      localStorage.setItem('userId', data.userId);

      navigate('/profile');
    } catch (error) {
      console.error('Login failed:', error);
      alert((error as Error).message);
    } finally {
      setIsLoading(false);
    }
  };

  return (
    <div className="glass-panel">
      <div className="text-center mb-6">
        <h2 style={{ fontSize: '1.75rem', fontWeight: '600', marginBottom: '0.5rem' }}>Welcome Back</h2>
        <p className="text-muted">Sign in to continue your journey</p>
      </div>

      <form onSubmit={handleSubmit}>
        <div className="form-group">
          <label className="form-label">Email Address</label>
          <div style={{ position: 'relative' }}>
            <div style={{ position: 'absolute', top: '50%', transform: 'translateY(-50%)', left: '1rem', color: 'var(--text-muted)' }}>
              <Mail size={20} />
            </div>
            <input
              type="email"
              name="email"
              className="form-input"
              placeholder="you@example.com"
              style={{ paddingLeft: '3rem' }}
              value={formData.email}
              onChange={handleChange}
              required
            />
          </div>
        </div>

        <div className="form-group">
          <div className="flex justify-between items-center mb-2">
            <label className="form-label" style={{ marginBottom: 0 }}>Password</label>
            <Link to="/forgot-password" style={{ fontSize: '0.875rem' }}>Forgot password?</Link>
          </div>
          <div style={{ position: 'relative' }}>
            <div style={{ position: 'absolute', top: '50%', transform: 'translateY(-50%)', left: '1rem', color: 'var(--text-muted)' }}>
              <Lock size={20} />
            </div>
            <input
              type="password"
              name="password"
              className="form-input"
              placeholder="••••••••"
              style={{ paddingLeft: '3rem' }}
              value={formData.password}
              onChange={handleChange}
              required
            />
          </div>
        </div>

        <button type="submit" className="btn btn-primary mt-4" disabled={isLoading}>
          {isLoading ? <Loader2 className="animate-spin" size={20} /> : (
            <>
              <LogIn size={20} style={{ marginRight: '0.5rem' }} />
              Sign In
            </>
          )}
        </button>
      </form>

      <div className="text-center mt-6 text-sm">
        <span className="text-muted">Don't have an account? </span>
        <Link to="/register" style={{ fontWeight: '600' }}>Sign up</Link>
      </div>
    </div>
  );
};

export default Login;
