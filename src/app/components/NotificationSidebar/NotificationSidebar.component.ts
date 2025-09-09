import { Component, OnInit } from '@angular/core';
import { NotificationService } from '../services/Notification.service';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import {
  LucideAngularModule,
  Bell,
  CheckCircle,
  AlertCircle,
} from 'lucide-angular';
import { environment } from '../../../environments/environment.development';
import { HttpClient } from '@angular/common/http';
import { getUserIdFromToken } from '../services/decode-token';
import { Notfication } from '../../models/Notfication';

@Component({
  selector: 'app-NotificationSidebar',
  templateUrl: './NotificationSidebar.component.html',
  styleUrls: ['./NotificationSidebar.component.css'],
  imports: [CommonModule, FormsModule, LucideAngularModule],
})
export class NotificationSidebarComponent implements OnInit {
  readonly Bell = Bell;
  notifications!: Notfication[];
  isOpen = false;

  constructor(private notificationService: NotificationService) {}

  ngOnInit() {
    let token = localStorage.getItem('token');
    if (token) {
      this.notificationService.startConnection(token).then(() => {
        this.notificationService.messages$.subscribe((msgs) => {
          this.notifications = msgs.map((msg) => ({
            ...msg,
            image: environment.baseUrl + msg.image,
          }));
        });
      });
    }
  }

  toggleSidebar() {
    this.isOpen = !this.isOpen;
  }
}
