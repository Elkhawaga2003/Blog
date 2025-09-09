import { Component, EventEmitter, Input, OnInit, Output } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { AuthService } from '../../services/Auth.service';
import { ToastService } from '../../../toastr/toast.service';

@Component({
  selector: 'app-VerificationStep',
  templateUrl: './VerificationStep.component.html',
  styleUrls: ['./VerificationStep.component.css'],
  imports: [FormsModule],
})
export class VerificationStepComponent implements OnInit {
  constructor(
    private authServices: AuthService,
    private toastServices: ToastService
  ) {}

  ngOnInit() {}
  @Input() email!: string;
  @Output() next = new EventEmitter();
  code = '';

  nextStep() {
    if (this.code.trim()) {
      let data = {
        Email: this.email,
        Code: this.code,
      };
      this.authServices.VerfiyCode(data).subscribe({
        next: (res) => {
          this.toastServices.show(res.message, 'success'),
            this.next.emit({ code: this.code });
        },
        error: (err) => {
          this.toastServices.show(err.error.message, 'error');
        },
      });
    }
  }
}
