import { CommonModule } from '@angular/common';
import { Component, OnInit } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { InfiniteScrollModule } from 'ngx-infinite-scroll';
import { FriendsService } from './services/friends.service';
import { environment } from '../../../environments/environment.development';
import { IUser } from '../../models/IUser';
import { identity } from 'rxjs';
import { AddpostService } from '../services/addpost.service';
import { FriendService } from '../services/friend.service';

@Component({
  selector: 'app-Friends',
  templateUrl: './Friends.component.html',
  styleUrls: ['./Friends.component.css'],
  standalone: true, // لو Angular 14+ وتستخدم standalone components
  imports: [CommonModule, FormsModule, InfiniteScrollModule],
})
export class FriendsComponent implements OnInit {
  friendRequests: any[] = [];
  users: IUser[] = [];
  loading = false;
  constructor(
    private friendsServices: FriendsService,
    private friendService: FriendService
  ) {}

  ngOnInit() {
    this.friendsServices.getUserRequest().subscribe({
      next: (res) => {
        this.friendRequests = res.map((res) => ({
          ...res,
          image: environment.baseUrl + res.image,
        }));
      },
      error: (err) => console.log(err),
    });
    this.friendsServices.GetUsers().subscribe({
      next: (res) => {
        this.users = res.map((res) => ({
          ...res,
          image: environment.baseUrl + res.imageUrl,
        }));
      },
      error: (err) => console.log(err),
    });
  }

  onScroll() {
    if (this.loading) return;
    this.loading = true;
  }

  acceptRequest(id: any) {
    this.friendsServices.AcceptUserRequest(id.requestId).subscribe({
      next: (res) => {
        this.friendRequests = this.friendRequests.filter((r) => r.id !== id);
      },
      error: (err) => console.log(err),
    });
  }

  declineRequest(id: any) {
    this.friendsServices.rejecttUserRequest(id).subscribe({
      next: (res) => {
        this.friendRequests = this.friendRequests.filter((r) => r.id !== id);
      },
      error: (err) => console.log(err),
    });
  }

  sendFriendRequest(id: string) {
    this.friendService.SendFriendReq(id).subscribe({
      next: (res) => console.log(res),
      error: (err) => console.error(err),
    });
  }
}
