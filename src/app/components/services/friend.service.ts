import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { environment } from '../../../environments/environment.development';

@Injectable({
  providedIn: 'root',
})
export class FriendService {
  constructor(private http: HttpClient) {}
  GetFriends(): Observable<any[]> {
    return this.http.get<any[]>(`${environment.baseUrl}/Freind/list-friends`);
  }
  SendFriendReq(accepterId: string): Observable<string> {
    return this.http.post<string>(
      `${environment.baseUrl}/Freind/send-request`,
      { accepterId }
    );
  }
}
