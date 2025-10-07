import type { Item } from "@/models/item";
import { createContext } from "react";

export interface ModalItem {
  isLoading: boolean;
  showModal: (itemId: number) => void;
  data: Item | undefined;
}

export const ModalItemContext = createContext({} as ModalItem);
