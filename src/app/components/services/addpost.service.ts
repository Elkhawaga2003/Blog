import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { environment } from '../../../environments/environment.development';
import { IPost } from '../../models/IPost';
import { IComment } from '../../models/IComment';

@Injectable({
  providedIn: 'root',
})
export class AddpostService {
  constructor(private http: HttpClient) {}
  CreatePost(fomrData: FormData): Observable<any> {
    return this.http.post<any>(`${environment.baseUrl}/Post`, fomrData);
  }
  GetPosts(page: number, pageSize: number): Observable<IPost[]> {
    return this.http.get<IPost[]>(
      `${environment.baseUrl}/Post?page=${page}&pageSize=${pageSize}`
    );
  }
  AddComment(comment: IComment): Observable<IComment> {
    return this.http.post<IComment>(`${environment.baseUrl}/Comment`, comment);
  }
  AddPost(formdata: FormData): Observable<string> {
    return this.http.post<string>(`${environment.baseUrl}/Post`, formdata);
  }
  LikePost(postId: number): Observable<string> {
    return this.http.post<string>(
      `${environment.baseUrl}/Post/Like?postId=${postId}`,
      {}
    );
  }
  DisLikePost(postId: number): Observable<string> {
    return this.http.post<string>(
      `${environment.baseUrl}/Post/DisLike?postId=${postId}`,
      {}
    );
  }
  Delete(Id: number): Observable<string> {
    return this.http.delete<string>(`${environment.baseUrl}/Post/${Id}`);
  }
  GetById(id: number): Observable<IPost> {
    return this.http.get<IPost>(`${environment.baseUrl}/Post/${id}`);
  }
  Update(formData: FormData): Observable<string> {
    return this.http.put<string>(`${environment.baseUrl}/Post`, formData);
  }
}
