import type { Item } from "@/models/item";
import { createContext } from "react";
import type { FilterSearch } from "../shared/filter-search";

export interface FetchShoppingCart {
  isLoading: boolean;
  setFilters: (filters: FilterSearch) => void;
  data: Item[];
}

export const FetchShoppingCartContext = createContext({} as FetchShoppingCart);
