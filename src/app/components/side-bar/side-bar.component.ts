import { CommonModule } from '@angular/common';
import { HttpClient } from '@angular/common/http';
import { Component, OnInit } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { FriendService } from '../services/friend.service';
import { environment } from '../../../environments/environment.development';
import { ToastService } from '../toastr/toast.service';

@Component({
  selector: 'app-side-bar',
  templateUrl: './side-bar.component.html',
  styleUrls: ['./side-bar.component.css'],
  imports: [CommonModule, FormsModule],
})
export class SideBarComponent implements OnInit {
  users: any[] = [];
  filteredUsers: any[] = [];
  searchQuery: string = '';
  loading: boolean = false;

  constructor(
    private friendService: FriendService,
    private toastService: ToastService
  ) {}

  ngOnInit() {
    this.getUsers();
  }

  getUsers() {
    this.loading = true;
    this.friendService.GetFriends().subscribe({
      next: (res) => {
        this.users = res.map((res) => ({
          id: res.friendId,
          imageUrl: environment.baseUrl + res.friendImage,
          name: res.friendName,
        }));
        this.filteredUsers = this.users;
        this.loading = false;
      },
      error: (err) => {
        console.error(err);
        this.loading = false;
      },
    });
  }

  addFriend(userId: string) {
    this.friendService.SendFriendReq(userId).subscribe({
      next: (res) => {
        this.toastService.show('request send successfully', 'success');
      },
      error: (err) => console.log(err),
    });
  }

  // Search filter
  ngOnChanges() {
    this.filteredUsers = this.users.filter((u) =>
      u.userName.toLowerCase().includes(this.searchQuery.toLowerCase())
    );
  }
}
