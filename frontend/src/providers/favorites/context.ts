import type { Item } from "@/models/item";
import { createContext } from "react";

export interface FetchFavorites {
  data: Item[];
}

export const FetchFavoritesContext = createContext({} as FetchFavorites);
