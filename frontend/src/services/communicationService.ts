import axios from 'axios';

const API_URL = 'http://localhost:5003/api';

const api = axios.create({
  baseURL: API_URL
});

api.interceptors.request.use((config) => {
  const token = localStorage.getItem('accessToken');
  if (token) {
    config.headers.Authorization = `Bearer ${token}`;
  }
  return config;
});

export interface ChatMessage {
  id: string;
  chatRoomId: string;
  senderId: string;
  senderName: string;
  text: string;
  timestamp: string;
  isRead: boolean;
}

export interface ChatRoom {
  id: string;
  name: string;
  createdAt: string;
  participants: Participant[];
}

export interface Participant {
  userId: string;
  username: string;
  role: string;
  isActive?: boolean;
  user?: {
    isActive: boolean;
  };
}

export interface UserNotification {
  id: string;
  title: string;
  message: string;
  createdAt: string;
  isRead: boolean;
  type: string;
}

export interface ReminderSchedule {
  reminderTime: string; // "HH:mm:ss"
  label: string;
  type: string;
}

export interface UserNotificationPreference {
  nutritionEnabled: boolean;
  workoutEnabled: boolean;
  schedules: ReminderSchedule[];
}

export const communicationService = {
  // Chat
  getUserRooms: async (userId: string): Promise<ChatRoom[]> => {
    const response = await api.get(`/chat/rooms/${userId}`);
    return response.data;
  },
  getRoomMessages: async (roomId: string): Promise<ChatMessage[]> => {
    const response = await api.get(`/chat/rooms/${roomId}/messages`);
    return response.data;
  },
  sendMessage: async (roomId: string, senderId: string, senderName: string, text: string): Promise<ChatMessage> => {
    const response = await api.post(`/chat/messages`, { 
        RoomId: roomId, 
        SenderId: senderId, 
        SenderName: senderName, 
        Text: text 
    });
    return response.data;
  },
  startChat: async (creatorId: string, targetUserId: string, roomName: string): Promise<ChatRoom> => {
    try {
        const response = await api.post(`/chat/rooms`, { 
            CreatorId: creatorId, 
            TargetUserId: targetUserId, 
            RoomName: roomName 
        });
        return response.data;
    } catch (error: any) {
        console.error('StartChat API Error:', error.response?.data || error.message);
        throw error;
    }
  },

  markChatAsRead: async (roomId: string, userId: string): Promise<void> => {
    await api.post(`/chat/rooms/${roomId}/read`, { userId });
  },

  // Users
  getCoaches: async (): Promise<Participant[]> => {
    const response = await api.get(`/users/coaches`);
    return response.data.map((u: any) => ({
        userId: u.id,
        username: u.fullName || u.username,
        role: u.role
    }));
  },
  getClients: async (): Promise<Participant[]> => {
    const response = await api.get(`/users/clients`);
    return response.data.map((u: any) => ({
        userId: u.id,
        username: u.fullName || u.username,
        role: u.role
    }));
  },
  getAllUsers: async (): Promise<Participant[]> => {
    const response = await api.get(`/users/all`);
    return response.data.map((u: any) => ({
        userId: u.id,
        username: u.fullName || u.username,
        role: u.role
    }));
  },

  // Notifications
  getPreferences: async (userId: string): Promise<UserNotificationPreference> => {
    const response = await api.get(`/notification/preferences/${userId}`);
    return response.data;
  },
  updatePreferences: async (userId: string, prefs: UserNotificationPreference): Promise<void> => {
    await api.put(`/notification/preferences/${userId}`, prefs);
  },
  getNotificationHistory: async (userId: string): Promise<UserNotification[]> => {
    const response = await api.get(`/notification/history/${userId}`);
    return response.data;
  },
  markAsRead: async (id: string): Promise<void> => {
    await api.post(`/notification/history/${id}/read`);
  }
};
