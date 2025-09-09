import { CommonModule } from '@angular/common';
import { Component, OnInit } from '@angular/core';
import { NameStepComponent } from './components/NameStep/NameStep.component';
import { EmailStepComponent } from './components/EmailStep/EmailStep.component';
import { VerificationStepComponent } from './components/VerificationStep/VerificationStep.component';
import { ProfileImageStepComponent } from './components/ProfileImageStep/ProfileImageStep.component';
import { Router } from '@angular/router';
import { PasswordStepComponent } from './components/PasswordStep/PasswordStep.component';
import { ToastrService } from 'ngx-toastr';
import { ToastService } from '../toastr/toast.service';
import { AuthService } from './services/Auth.service';

@Component({
  selector: 'app-register',
  templateUrl: './register.component.html',
  styleUrls: ['./register.component.css'],
  imports: [
    CommonModule,
    NameStepComponent,
    EmailStepComponent,
    VerificationStepComponent,
    ProfileImageStepComponent,
    PasswordStepComponent,
  ],
})
export class RegisterComponent implements OnInit {
  constructor(
    private router: Router,
    private toastServices: ToastService,
    private authServices: AuthService
  ) {}

  ngOnInit() {}
  step = 1;
  formData: any = {};

  nextStep(data: any) {
    this.formData = { ...this.formData, ...data };
    if (this.step == 5) {
      const formData2 = new FormData();
      formData2.append('Email', this.formData.email);
      formData2.append('ConfirmPassword', this.formData.passwordConfirm);
      formData2.append('Image', this.formData.image);
      formData2.append('Password', this.formData.password);
      formData2.append('Name', this.formData.name);

      this.authServices.Register(formData2).subscribe({
        next: (res) => {
          this.toastServices.show('register successfully', 'success');
          this.authServices.setTokens(res.token, null);
          this.router.navigateByUrl('/home');
        },
        error: (err) => {
          this.toastServices.show('somthing is wrong', 'error'),
            console.log(err);
        },
      });
    }
    this.step++;
  }

  goTo(step: any) {
    this.step++;
  }
}
