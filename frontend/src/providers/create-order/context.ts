import { createContext, type Dispatch, type SetStateAction } from "react";
import type { ShoppingCartItem } from "../shopping-cart/models/cart-item";

export interface CreateOrder {
  item?: ShoppingCartItem;
  setItem: Dispatch<SetStateAction<ShoppingCartItem | undefined>>;
}

export const CreateOrderContext = createContext({} as CreateOrder);
