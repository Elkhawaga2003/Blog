import { CommonModule } from '@angular/common';
import { Component, OnInit } from '@angular/core';
import { RouterLink } from '@angular/router';
import {
  Home,
  LogIn,
  LogOut,
  LucideAngularModule,
  Settings,
  User,
  Users,
  Video,
} from 'lucide-angular';
import { AuthService } from '../register/services/Auth.service';
import { ToastService } from '../toastr/toast.service';

@Component({
  selector: 'app-header',
  templateUrl: './header.component.html',
  styleUrls: ['./header.component.css'],
  imports: [LucideAngularModule, CommonModule, RouterLink],
})
export class HeaderComponent implements OnInit {
  readonly Home = Home;
  readonly User = User;
  readonly Users = Users;
  readonly Video = Video;
  readonly Settings = Settings;
  readonly LogIn = LogIn;
  readonly LogOut = LogOut;

  isLogged!: boolean;
  constructor(
    private authServices: AuthService,
    private toastServices: ToastService
  ) {}

  ngOnInit() {
    this.isLogged = this.authServices.isLogged();
    console.log(this.isLogged);
  }
  Logout() {
    var refreshToken = localStorage.getItem('RefreshToken');
    if (refreshToken == null) {
      this.authServices.ClearTokens();
    } else {
      this.authServices.Logout(refreshToken).subscribe({
        next: (res) => {
          this.authServices.ClearTokens();
          this.toastServices.show('you logout out', 'success');
        },
        error: (err) => {
          console.log(err);

          this.toastServices.show('try letter', 'error');
        },
      });
    }
  }
}
