import { Component, EventEmitter, OnInit, Output } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { AuthService } from '../../services/Auth.service';
import { HotToastService } from '@ngneat/hot-toast';
import { Toast, ToastrService } from 'ngx-toastr';
import { ToastService } from '../../../toastr/toast.service';

@Component({
  selector: 'app-EmailStep',
  templateUrl: './EmailStep.component.html',
  styleUrls: ['./EmailStep.component.css'],
  imports: [FormsModule],
})
export class EmailStepComponent implements OnInit {
  constructor(
    private authServices: AuthService,
    private toastServices: ToastService
  ) {}
  email: string = '';
  ngOnInit() {}
  @Output() next = new EventEmitter();

  nextStep() {
    if (this.email.trim()) {
      this.authServices.SendVerfication(this.email).subscribe({
        next: (res) => {
          this.toastServices.show(res.message, 'success'),
            this.next.emit({ email: this.email });
        },
        error: (err) => {
          this.toastServices.show(err.message, 'error');
        },
      });
    }
  }
}
