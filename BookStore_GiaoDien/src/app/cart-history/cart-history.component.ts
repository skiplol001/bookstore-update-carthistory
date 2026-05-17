import { Component, OnInit } from '@angular/core';
import { CartHistoryService } from '../../services/cart-history.service'; // Check lại đường dẫn cho đúng nha ông

@Component({
  selector: 'app-cart-history',
  templateUrl: './cart-history.component.html',
  styleUrls: ['./cart-history.component.css'] // Nếu có file css riêng
})
export class CartHistoryComponent implements OnInit {
  cartHistoryList: any[] = [];
  isLoading: boolean = true;
  isLocalSession: boolean = false; // Thiết lập tùy thuộc vào việc check đăng nhập của ông

  constructor(private cartHistoryService: CartHistoryService) { }

  ngOnInit(): void {
    this.loadHistory();
  }

  loadHistory(): void {
    this.isLoading = true;
    this.cartHistoryService.getHistory().subscribe({
      next: (response) => {
        if (response.success) {
          this.cartHistoryList = response.data;
          // Nếu API trả về flag báo session tạm thời thì bật lên
          if (response.isLocalSession) {
            this.isLocalSession = true;
          }
        } else {
          alert('Không thể tải lịch sử giỏ hàng.');
        }
        this.isLoading = false;
      },
      error: (err) => {
        console.error(err);
        alert('Có lỗi kết nối đến máy chủ.');
        this.isLoading = false;
      }
    });
  }

  syncCart(): void {
    this.cartHistoryService.syncSessionToDb().subscribe({
      next: (response) => {
        if (response.success) {
          alert(response.message);
          this.isLocalSession = false;
          this.loadHistory(); // Tải lại danh sách sau khi gộp
        } else {
          alert(response.message);
        }
      },
      error: (err) => {
        alert('Lỗi đồng bộ hệ thống.');
      }
    });
  }
}
