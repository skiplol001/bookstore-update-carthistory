import { Injectable } from '@angular/core';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../environments/environment';
import { ReadBookDTO } from '../app/models/BookRead';

@Injectable({
  providedIn: 'root'
})
export class ReadBookService {
  private apiUrl = `${environment.apiUrl}/ReadBook`;

  constructor(private http: HttpClient) { }

  getHistory(): Observable<{ success: boolean; data: ReadBookDTO[]; isLocalSession?: boolean }> {
    return this.http.get<{ success: boolean; data: ReadBookDTO[]; isLocalSession?: boolean }>(`${this.apiUrl}/GetHistory`);
  }

  syncSessionToDb(): Observable<{ success: boolean; message: string }> {
    return this.http.post<{ success: boolean; message: string }>(`${this.apiUrl}/SyncSessionToDb`, {});
  }

  addHistory(productId: number): Observable<{ success: boolean; message: string }> {
    const token = localStorage.getItem('token') || localStorage.getItem('accessToken') || '';

    // Thiết lập Header bắt buộc để C# Controller nhận diện đúng kiểu Form gửi lên
    const headers = new HttpHeaders({
      'Authorization': `Bearer ${token}`,
      'Content-Type': 'application/x-www-form-urlencoded'
    });

    // Đóng gói tham số rời rạc thành chuỗi URL-encoded chuẩn hóa
    const body = new URLSearchParams();
    body.set('productId', productId.toString());
    body.set('quantity', '1');

    return this.http.post<{ success: boolean; message: string }>(
      `${this.apiUrl}/AddToHistory`,
      body.toString(),
      { headers }
    );
  }
}
