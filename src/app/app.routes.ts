import { Routes } from '@angular/router';
import { Home } from 'lucide-angular';
import { HomeComponent } from './components/home/home.component';
import { ProfileComponent } from './components/profile/profile.component';
import { LoginComponent } from './components/login/login.component';
import { RegisterComponent } from './components/register/register.component';
import { AddPostComponent } from './components/AddPost/AddPost.component';
import { FriendsComponent } from './components/Friends/Friends.component';

export const routes: Routes = [
  { path: '', pathMatch: 'full', component: HomeComponent },
  { path: 'home', component: HomeComponent },
  { path: 'profile', component: ProfileComponent },
  { path: 'login', component: LoginComponent },
  { path: 'AddPost', component: AddPostComponent },
  { path: 'AddPost/:id', component: AddPostComponent },
  { path: 'register', component: RegisterComponent },
  { path: 'friends', component: FriendsComponent },
  { path: '**', component: HomeComponent },
];
