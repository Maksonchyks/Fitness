import React, { useState, useEffect, useRef } from 'react';
import { communicationService, type ChatRoom, type ChatMessage, type Participant } from '../services/communicationService';
import { signalRService } from '../services/signalRService';
import { Send, User, MessageCircle, Info, Plus, X, Search } from 'lucide-react';
import './Chat.css';

const Chat = () => {
  const [rooms, setRooms] = useState<ChatRoom[]>([]);
  const [selectedRoom, setSelectedRoom] = useState<ChatRoom | null>(null);
  const [messages, setMessages] = useState<ChatMessage[]>([]);
  const [newMessage, setNewMessage] = useState('');
  const [userId, setUserId] = useState<string | null>(null);
  const [userName, setUserName] = useState<string | null>(null);
  const [userRole, setUserRole] = useState<string | null>(null);
  const [isNewChatModalOpen, setIsNewChatModalOpen] = useState(false);
  const [contacts, setContacts] = useState<Participant[]>([]);
  const [searchQuery, setSearchQuery] = useState('');
  const [loading, setLoading] = useState(false);
  const messagesAreaRef = useRef<HTMLDivElement>(null);

  useEffect(() => {
    const storedUserId = localStorage.getItem('userId');
    const storedRole = localStorage.getItem('userRole');
    const storedName = localStorage.getItem('userName');
    
    if (storedUserId) {
        setUserId(storedUserId);
        setUserRole(storedRole);
        setUserName(storedName);
        loadRooms(storedUserId);
        const token = localStorage.getItem('accessToken');
        signalRService.startConnection(storedUserId, token);
    }
  }, []);

  useEffect(() => {
    if (isNewChatModalOpen) {
        loadContacts();
    }
  }, [isNewChatModalOpen]);

  const loadContacts = async () => {
    try {
        setLoading(true);
        const data = await communicationService.getAllUsers();
        setContacts(data.filter(c => c.userId !== userId));
    } catch (err) {
        console.error('Failed to load contacts', err);
    } finally {
        setLoading(false);
    }
  };

  const loadRooms = async (id: string) => {
    try {
      const data = await communicationService.getUserRooms(id);
      setRooms(data);
      if (data.length > 0 && !selectedRoom) {
          setSelectedRoom(data[0]);
      }
    } catch (err) {
      console.error('Failed to load rooms', err);
    }
  };

  const handleStartChat = async (contact: Participant) => {
    if (!userId) return;
    try {
        // Ми не хардкодимо ім'я кімнати для приватних чатів, бекенд може його ігнорувати або ми використаємо його як дефолт
        const room = await communicationService.startChat(userId, contact.userId, `Чат з ${contact.username}`);
        
        setRooms(prev => {
            const exists = prev.find(r => r.id === room.id);
            if (exists) return prev;
            return [room, ...prev];
        });
        
        setSelectedRoom(room);
        setIsNewChatModalOpen(false);
    } catch (err) {
        console.error('Failed to start chat', err);
        alert('Не вдалося почати чат. Спробуйте ще раз.');
    }
  };

  useEffect(() => {
    if (selectedRoom) {
      if (userId) {
          console.log(`[Chat] Marking room ${selectedRoom.id} as read for user ${userId}`);
          communicationService.markChatAsRead(selectedRoom.id, userId)
            .then(() => {
                console.log('[Chat] MarkAsRead successful, loading messages...');
                loadMessages(selectedRoom.id);
            })
            .catch(err => {
                console.error('[Chat] MarkAsRead failed', err);
                loadMessages(selectedRoom.id);
            });
      } else {
          loadMessages(selectedRoom.id);
      }
      
      signalRService.onReceiveMessage((msg: ChatMessage) => {
        console.log('[SignalR] Received message:', msg);
        if (selectedRoom && msg.chatRoomId === selectedRoom.id) {
          setMessages(prev => {
              if (prev.find(m => m.id.toLowerCase() === msg.id.toLowerCase())) return prev;
              return [...prev, msg];
          });
        }
      });

      signalRService.onMessagesRead((data: any) => {
        console.log('[SignalR] Messages read event:', data);
        const currentRoomId = selectedRoom?.id?.toLowerCase();
        const incomingRoomId = data.roomId?.toLowerCase();
        
        if (currentRoomId === incomingRoomId && data.readerId?.toLowerCase() !== userId?.toLowerCase()) {
          console.log('[Chat] Updating local messages to READ status');
          setMessages(prev => prev.map(m => ({ ...m, isRead: true })));
        }
      });

        signalRService.onUserStatusChanged((data: any) => {
          console.log('[SignalR] User status changed:', data);
          setRooms(prev => prev.map(room => ({
            ...room,
            participants: room.participants.map(p => 
              p.userId === data.userId ? { ...p, user: { ...p.user, isOnline: data.isActive } } : p
            )
          })));
        
        if (selectedRoom) {
            setSelectedRoom(prev => {
                if (!prev) return null;
                return {
                    ...prev,
                    participants: prev.participants.map(p => 
                        p.userId === data.userId ? { ...p, user: { ...p.user, isOnline: data.isActive } } : p
                    )
                };
            });
        }
      });

      signalRService.joinRoom(selectedRoom.id);
    }

    return () => {
      if (selectedRoom) {
        signalRService.leaveRoom(selectedRoom.id);
      }
    };
  }, [selectedRoom]);

  useEffect(() => {
    const area = messagesAreaRef.current;
    if (area) {
        area.scrollTo({ top: area.scrollHeight, behavior: 'smooth' });
    }
  }, [messages]);

  const loadMessages = async (roomId: string) => {
    try {
      const data = await communicationService.getRoomMessages(roomId);
      setMessages(data);
    } catch (err) {
      console.error('Failed to load messages', err);
    }
  };

  const handleSendMessage = async (e: React.FormEvent) => {
    e.preventDefault();
    if (!newMessage.trim() || !selectedRoom || !userId || !userName) return;

    const textToSend = newMessage.trim();
    setNewMessage('');

    try {
      const sentMsg = await communicationService.sendMessage(selectedRoom.id, userId, userName, textToSend);
      setMessages(prev => {
          if (prev.find(m => m.id.toLowerCase() === sentMsg.id.toLowerCase())) return prev;
          return [...prev, sentMsg];
      });
    } catch (err) {
      console.error('Failed to send message', err);
      setNewMessage(textToSend);
    }
  };

  const getRoomDisplayName = (room: ChatRoom) => {
    if (!userId) return room.name || "Чат";
    
    // Якщо це приватний чат (2 учасники), показуємо ім'я іншого учасника
    if (room.participants && room.participants.length === 2) {
        const other = room.participants.find(p => p.userId !== userId);
        return other ? other.username : (room.name || "Приватний чат");
    }
    
    return room.name || "Груповий чат";
  };

  return (
    <div className="chat-page container">
      <div className="chat-layout">
        <aside className="chat-sidebar">
          <div className="sidebar-header">
            <h3>Повідомлення</h3>
            <button className="btn-new-chat" onClick={() => setIsNewChatModalOpen(true)}>
                <Plus size={22} />
            </button>
          </div>
          <div className="rooms-list">
            {rooms.length === 0 ? (
                <div className="no-rooms-hint" style={{padding: '20px', textAlign: 'center', color: '#64748b', fontSize: '0.9rem'}}>
                    У вас поки немає чатів. Натисніть "+", щоб почати.
                </div>
            ) : rooms.map(room => (
              <div 
                key={room.id} 
                className={`room-item ${selectedRoom?.id === room.id ? 'active' : ''}`}
                onClick={() => setSelectedRoom(room)}
              >
                <div className="room-avatar">
                  <User size={24} />
                </div>
                <div className="room-info">
                  <div className="room-name">{getRoomDisplayName(room)}</div>
                  <div className="room-last-msg">Натисніть, щоб переглянути</div>
                </div>
              </div>
            ))}
          </div>
        </aside>

        <main className="chat-main">
          {selectedRoom ? (
            <>
              <div className="chat-header">
                <div className="header-user">
                  <div className="room-avatar" style={{margin: 0, width: '44px', height: '44px'}}>
                    <User size={22} />
                  </div>
                  <div>
                    <h4>{getRoomDisplayName(selectedRoom)}</h4>
                    {(() => {
                        const other = selectedRoom.participants.find(p => p.userId !== userId);
                        const isOnline = other?.user?.isOnline;
                        return (
                            <div className="status-indicator">
                                <span className={`status-dot ${isOnline ? 'online' : 'offline'}`}></span>
                                <span className={isOnline ? 'status-online' : 'status-offline'}>
                                    {isOnline ? 'Активний' : 'Не в мережі'}
                                </span>
                            </div>
                        );
                    })()}
                  </div>
                </div>
                <button className="btn-icon" style={{background: 'none', border: 'none', color: '#64748b'}}><Info size={22} /></button>
              </div>

              <div className="messages-area" ref={messagesAreaRef}>
                {messages.length === 0 ? (
                    <div style={{flex: 1, display: 'flex', alignItems: 'center', justifyContent: 'center', color: '#94a3b8'}}>
                        Повідомлень поки немає. Напишіть щось!
                    </div>
                ) : messages.map(msg => (
                  <div key={msg.id} className={`message-bubble ${msg.senderId === userId ? 'own' : 'other'}`}>
                    <div className="message-content">
                      <p style={{margin: 0}}>{msg.text}</p>
                      <span className="message-time">
                        {new Date(msg.timestamp).toLocaleTimeString([], { hour: '2-digit', minute: '2-digit' })}
                        {msg.senderId === userId && (
                            <span className="read-status" style={{marginLeft: '6px', color: msg.isRead ? '#10b981' : '#64748b'}}>
                                {msg.isRead ? '✓✓' : '✓'}
                            </span>
                        )}
                      </span>
                    </div>
                  </div>
                ))}
              </div>

              <div className="chat-input-container">
                <form className="chat-input" onSubmit={handleSendMessage}>
                    <input 
                    type="text" 
                    placeholder="Напишіть повідомлення..." 
                    value={newMessage}
                    onChange={(e) => setNewMessage(e.target.value)}
                    />
                    <button type="submit" className="send-btn">
                    <Send size={20} />
                    </button>
                </form>
              </div>
            </>
          ) : (
            <div className="no-room-selected">
              <div style={{background: '#f1f5f9', padding: '30px', borderRadius: '50%', marginBottom: '20px'}}>
                <MessageCircle size={64} color="#3b82f6" />
              </div>
              <p>Оберіть чат або створіть новий,<br/> щоб почати спілкування</p>
              <button className="btn-primary" onClick={() => setIsNewChatModalOpen(true)} style={{marginTop: '10px', borderRadius: '14px', padding: '12px 24px'}}>
                Почати новий чат
              </button>
            </div>
          )}
        </main>
      </div>

      {isNewChatModalOpen && (
        <div className="modal-overlay" onClick={() => setIsNewChatModalOpen(false)}>
          <div className="new-chat-modal" onClick={e => e.stopPropagation()}>
            <div className="modal-header">
              <h3>Новий чат</h3>
              <button onClick={() => setIsNewChatModalOpen(false)} style={{background: 'none', border: 'none'}}><X size={24} /></button>
            </div>
            <div className="modal-search">
              <Search size={20} color="#64748b" />
              <input 
                type="text" 
                placeholder="Пошук за ім'ям..."
                value={searchQuery}
                onChange={(e) => setSearchQuery(e.target.value)}
                autoFocus
              />
            </div>
            <div className="contacts-list">
              {loading ? (
                  <div style={{padding: '40px', textAlign: 'center', color: '#64748b'}}>Завантаження...</div>
              ) : contacts.length === 0 ? (
                  <div style={{padding: '40px', textAlign: 'center', color: '#64748b'}}>Користувачів не знайдено</div>
              ) : contacts
                .filter(c => c.username.toLowerCase().includes(searchQuery.toLowerCase()))
                .map(contact => (
                <div key={contact.userId} className="contact-item" onClick={() => handleStartChat(contact)}>
                  <div className="contact-avatar">
                    <User size={24} />
                  </div>
                  <div className="contact-info">
                    <div className="contact-name">{contact.username}</div>
                    <div className="contact-role">{contact.role}</div>
                  </div>
                  <div className="plus-icon" style={{color: '#3b82f6'}}>
                    <Plus size={20} />
                  </div>
                </div>
              ))}
            </div>
          </div>
        </div>
      )}
    </div>
  );
};

export default Chat;
