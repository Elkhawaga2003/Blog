import { CommonModule } from '@angular/common';
import {
  Component,
  ElementRef,
  HostListener,
  OnInit,
  QueryList,
  ViewChildren,
} from '@angular/core';
import { FormsModule } from '@angular/forms';
import {
  Edit,
  Heart,
  LucideAngularModule,
  MessageCircle,
  MoreVertical,
  Send,
  Share2,
  Trash2,
} from 'lucide-angular';

@Component({
  selector: 'app-profile',
  templateUrl: './profile.component.html',
  styleUrls: ['./profile.component.css'],
  imports: [CommonModule, FormsModule, LucideAngularModule],
})
export class ProfileComponent implements OnInit {
  // Icons
  readonly Heart = Heart;
  readonly MessageCircle = MessageCircle;
  readonly Share2 = Share2;
  readonly MoreVertical = MoreVertical;
  readonly Send = Send;
  readonly Edit = Edit;
  readonly Trash2 = Trash2;
  //feilds
  Liked!: any;
  openMenuId: number | null = null;
  @ViewChildren('menuWrapper') menuWrappers!: QueryList<ElementRef>;
  @ViewChildren('CmenuWrapper') CmenuWrappers!: QueryList<ElementRef>;
  constructor() {}

  ngOnInit() {}

  posts = [
    {
      id: 1,
      user: {
        name: 'Ali Mohamed',
        profileImage: 'https://i.pravatar.cc/150?img=12',
      },
      createdAt: new Date(),
      content: 'new iadea 🔥',
      image: 'https://source.unsplash.com/600x400/?nature',
      likes: 34,
      comments: [
        { id: 1, user: 'Mona', text: 'stule👌' },
        { id: 2, user: 'Tarek', text: 'UI فخم جدًا' },
      ],
      newComment: '',
    },
  ];

  like(post: any) {
    post.likes++;
  }

  addComment(post: any) {
    const commentText = post.newComment?.trim();
    if (commentText) {
      post.comments.push({ user: 'You', text: commentText });
      post.newComment = '';
    }
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
  toggleLike(post: any) {
    this.Liked = !this.Liked;
    if (this.Liked) {
      post.likes++;
    } else {
      post.likes--;
    }
  }
}
