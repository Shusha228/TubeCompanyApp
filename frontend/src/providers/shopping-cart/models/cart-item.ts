import type { Item } from "@/models/item";
import type { PaginatedResult } from "@/models/paginated-result";

export interface ShoppingCartItem {
  userId: number;
  productId: number;
  productName: string;
  quantity: number;
  isInMeters: true;
  finalPrice: number;
  unitPrice: number;
  addedAt: Date;
  updatedAt: Date;
  stockId: string;
  warehouse: string;
  product: Item;
}

export type ShoppingCartPaginatedResponse = PaginatedResult<ShoppingCartItem>;
