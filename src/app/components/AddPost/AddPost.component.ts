import { CommonModule } from '@angular/common';
import { Component, OnInit } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { ActivatedRoute, Router } from '@angular/router';
import { LucideAngularModule } from 'lucide-angular';
import { PlusCircle, Image } from 'lucide-angular';
import { ToastService } from '../toastr/toast.service';
import { AddpostService } from '../services/addpost.service';
import { environment } from '../../../environments/environment.development';
@Component({
  selector: 'app-AddPost',
  templateUrl: './AddPost.component.html',
  styleUrls: ['./AddPost.component.css'],
  imports: [CommonModule, FormsModule, LucideAngularModule],
})
export class AddPostComponent implements OnInit {
  readonly Image = Image;
  postId!: number;
  constructor(
    private router: Router,
    private toastServices: ToastService,
    private postServices: AddpostService,
    private activatedRoute: ActivatedRoute
  ) {}

  ngOnInit() {
    this.activatedRoute.paramMap.subscribe((paramMap) => {
      const id = paramMap.get('id');
      if (id) {
        this.postId = +id;
        this.postServices.GetById(this.postId).subscribe({
          next: (res) => {
            this.newPost.content = res.content;
            this.newPost.imagePreview = environment.baseUrl + res.imageUrl;
          },
          error: (err) => console.log(err),
        });
      }
    });
  }
  newPost = {
    content: '',
    image: null as File | null,
    imagePreview: '',
  };

  onImageSelected(event: any) {
    const file = event.target.files[0];
    if (file) {
      this.newPost.image = file;

      const reader = new FileReader();
      reader.onload = () => {
        this.newPost.imagePreview = reader.result as string;
      };
      reader.readAsDataURL(file);
    }
  }

  createPost() {
    if (!this.newPost.content.trim() && !this.newPost.image) return;

    const formData = new FormData();
    formData.append('Content', this.newPost.content);
    if (this.newPost.image) {
      formData.append('Image', this.newPost.image);
    }
    if (this.postId) {
      formData.append('Id', this.postId.toString());
      this.postServices.Update(formData).subscribe({
        next: (res) => {
          this.router.navigateByUrl('/home');
          this.toastServices.show('Updated succesfully', 'success');
        },
        error: (err) => this.toastServices.show(err.error, 'error'),
      });
    } else {
      this.postServices.AddPost(formData).subscribe({
        next: (res) => {
          this.router.navigateByUrl('/home');
          this.toastServices.show('added succesfully', 'success');
        },
        error: (err) => {
          this.toastServices.show(err.error, 'error');
        },
      });
    }
  }
}
