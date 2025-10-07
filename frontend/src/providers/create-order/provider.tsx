import { useState, type JSX } from "react";
import type { ShoppingCartItem } from "../shopping-cart/models/cart-item";
import { CreateOrderContext } from "./context";

export const CreateOrderProvider = ({
  children,
}: {
  children: JSX.Element;
}) => {
  const [item, setItem] = useState<ShoppingCartItem>();
  return (
    <CreateOrderContext.Provider value={{ item, setItem }}>
      {children}
    </CreateOrderContext.Provider>
  );
};
