import { Component, OnInit } from '@angular/core';
import { ReadBookService } from '../../services/read-book.service';
import { ReadBookDTO } from '../models/BookRead';
import { ToastService } from '../../services/toast.service'; // <-- Check lại đường dẫn tới file toast.service của ông nha

@Component({
  selector: 'app-read-book',
  templateUrl: './read-book.component.html',
  styleUrls: ['./read-book.component.css']
})
export class ReadBookComponent implements OnInit {
  readBookList: ReadBookDTO[] = [];
  isLoading: boolean = true;
  isLocalSession: boolean = false;

  // Inject thêm ToastService vào constructor
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
      next: (response) => {
        if (response.success) {
          this.readBookList = response.data;
          if (response.isLocalSession) {
            this.isLocalSession = true;
          }
        } else {
          // Giả sử hàm của ông tên là error(), nếu tên khác như show() hay danger() thì ông đổi lại tên hàm nha
          this.toast.error('Không thể tải danh sách sách đã đọc.');
        }
        this.isLoading = false;
      },
      error: (err) => {
        console.error(err);
        this.toast.error('Có lỗi kết nối đến máy chủ.');
        this.isLoading = false;
      }
    });
  }

  syncCart(): void {
    this.readBookService.syncSessionToDb().subscribe({
      next: (response) => {
        if (response.success) {
          // Bắn toast success xịn mịn
          this.toast.success(response.message || 'Đồng bộ danh sách đọc thành công!');
          this.isLocalSession = false;
          this.loadHistory(); // Tải lại danh sách sau khi đồng bộ
        } else {
          this.toast.error(response.message || 'Đồng bộ thất bại.');
        }
      },
      error: (err) => {
        console.error(err);
        this.toast.error('Lỗi đồng bộ hệ thống.');
      }
    });
  }
}
