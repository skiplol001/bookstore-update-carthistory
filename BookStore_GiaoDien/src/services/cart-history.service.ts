import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class CartHistoryService {
  // Thay thế bằng domain API thực tế của ông nếu có cấu hình CORS riêng, 
  // nếu chạy chung host thì chỉ cần để đường dẫn tương đối.
  private apiUrl = '/CartHistory'; 

  constructor(private http: HttpClient) { }

  // Lấy lịch sử giỏ hàng
  getHistory(): Observable<any> {
    return this.http.get(`${this.apiUrl}/GetHistory`);
  }

  // Đồng bộ session xuống DB
  syncSessionToDb(): Observable<any> {
    return this.http.post(`${this.apiUrl}/SyncSessionToDb`, {});
  }
}
