import * as signalR from '@microsoft/signalr';

class SignalRService {
    constructor() {
        this.groupConnection = null;
        this.isConnected = false;
    }

    async startConnections() {
        this.groupConnection = new signalR.HubConnectionBuilder()
            .withUrl('https://localhost:5001/groupHub')
            .withAutomaticReconnect()
            .build();

        try {
            await this.groupConnection.start();
            this.isConnected = true;
            console.log('SignalR Connections Established');
        } catch (err) {
            console.error('SignalR Connection Error:', err);
        }
    }

    async joinGroup(groupId) {
        if (this.groupConnection && this.isConnected) {
            await this.groupConnection.invoke('JoinGroup', groupId);
        }
    }

    async leaveGroup(groupId) {
        if (this.groupConnection && this.isConnected) {
            return await this.groupConnection.invoke('LeaveGroup', groupId);
        }
    }

    async sendMessage(groupId, senderId, content) {
        if (this.groupConnection && this.isConnected) {
            return await this.groupConnection.invoke('SendMessage', groupId, senderId, content);
        }
    }

    onReceiveMessage(callback) {
        if (this.groupConnection) {
            this.groupConnection.on('ReceiveMessage', callback)
        }
    }

    stopConnection() {
        if (this.groupConnection) {
            this.groupConnection.stop();
            this.groupConnection = null;
            this.connected = false;
        }
    }
}

export const signalRService = new SignalRService();
