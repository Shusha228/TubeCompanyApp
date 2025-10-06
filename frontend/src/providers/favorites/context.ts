import type { Item } from "@/models/item";
import { createContext } from "react";
import type { FilterSearch } from "../shared/filter-search";

export interface FetchFavorites {
  isLoading: boolean;
  setFilters: (filters: FilterSearch) => void;
  data: Item[];
}

export const FetchFavoritesContext = createContext({} as FetchFavorites);
