import type { Item } from "@/models/item";
import { createContext } from "react";
import type { FilterSearch } from "../shared/filter-search";

export interface FetchAllItems {
  isLoading: boolean;
  setFilters: (filters: FilterSearch) => void;
  data: Item[];
  deleteItem: (item: Item) => void;
  addItem: (item: Item) => void;
  refreshItems: () => void;
}

export const FetchAllItemsContext = createContext({} as FetchAllItems);
