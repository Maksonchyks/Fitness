import { Outlet, useNavigate, useLocation, Link } from 'react-router-dom';
import { Dumbbell, User, LogOut, LayoutDashboard, Utensils, Activity, MessageCircle } from 'lucide-react';
import './MainLayout.css';
import NotificationManager from './NotificationManager';

const MainLayout = () => {
  const navigate = useNavigate();
  const location = useLocation();
  
  const handleLogout = () => {
    localStorage.removeItem('accessToken');
    navigate('/login');
  };

  const isActive = (path: string) => location.pathname === path;

  return (
    <div className="main-layout">
      <NotificationManager />
      <header className="global-header">
        <div className="header-container">
          <div className="logo-section" onClick={() => navigate('/profile')}>
            <div className="logo-icon">
              <Dumbbell size={24} />
            </div>
            <span className="logo-text">FitnessApp</span>
          </div>

          <nav className="main-nav">
            <Link to="/profile" className={`nav-link ${isActive('/profile') ? 'active' : ''}`}>
              <User size={18} className="mr-1" /> Профіль
            </Link>
            <Link to="/workout" className={`nav-link ${isActive('/workout') ? 'active' : ''}`}>
              <Activity size={18} className="mr-1" /> Тренування
            </Link>
            <Link to="/nutrition" className={`nav-link ${isActive('/nutrition') ? 'active' : ''}`}>
              <Utensils size={18} className="mr-1" /> Харчування
            </Link>
            <Link to="/chat" className={`nav-link ${isActive('/chat') ? 'active' : ''}`}>
              <MessageCircle size={18} className="mr-1" /> Чат
            </Link>
          </nav>

          <div className="user-section">
            <button onClick={handleLogout} className="btn btn-outline" style={{ width: 'auto', padding: '0.5rem 1rem', fontSize: '0.85rem' }}>
              <LogOut size={16} className="mr-2" /> Вийти
            </button>
          </div>
        </div>
      </header>

      <main className="main-content">
        <Outlet />
      </main>

      <footer className="global-footer">
        <div className="footer-container">
          <div className="footer-info">
            <div className="logo-section">
              <div className="logo-icon" style={{ width: 32, height: 32 }}>
                <Dumbbell size={18} />
              </div>
              <span className="logo-text" style={{ fontSize: '1.1rem' }}>FitnessApp</span>
            </div>
            <p>Ваш персональний помічник у світі фітнесу та здорового харчування. Досягайте цілей швидше разом з нами.</p>
          </div>

          <div className="footer-links">
            <div className="footer-group">
              <h4>Продукт</h4>
              <ul>
                <li><Link to="/workout">Тренування</Link></li>
                <li><Link to="/nutrition">Харчування</Link></li>
                <li><Link to="/profile">Мій прогрес</Link></li>
              </ul>
            </div>
            <div className="footer-group">
              <h4>Підтримка</h4>
              <ul>
                <li><a href="#">Довідка</a></li>
                <li><a href="#">Контакти</a></li>
                <li><a href="#">FAQ</a></li>
              </ul>
            </div>
            <div className="footer-group">
              <h4>Правова інформація</h4>
              <ul>
                <li><a href="#">Конфіденційність</a></li>
                <li><a href="#">Умови використання</a></li>
              </ul>
            </div>
          </div>
        </div>
        <div className="footer-bottom">
          &copy; {new Date().getFullYear()} FitnessApp Inc. Всі права захищені.
        </div>
      </footer>
    </div>
  );
};

export default MainLayout;
