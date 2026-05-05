import { Outlet } from 'react-router-dom';
import { Dumbbell } from 'lucide-react';
import './AuthLayout.css';

const AuthLayout = () => {
  return (
    <div className="auth-container">
      <div className="auth-background">
        <div className="glow-orb orb-1"></div>
        <div className="glow-orb orb-2"></div>
      </div>
      
      <div className="auth-content">
        <div className="brand animate-fade-in">
          <div className="brand-logo">
            <Dumbbell size={40} color="var(--primary)" />
          </div>
          <h1>FitnessApp</h1>
          <p>Your ultimate fitness journey starts here.</p>
        </div>
        
        <div className="auth-panel-wrapper animate-fade-in delay-100">
          <Outlet />
        </div>
      </div>
    </div>
  );
};

export default AuthLayout;
