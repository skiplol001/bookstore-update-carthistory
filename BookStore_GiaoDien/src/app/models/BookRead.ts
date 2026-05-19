export interface ReadBookDTO {
  productId: number;
  productName: string;
  productImage?: string;
  price: number;
  quantity: number;
  addedDate: Date | string;
}
