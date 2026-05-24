import { Component, OnInit } from '@angular/core';
import { ReadBookService } from '../../services/read-book.service';
import { ReadBookDTO } from '../models/BookRead';
import { ToastService } from '../../services/toast.service';

@Component({
  selector: 'app-read-book',
  templateUrl: './read-book.component.html',
  //styleUrls: ['./read-book.component.css'] 
})
export class ReadBookComponent implements OnInit {
  readBookList: ReadBookDTO[] = [];
  isLoading: boolean = true;
  isLocalSession: boolean = false;

  constructor(
    private readBookService: ReadBookService,
    private toast: ToastService
  ) { }

  ngOnInit(): void {
    this.loadHistory();
  }

  loadHistory(): void {
    this.isLoading = true;
    this.readBookService.getHistory().subscribe({
      next: (response: any) => {
        if (response.success) {
          this.readBookList = response.data;
          if (response.isLocalSession) {
            this.isLocalSession = true;
          }
        } else {
          this.toast.show('Không thể tải danh sách sách đã đọc.', 'error');
        }
        this.isLoading = false;
      },
      error: (err: any) => {
        console.error(err);
        this.toast.show('Có lỗi kết nối đến máy chủ.', 'error');
        this.isLoading = false;
      }
    });
  }

  syncCart(): void {
    this.readBookService.syncSessionToDb().subscribe({
      next: (response: any) => {
        if (response.success) {
          this.toast.show(response.message || 'Đồng bộ danh sách đọc thành công!', 'success');
          this.isLocalSession = false;
          this.loadHistory();
        } else {
          this.toast.show(response.message || 'Đồng bộ thất bại.', 'error');
        }
      },
      error: (err: any) => {
        console.error(err);
        this.toast.show('Lỗi đồng bộ hệ thống.', 'error');
      }
    });
  }
}
