import { CommonModule } from '@angular/common';
import { Component, OnInit } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { Router, RouterLink } from '@angular/router';
import { AuthService } from '../register/services/Auth.service';
import { ToastService } from '../toastr/toast.service';

@Component({
  selector: 'app-login',
  templateUrl: './login.component.html',
  styleUrls: ['./login.component.css'],
  imports: [CommonModule, FormsModule, RouterLink],
})
export class LoginComponent implements OnInit {
  email!: string;
  password!: string;
  constructor(
    private authServices: AuthService,
    private toastServices: ToastService,
    private router: Router
  ) {}

  ngOnInit() {}
  login() {
    if (this.password == '' || this.email == '') {
      this.toastServices.show("password or email can't be null", 'error');
    } else {
      let Data = {
        Email: this.email,
        Password: this.password,
      };
      this.authServices.Login(Data).subscribe({
        next: (res) => {
          this.toastServices.show('welcome again', 'success');

          this.authServices.setTokens(res.token, res.refreshToken);
          this.router.navigateByUrl('/home');
        },
        error: (err) => {
          this.toastServices.show(err.error.error, 'error');
        },
      });
    }
  }
}
