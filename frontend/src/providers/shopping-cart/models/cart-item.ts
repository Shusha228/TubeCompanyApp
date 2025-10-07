import type { Item } from "@/models/item";
import type { PaginatedResult } from "@/models/paginated-result";

export interface ShoppingCartItem {
  userId: number;
  productId: number;
  productName: string;
  quantity: number;
  isInMeters: boolean;
  finalPrice: number;
  unitPrice: number;
  addedAt: string; // ISO string from API
  updatedAt: string; // ISO string from API
  stockId: string;
  warehouse: string;
  product: Item | null;
}

export type ShoppingCartPaginatedResponse = PaginatedResult<ShoppingCartItem>;
