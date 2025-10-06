import type { City } from "@/models/city";
import type { Item } from "@/models/item";
import { createContext, type SetStateAction } from "react";
import type { FilterSearch } from "../shared/filter-search";

export interface Filters extends FilterSearch {
  gosStandart?: string;
  diameter?: string;
  wall?: string;
  steelMark?: string;
  city?: City;
  productTypes?: string[];
}

export interface FetchItems {
  data: Item[];
  isLoading: boolean;
  setFilters: (filters: SetStateAction<Filters | undefined>) => void;
}

export const FetchItemsContext = createContext({} as FetchItems);
