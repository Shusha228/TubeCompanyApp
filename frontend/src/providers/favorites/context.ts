import type { Item } from "@/models/item";
import { createContext } from "react";
import type { FilterSearch } from "../shared/filter-search";

export interface FetchFavorites {
  data: Item[];
  setFilters: (filter: FilterSearch) => void;
  hasInFavorite: (id: number) => boolean;
  toggleFavorite: (item: Item) => void;
}

export const FetchFavoritesContext = createContext({} as FetchFavorites);
