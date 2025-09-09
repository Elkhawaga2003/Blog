import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { environment } from '../../../../environments/environment.development';
@Injectable({
  providedIn: 'root',
})
export class FriendsService {
  constructor(private http: HttpClient) {}
  getUserRequest(): Observable<any[]> {
    return this.http.get<any[]>(
      `${environment.baseUrl}/Freind/friend-requests`
    );
  }
  GetUsers(): Observable<any[]> {
    return this.http.get<any[]>(`${environment.baseUrl}/Freind/list-users`);
  }
  AcceptUserRequest(id: number): Observable<any[]> {
    console.log(id);

    return this.http.post<any[]>(
      `${environment.baseUrl}/Freind/accept-request`,
      { id }
    );
  }
  rejecttUserRequest(id: number): Observable<any[]> {
    return this.http.post<any[]>(
      `${environment.baseUrl}/Freind/reject-request`,
      { id }
    );
  }
}
