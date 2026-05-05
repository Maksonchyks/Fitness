import React, { useEffect, useState } from 'react';
import { signalRService } from '../services/signalRService';
import { Bell, X, Dumbbell, Apple, Info } from 'lucide-react';
import { useNavigate } from 'react-router-dom';
import { jwtDecode } from 'jwt-decode'; // Библиотека для декодирования JWT
import './NotificationManager.css';

interface Notification {
  title: string;
  message: string;
  timestamp: string;
  type: string;
}

const NotificationManager = () => {
  const [notifications, setNotifications] = useState<Notification[]>([]);
  const navigate = useNavigate();

  useEffect(() => {
    console.log("[SignalR] NotificationManager mounted. Checking token...");
    const token = localStorage.getItem('accessToken');
    if (!token) {
      console.warn("[SignalR] No accessToken found in localStorage. Connection aborted.");
      return;
    }

    try {
      const decoded: any = jwtDecode(token);
      const userId = decoded.sub || decoded["http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier"];
      console.log("[SignalR] Token decoded. User ID:", userId);
      
      if (userId) {
        console.log("[SignalR] Starting connection...");
        signalRService.startConnection(userId, token);
      }
    } catch (e) {
      console.error("[SignalR] Failed to decode token or start connection", e);
    }

    signalRService.onReceiveNotification((notif: Notification) => {
      console.log("[SignalR] Received notification:", notif);
      setNotifications(prev => [notif, ...prev]);
      
      // Авто-видалення через 10 секунд
      setTimeout(() => {
        setNotifications(prev => prev.filter(n => n !== notif));
      }, 10000);
    });

    return () => {
      signalRService.stopConnection();
    };
  }, []);

  const handleNotificationClick = (notif: Notification, index: number) => {
    // Логіка переходів залежно від типу
    if (notif.type.toLowerCase().includes('workout')) {
      navigate('/workout');
    } else if (notif.type.toLowerCase().includes('nutrition') || notif.type.toLowerCase().includes('meal')) {
      navigate('/nutrition');
    }
    
    removeNotification(index);
  };

  const removeNotification = (index: number) => {
    setNotifications(prev => prev.filter((_, i) => i !== index));
  };

  const getIcon = (type: string) => {
    const t = type.toLowerCase();
    if (t.includes('workout')) return <Dumbbell size={20} />;
    if (t.includes('nutrition') || t.includes('meal')) return <Apple size={20} />;
    return <Info size={20} />;
  };

  if (notifications.length === 0) return null;

  return (
    <div className="notification-container">
      {notifications.map((notif, index) => (
        <div 
          key={index} 
          className={`notification-toast ${notif.type.toLowerCase()}`}
          onClick={() => handleNotificationClick(notif, index)}
          style={{ cursor: 'pointer' }}
        >
          <div className="notification-icon">
            {getIcon(notif.type)}
          </div>
          <div className="notification-content">
            <div className="notification-title">{notif.title}</div>
            <div className="notification-message">{notif.message}</div>
          </div>
          <button 
            className="notification-close" 
            onClick={(e) => {
              e.stopPropagation();
              removeNotification(index);
            }}
          >
            <X size={16} />
          </button>
        </div>
      ))}
    </div>
  );
};

export default NotificationManager;
