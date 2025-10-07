import { createContext } from "react";
import type { FilterSearch } from "../shared/filter-search";
import type { ShoppingCartItem } from "./models/cart-item";

export interface FetchShoppingCart {
  isLoading: boolean;
  setFilters: (filters: FilterSearch) => void;
  next: () => void;
  hasNext: boolean;
  data: ShoppingCartItem[];
  deleteItem: (item: ShoppingCartItem) => void;
}

export const FetchShoppingCartContext = createContext({} as FetchShoppingCart);
