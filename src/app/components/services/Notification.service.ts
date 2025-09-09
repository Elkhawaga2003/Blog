import { Injectable } from '@angular/core';
import * as signalR from '@microsoft/signalr';
import { environment } from '../../../environments/environment.development';
import { BehaviorSubject } from 'rxjs';

@Injectable({
  providedIn: 'root',
})
export class NotificationService {
  private hubConnection!: signalR.HubConnection;

  // هنا هنخزن الرسائل
  private messagesSource = new BehaviorSubject<any[]>([]);
  messages$ = this.messagesSource.asObservable();

  public startConnection(userToken: string): Promise<void> {
    this.hubConnection = new signalR.HubConnectionBuilder()
      .withUrl(`${environment.baseUrl}/notificationHub`, {
        accessTokenFactory: () => userToken,
      })
      .withAutomaticReconnect()
      .build();

    this.hubConnection.on('ReceiveNotification', (message) => {
      console.log('📩 إشعار جديد:', message);
      const currentMessages = this.messagesSource.value;
      this.messagesSource.next([...currentMessages, message]);
    });

    return this.hubConnection
      .start()
      .then(() => console.log('SignalR Connected'))
      .catch((err) => console.log('Error: ' + err));
  }
}
