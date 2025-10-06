import type { Item } from "@/models/item";

export interface ModalItem {
  IsShowModal: boolean;
  isLoading: boolean;
  showModal: (itemId: string) => void;
  data: Item;
}
