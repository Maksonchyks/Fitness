import { useState } from 'react';
import { Link } from 'react-router-dom';
import { Mail, ArrowLeft, Send, Loader2 } from 'lucide-react';

const ForgotPassword = () => {
  const [isLoading, setIsLoading] = useState(false);
  const [isSent, setIsSent] = useState(false);
  const [email, setEmail] = useState('');

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();
    setIsLoading(true);
    
    // Simulate API call to /api/v1/auth/forgot-password
    try {
      await new Promise(resolve => setTimeout(resolve, 1500));
      setIsSent(true);
    } catch (error) {
      console.error('Failed to send reset link:', error);
    } finally {
      setIsLoading(false);
    }
  };

  return (
    <div className="glass-panel">
      <Link to="/login" className="flex items-center text-sm mb-6 text-muted hover:text-white" style={{ display: 'inline-flex', textDecoration: 'none' }}>
        <ArrowLeft size={16} style={{ marginRight: '0.25rem' }} /> Back to login
      </Link>

      <div className="text-center mb-6">
        <h2 style={{ fontSize: '1.75rem', fontWeight: '600', marginBottom: '0.5rem' }}>Reset Password</h2>
        <p className="text-muted">Enter your email and we'll send you instructions</p>
      </div>

      {isSent ? (
        <div className="text-center animate-fade-in">
          <div style={{ width: '64px', height: '64px', borderRadius: '50%', background: 'rgba(34, 197, 94, 0.2)', display: 'flex', alignItems: 'center', justifyContent: 'center', margin: '0 auto 1.5rem' }}>
            <Mail size={32} color="var(--success)" />
          </div>
          <h3 style={{ fontSize: '1.25rem', marginBottom: '0.5rem' }}>Check your email</h3>
          <p className="text-muted mb-6">We've sent a password reset link to <br/><strong>{email}</strong></p>
          <button onClick={() => setIsSent(false)} className="btn btn-outline text-sm">
            Try another email
          </button>
        </div>
      ) : (
        <form onSubmit={handleSubmit} className="animate-fade-in">
          <div className="form-group">
            <label className="form-label">Email Address</label>
            <div style={{ position: 'relative' }}>
              <div style={{ position: 'absolute', top: '50%', transform: 'translateY(-50%)', left: '1rem', color: 'var(--text-muted)' }}>
                <Mail size={20} />
              </div>
              <input 
                type="email" 
                className="form-input" 
                placeholder="you@example.com"
                style={{ paddingLeft: '3rem' }}
                value={email}
                onChange={(e) => setEmail(e.target.value)}
                required
              />
            </div>
          </div>

          <button type="submit" className="btn btn-primary mt-4" disabled={isLoading}>
            {isLoading ? <Loader2 className="animate-spin" size={20} /> : (
              <>
                <Send size={20} style={{ marginRight: '0.5rem' }} />
                Send Reset Link
              </>
            )}
          </button>
        </form>
      )}
    </div>
  );
};

export default ForgotPassword;
