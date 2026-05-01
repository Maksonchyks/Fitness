import { useState } from 'react';
import { Link, useNavigate } from 'react-router-dom';
import { Lock, CheckCircle, Loader2 } from 'lucide-react';

const ResetPassword = () => {
  const navigate = useNavigate();
  const [isLoading, setIsLoading] = useState(false);
  const [isSuccess, setIsSuccess] = useState(false);
  const [formData, setFormData] = useState({
    password: '',
    confirmPassword: ''
  });

  const handleChange = (e: React.ChangeEvent<HTMLInputElement>) => {
    setFormData({ ...formData, [e.target.name]: e.target.value });
  };

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();
    if (formData.password !== formData.confirmPassword) {
      alert("Passwords don't match");
      return;
    }

    setIsLoading(true);
    
    // Simulate API call to /api/v1/auth/reset-password
    try {
      await new Promise(resolve => setTimeout(resolve, 1500));
      setIsSuccess(true);
    } catch (error) {
      console.error('Failed to reset password:', error);
    } finally {
      setIsLoading(false);
    }
  };

  return (
    <div className="glass-panel">
      <div className="text-center mb-6">
        <h2 style={{ fontSize: '1.75rem', fontWeight: '600', marginBottom: '0.5rem' }}>Set New Password</h2>
        <p className="text-muted">Please enter your new password below</p>
      </div>

      {isSuccess ? (
        <div className="text-center animate-fade-in">
          <div style={{ width: '64px', height: '64px', borderRadius: '50%', background: 'rgba(34, 197, 94, 0.2)', display: 'flex', alignItems: 'center', justifyContent: 'center', margin: '0 auto 1.5rem' }}>
            <CheckCircle size={32} color="var(--success)" />
          </div>
          <h3 style={{ fontSize: '1.25rem', marginBottom: '0.5rem' }}>Password Updated</h3>
          <p className="text-muted mb-6">Your password has been changed successfully.</p>
          <button onClick={() => navigate('/login')} className="btn btn-primary">
            Continue to Login
          </button>
        </div>
      ) : (
        <form onSubmit={handleSubmit} className="animate-fade-in">
          <div className="form-group">
            <label className="form-label">New Password</label>
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

          <div className="form-group">
            <label className="form-label">Confirm New Password</label>
            <div style={{ position: 'relative' }}>
              <div style={{ position: 'absolute', top: '50%', transform: 'translateY(-50%)', left: '1rem', color: 'var(--text-muted)' }}>
                <Lock size={20} />
              </div>
              <input 
                type="password" 
                name="confirmPassword"
                className="form-input" 
                placeholder="••••••••"
                style={{ paddingLeft: '3rem' }}
                value={formData.confirmPassword}
                onChange={handleChange}
                required
              />
            </div>
          </div>

          <button type="submit" className="btn btn-primary mt-4" disabled={isLoading}>
            {isLoading ? <Loader2 className="animate-spin" size={20} /> : 'Reset Password'}
          </button>
        </form>
      )}
    </div>
  );
};

export default ResetPassword;
