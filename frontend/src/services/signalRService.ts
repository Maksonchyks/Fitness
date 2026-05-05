import * as signalR from '@microsoft/signalr';

const HUB_URL = 'http://localhost:5003/communicationHub';

class SignalRService {
  private connection: signalR.HubConnection | null = null;

  async startConnection(userId: string, token: string | null) {
    if (this.connection && (this.connection.state === signalR.HubConnectionState.Connected || this.connection.state === signalR.HubConnectionState.Connecting)) {
        return;
    }

    this.connection = new signalR.HubConnectionBuilder()
      .withUrl(`${HUB_URL}?access_token=${token}`)
      .withAutomaticReconnect()
      .build();

    try {
      await this.connection.start();
      console.log('SignalR Connected.');
      
      // Join user group for personal notifications
      // Actually the backend adds to group on connect based on UserIdentifier
      // But we can manually call if needed or if UserIdentifier is not set correctly
    } catch (err) {
      console.error('SignalR Connection Error: ', err);
      setTimeout(() => this.startConnection(userId, token), 5000);
    }
  }

  onReceiveMessage(callback: (message: any) => void) {
    this.connection?.off('ReceiveMessage');
    this.connection?.on('ReceiveMessage', callback);
  }

  onReceiveNotification(callback: (notification: any) => void) {
    this.connection?.off('ReceiveNotification');
    this.connection?.on('ReceiveNotification', callback);
  }

  onMessagesRead(callback: (data: any) => void) {
    this.connection?.off('MessagesRead');
    this.connection?.on('MessagesRead', callback);
  }

  onUserStatusChanged(callback: (data: any) => void) {
    this.connection?.off('UserStatusChanged');
    this.connection?.on('UserStatusChanged', callback);
  }

  async joinRoom(roomId: string) {
    if (this.connection?.state === signalR.HubConnectionState.Connected) {
        console.log(`[SignalR] Joining room: ${roomId}`);
        await this.connection.invoke('JoinRoom', roomId);
    } else {
        console.log(`[SignalR] Connection not ready, retrying JoinRoom: ${this.connection?.state}`);
        setTimeout(() => this.joinRoom(roomId), 1000);
    }
  }

  async leaveRoom(roomId: string) {
    if (this.connection?.state === signalR.HubConnectionState.Connected) {
        await this.connection.invoke('LeaveRoom', roomId);
    }
  }

  stopConnection() {
    this.connection?.stop();
  }
}

export const signalRService = new SignalRService();
