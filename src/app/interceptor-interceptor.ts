import { HttpErrorResponse, HttpInterceptorFn } from '@angular/common/http';
import { inject } from '@angular/core';
import { Router } from '@angular/router';
import { catchError, switchMap, throwError } from 'rxjs';
import { ToastService } from './components/toastr/toast.service';
import { AuthService } from './components/register/services/Auth.service';

export const interceptorInterceptor: HttpInterceptorFn = (req, next) => {
  const userService = inject(AuthService);
  let router = inject(Router);
  const token = localStorage.getItem('token');
  const refreshToken = localStorage.getItem('RefreshToken');
  let toastr = inject(ToastService);
  if (token) {
    req = req.clone({
      setHeaders: {
        Authorization: `Bearer ${token}`,
      },
    });
  }

  return next(req).pipe(
    catchError((error: HttpErrorResponse) => {
      if (error.status === 401 && refreshToken) {
        return userService.RefreshToken(refreshToken).pipe(
          switchMap((res: any) => {
            userService.setTokens(res.token, res.refreshToken);
            const newReq = req.clone({
              setHeaders: {
                Authorization: `Bearer ${res.token}`,
              },
            });
            return next(newReq);
          }),
          catchError((refreshErr) => {
            console.error('Refresh Failed:', refreshErr);
            userService.ClearTokens();
            return throwError(() => refreshErr);
          })
        );
      } else if (error.status == 401) {
        toastr.show('you are not sign in yet', 'error');
      }
      router.navigateByUrl('/home');
      return throwError(() => error);
    })
  );
};
