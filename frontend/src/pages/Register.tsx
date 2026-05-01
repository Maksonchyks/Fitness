import { useState } from 'react';
import { Link, useNavigate } from 'react-router-dom';
import { Mail, Lock, User, UserPlus, Loader2, Calendar } from 'lucide-react';

const Register = () => {
  const navigate = useNavigate();
  const [isLoading, setIsLoading] = useState(false);
  const [formData, setFormData] = useState({
    username: '',
    email: '',
    password: '',
    confirmPassword: '',
    firstName: '',
    lastName: '',
    dateOfBirth: ''
  });

  const handleChange = (e: React.ChangeEvent<HTMLInputElement>) => {
    setFormData({ ...formData, [e.target.name]: e.target.value });
  };

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();
    setIsLoading(true);

    try {
      const response = await fetch('http://localhost:5000/api/auth/register', {
        method: 'POST',
        headers: { 'Content-Type': 'application/json' },
        body: JSON.stringify({
          email: formData.email,
          username: formData.username,
          password: formData.password,
          confirmPassword: formData.confirmPassword,
          firstName: formData.firstName,
          lastName: formData.lastName,
          dateOfBirth: formData.dateOfBirth || null,
          gender: 'NotSpecified', // Added default since it's missing from form
          fitnessGoal: 'NotSpecified' // Added default
        })
      });

      if (!response.ok) {
        const err = await response.json();
        let errorMessage = err.message || err.title || 'Registration failed';
        if (err.errors) {
            const validationErrors = Object.values(err.errors).flat().join(', ');
            errorMessage += `: ${validationErrors}`;
        } else if (err.validationErrors) {
            const validationErrors = Object.values(err.validationErrors).flat().join(', ');
            errorMessage += `: ${validationErrors}`;
        }
        throw new Error(errorMessage);
      }

      alert('Registration successful! Please login.');
      navigate('/login');
    } catch (error) {
      console.error('Registration failed:', error);
      alert((error as Error).message);
    } finally {
      setIsLoading(false);
    }
  };

  return (
    <div className="glass-panel">
      <div className="text-center mb-6">
        <h2 style={{ fontSize: '1.75rem', fontWeight: '600', marginBottom: '0.5rem' }}>Create Account</h2>
        <p className="text-muted">Join us and reach your fitness goals</p>
      </div>

      <form onSubmit={handleSubmit}>
        <div className="flex gap-2 mb-4">
          <div className="form-group" style={{ marginBottom: 0, flex: 1 }}>
            <input
              type="text"
              name="firstName"
              className="form-input"
              placeholder="First Name"
              value={formData.firstName}
              onChange={handleChange}
              required
            />
          </div>
          <div className="form-group" style={{ marginBottom: 0, flex: 1 }}>
            <input
              type="text"
              name="lastName"
              className="form-input"
              placeholder="Last Name"
              value={formData.lastName}
              onChange={handleChange}
              required
            />
          </div>
        </div>

        <div className="form-group">
          <div style={{ position: 'relative' }}>
            <div style={{ position: 'absolute', top: '50%', transform: 'translateY(-50%)', left: '1rem', color: 'var(--text-muted)' }}>
              <User size={20} />
            </div>
            <input
              type="text"
              name="username"
              className="form-input"
              placeholder="Username"
              style={{ paddingLeft: '3rem' }}
              value={formData.username}
              onChange={handleChange}
              required
            />
          </div>
        </div>

        <div className="form-group">
          <div style={{ position: 'relative' }}>
            <div style={{ position: 'absolute', top: '50%', transform: 'translateY(-50%)', left: '1rem', color: 'var(--text-muted)' }}>
              <Mail size={20} />
            </div>
            <input
              type="email"
              name="email"
              className="form-input"
              placeholder="Email Address"
              style={{ paddingLeft: '3rem' }}
              value={formData.email}
              onChange={handleChange}
              required
            />
          </div>
        </div>

        <div className="form-group">
          <div style={{ position: 'relative' }}>
            <div style={{ position: 'absolute', top: '50%', transform: 'translateY(-50%)', left: '1rem', color: 'var(--text-muted)' }}>
              <Calendar size={20} />
            </div>
            <input
              type="date"
              name="dateOfBirth"
              className="form-input"
              style={{ paddingLeft: '3rem' }}
              value={formData.dateOfBirth}
              onChange={handleChange}
              required
            />
          </div>
        </div>

        <div className="form-group">
          <div style={{ position: 'relative' }}>
            <div style={{ position: 'absolute', top: '50%', transform: 'translateY(-50%)', left: '1rem', color: 'var(--text-muted)' }}>
              <Lock size={20} />
            </div>
            <input
              type="password"
              name="password"
              className="form-input"
              placeholder="Password"
              style={{ paddingLeft: '3rem' }}
              value={formData.password}
              onChange={handleChange}
              required
            />
          </div>
        </div>

        <div className="form-group">
          <div style={{ position: 'relative' }}>
            <div style={{ position: 'absolute', top: '50%', transform: 'translateY(-50%)', left: '1rem', color: 'var(--text-muted)' }}>
              <Lock size={20} />
            </div>
            <input
              type="password"
              name="confirmPassword"
              className="form-input"
              placeholder="Confirm Password"
              style={{ paddingLeft: '3rem' }}
              value={formData.confirmPassword}
              onChange={handleChange}
              required
            />
          </div>
        </div>

        <button type="submit" className="btn btn-primary mt-4" disabled={isLoading}>
          {isLoading ? <Loader2 className="animate-spin" size={20} /> : (
            <>
              <UserPlus size={20} style={{ marginRight: '0.5rem' }} />
              Create Account
            </>
          )}
        </button>
      </form>

      <div className="text-center mt-6 text-sm">
        <span className="text-muted">Already have an account? </span>
        <Link to="/login" style={{ fontWeight: '600' }}>Sign in</Link>
      </div>
    </div>
  );
};

export default Register;
