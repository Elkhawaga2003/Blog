import { CommonModule } from '@angular/common';
import { Component, EventEmitter, OnInit, Output } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { Router } from '@angular/router';

@Component({
  selector: 'app-ProfileImageStep',
  templateUrl: './ProfileImageStep.component.html',
  styleUrls: ['./ProfileImageStep.component.css'],
  imports: [CommonModule, FormsModule],
})
export class ProfileImageStepComponent implements OnInit {
  constructor(private router: Router) {}

  @Output() nextStep = new EventEmitter<{ image: File | null }>();

  selectedImage: File | null = null;
  imagePreviewUrl: string | null = null;

  ngOnInit(): void {}

  onFileSelected(event: Event): void {
    const input = event.target as HTMLInputElement;
    if (input.files && input.files.length > 0) {
      this.selectedImage = input.files[0];

      const reader = new FileReader();
      reader.onload = () => {
        this.imagePreviewUrl = reader.result as string;
      };
      reader.readAsDataURL(this.selectedImage);
    }
  }

  @Output() next = new EventEmitter();
  submit() {
    this.next.emit({ image: this.selectedImage });
  }
}
