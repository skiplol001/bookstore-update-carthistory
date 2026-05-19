import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { ReadBookDTO } from '../app/models/BookRead';

@Injectable({
  providedIn: 'root'
})
export class ReadBookService {
  private apiUrl = '/ReadBookController'; 

  constructor(private http: HttpClient) { }

  getHistory(): Observable<{ success: boolean; data: ReadBookDTO[]; isLocalSession?: boolean }> {
    return this.http.get<{ success: boolean; data: ReadBookDTO[]; isLocalSession?: boolean }>(`${this.apiUrl}/GetHistory`);
  }

  syncSessionToDb(): Observable<{ success: boolean; message: string }> {
    return this.http.post<{ success: boolean; message: string }>(`${this.apiUrl}/SyncSessionToDb`, {});
  }
}
