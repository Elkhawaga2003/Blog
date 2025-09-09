import { Component, EventEmitter, OnInit, Output } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { ToastrService } from 'ngx-toastr';
import { ToastService } from '../../../toastr/toast.service';

@Component({
  selector: 'app-PasswordStep',
  templateUrl: './PasswordStep.component.html',
  styleUrls: ['./PasswordStep.component.css'],
  imports: [FormsModule],
})
export class PasswordStepComponent implements OnInit {
  constructor(private toastServices: ToastService) {}

  ngOnInit() {}
  @Output() next = new EventEmitter();
  password = '';
  passwordConfirm = '';

  nextStep() {
    if (this.password.trim() === this.passwordConfirm.trim()) {
      this.next.emit({
        password: this.password,
        passwordConfirm: this.passwordConfirm,
      });
    } else {
      this.toastServices.show("the password dosn't match", 'error');
    }
  }
}
