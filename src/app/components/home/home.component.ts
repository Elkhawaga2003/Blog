import { CommonModule, DatePipe } from '@angular/common';
import {
  Component,
  ElementRef,
  HostListener,
  OnInit,
  QueryList,
  ViewChild,
  ViewChildren,
} from '@angular/core';
import { FormsModule } from '@angular/forms';
import {
  LucideAngularModule,
  Heart,
  MessageCircle,
  Share2,
  MoreVertical,
  Send,
  Edit,
  Trash2,
  LockKeyhole,
} from 'lucide-angular';
import { PlusCircle, Image } from 'lucide-angular';

import { ToastService } from '../toastr/toast.service';
import { AddpostService } from '../services/addpost.service';
import { IPost } from '../../models/IPost';
import { IComment } from '../../models/IComment';
import { environment } from '../../../environments/environment.development';
import { InfiniteScrollModule } from 'ngx-infinite-scroll';
import { Router } from '@angular/router';
import { getUserIdFromToken } from '../services/decode-token';
import { SideBarComponent } from '../side-bar/side-bar.component';
import { NotificationService } from '../services/Notification.service';
import { HttpClient, HttpClientModule } from '@angular/common/http';
@Component({
  selector: 'app-home',
  templateUrl: './home.component.html',
  styleUrls: ['./home.component.css'],
  standalone: true,
  imports: [
    DatePipe,
    CommonModule,
    FormsModule,
    LucideAngularModule,
    FormsModule,
    InfiniteScrollModule,
    SideBarComponent,
  ],
})
export class HomeComponent implements OnInit {
  // Icons
  readonly Heart = Heart;
  readonly MessageCircle = MessageCircle;
  readonly Share2 = Share2;
  readonly MoreVertical = MoreVertical;
  readonly Send = Send;
  readonly Edit = Edit;
  readonly Trash2 = Trash2;
  readonly PlusCircle = PlusCircle;
  readonly Image = Image;
  userImage!: string;
  Liked: boolean = false;
  openMenuId: number | null = null;
  @ViewChildren('menuWrapper') menuWrappers!: QueryList<ElementRef>;
  @ViewChildren('CmenuWrapper') CmenuWrappers!: QueryList<ElementRef>;
  posts: IPost[] = [];
  page = 1;
  pageSize = 10;
  loading = false;
  comments!: IComment[];
  newComment = {} as IComment;
  constructor(
    private toastServices: ToastService,
    private postServices: AddpostService,
    private router: Router,
    private notificationService: NotificationService,
    private http: HttpClient
  ) {}

  ngOnInit() {
    this.loadPosts();
    const token = localStorage.getItem('token');
  }

  loadPosts() {
    if (this.loading) return;

    this.loading = true;

    this.postServices.GetPosts(this.page, this.pageSize).subscribe({
      next: (res) => {
        const token = localStorage.getItem('token');
        let currentUserId: string | null = null;

        if (token) {
          try {
            const decoded: any = getUserIdFromToken(token);
            currentUserId = decoded;
          } catch (err) {
            console.error('Failed to decode token:', err);
          }
        }

        const mappedPosts = res.map((post: any) => {
          const likedUserIds: string[] = post.likedUserIds || [];
          const liked = currentUserId
            ? likedUserIds.includes(currentUserId)
            : false;

          return {
            ...post,
            liked,
            image: post?.image ? `${environment.baseUrl}${post.image}` : null,
            user: {
              ...post.user,
              image: post.user?.image
                ? `${environment.baseUrl}${post.user.image}`
                : null,
            },
            comments:
              post.comments?.map((comment: any) => ({
                ...comment,
                user: comment.user
                  ? {
                      ...comment.user,
                      image: comment.user.image
                        ? `${environment.baseUrl}${comment.user.image}`
                        : null,
                    }
                  : null,
              })) || [],
          };
        });

        this.posts.push(...mappedPosts);
        this.page++;
        this.loading = false;
      },
      error: (err) => {
        console.log(err);
        this.loading = false;
      },
    });
  }

  onScroll() {
    this.loadPosts();
  }

  like(post: any) {
    post.likes++;
  }

  addComment(id: number) {
    this.newComment.postId = id;

    this.postServices.AddComment(this.newComment).subscribe({
      next: (res) => {
        const post = this.posts.find((p) => p.id === id);
        if (post) {
          if (!post.comments) {
            post.comments = [];
          }

          post.comments.push({
            ...res,
            user: {
              ...res.user,
              image: res.user.image
                ? `${environment.baseUrl}${post.user.image}`
                : null,
            },
          });
        }

        this.toastServices.show('Comment added', 'success');
        this.newComment.content = ''; // تفريغ حقل الكومنت بعد الإرسال
      },
      error: (err) => this.toastServices.show(err.error.error, 'error'),
    });
  }

  toggleMenu(id: number) {
    this.openMenuId = this.openMenuId === id ? null : id;
  }

  isMenuOpen(id: number): boolean {
    return this.openMenuId === id;
  }

  commentDate(comment: any): string {
    return new Date().toLocaleString();
  }
  @HostListener('document:click', ['$event'])
  handleClickOutside(event: MouseEvent) {
    let clickedInsideAny = false;

    this.menuWrappers?.forEach((wrapper) => {
      if (wrapper.nativeElement.contains(event.target)) {
        clickedInsideAny = true;
      }
    });
    this.CmenuWrappers?.forEach((wrapper) => {
      if (wrapper.nativeElement.contains(event.target)) {
        clickedInsideAny = true;
      }
    });

    if (!clickedInsideAny) {
      this.openMenuId = null;
    }
  }
  toggleLike(post: IPost) {
    const wasLiked = post.liked;

    post.liked = !wasLiked;
    post.likes += post.liked ? 1 : -1;

    const action = post.liked
      ? this.postServices.LikePost(post.id)
      : this.postServices.DisLikePost(post.id);

    action.subscribe({
      next: () => {},
      error: (err) => {
        console.error(err);

        post.liked = wasLiked;
        post.likes += post.liked ? 1 : -1;
      },
    });
  }

  openAddPostModal() {
    console.log('Open Add Post Modal');
  }
  AddProduct() {
    this.router.navigateByUrl('/AddPost');
  }
  DeletePost(post: IPost) {
    const token = localStorage.getItem('token');
    let currentUserId: string | null = null;
    if (token) {
      try {
        const decoded: any = getUserIdFromToken(token);
        currentUserId = decoded;
      } catch (err) {
        console.error('Failed to decode token:', err);
      }
    }
    if (post.user.id === currentUserId) {
      this.postServices.Delete(post.id).subscribe({
        next: (res) => {
          this.toastServices.show('you delet it succesfully', 'success');
          this.posts = this.posts.filter((p) => p.id !== post.id);
        },
        error: (err) => console.log(err),
      });
    }
  }
  UpdatePost(post: IPost) {
    const token = localStorage.getItem('token');
    let currentUserId: string | null = null;
    if (token) {
      try {
        const decoded: any = getUserIdFromToken(token);
        currentUserId = decoded;
      } catch (err) {
        console.error('Failed to decode token:', err);
      }
    }
    if (post.user.id === currentUserId) {
      this.router.navigateByUrl(`/AddPost/${post.id}`);
    }
  }
}
