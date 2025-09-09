import { Component } from '@angular/core';
import { RouterOutlet } from '@angular/router';
import { HomeComponent } from './components/home/home.component';
import { HeaderComponent } from './components/header/header.component';
import { ToastrComponent } from './components/toastr/toastr.component';
import { SideBarComponent } from './components/side-bar/side-bar.component';
import { NotificationSidebarComponent } from './components/NotificationSidebar/NotificationSidebar.component';

@Component({
  selector: 'app-root',
  standalone: true,
  imports: [
    RouterOutlet,
    HomeComponent,
    HeaderComponent,
    ToastrComponent,
    SideBarComponent,
    NotificationSidebarComponent,
  ],
  templateUrl: './app.html',
  styleUrls: ['./app.css'],
})
export class App {
  protected title = 'Blog';
}
