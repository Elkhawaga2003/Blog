import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { BehaviorSubject, Observable } from 'rxjs';
import { environment } from '../../../../environments/environment.development';

@Injectable({
  providedIn: 'root',
})
export class AuthService {
  private behaviorSubject = new BehaviorSubject<boolean>(false);
  isLoggedIn$ = this.behaviorSubject.asObservable();
  constructor(private http: HttpClient) {
    const token = localStorage.getItem('token');
    if (token) {
      this.behaviorSubject.next(true);
    }
  }
  Register(formData: any): Observable<any> {
    return this.http.post<any>(`${environment.baseUrl}/Auth`, formData);
  }
  Login(Data: any): Observable<any> {
    return this.http.post<any>(`${environment.baseUrl}/Auth/LogIn`, Data);
  }
  Logout(token: string): Observable<string> {
    return this.http.post<string>(`${environment.baseUrl}/Auth/RevokToken`, {
      token,
    });
  }
  RefreshToken(refreshToken: string): Observable<any> {
    return this.http.post<any>(`${environment.baseUrl}/Auth/RefreshToken`, {
      refreshToken,
    });
  }
  SendVerfication(email: string): Observable<any> {
    return this.http.post<any>(
      `${environment.baseUrl}/Auth/send-verification-code`,
      { email }
    );
  }
  VerfiyCode(data: any): Observable<any> {
    return this.http.post<any>(`${environment.baseUrl}/Auth/verify-code`, data);
  }

  setTokens(token: string, refreshToken: string | null) {
    this.behaviorSubject.next(true);
    localStorage.setItem('token', token);
    if (refreshToken != null)
      localStorage.setItem('RefreshToken', refreshToken);
  }
  ClearTokens() {
    localStorage.removeItem('token');
    localStorage.removeItem('RefreshToken');
    this.behaviorSubject.next(false);
  }
  isLogged(): boolean {
    return this.behaviorSubject.value;
  }
}
