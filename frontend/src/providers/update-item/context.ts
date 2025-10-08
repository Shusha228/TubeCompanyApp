import type { Item } from "@/models/item";
import { createContext, type Dispatch, type SetStateAction } from "react";

export interface UpdateItem {
  item?: Item;
  setItem: Dispatch<SetStateAction<Item | undefined>>;
  updateItem: (updatedItem: Partial<Item>) => Promise<void>;
}

export const UpdateItemContext = createContext({} as UpdateItem);
