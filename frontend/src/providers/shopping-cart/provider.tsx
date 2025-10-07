import { getURL } from "@/api";
import { useEffect, useState, type JSX } from "react";
import { useUser } from "../user";
import { FetchShoppingCartContext } from "./context";
import type {
  ShoppingCartItem,
  ShoppingCartPaginatedResponse,
} from "./models/cart-item";

const getFavorites = async (id: number, from: number, to: number) =>
  await fetch(getURL(`Cart/${id}/paged?from=${from}$to=${to}`));

export const FetchShoppingCartProvider = ({
  children,
}: {
  children: JSX.Element;
}) => {
  const { telegramId } = useUser();
  const [data, setData] = useState<ShoppingCartItem[]>([]);
  const [count, setCount] = useState(0);
  const [currentTo, setCurrentTo] = useState(20);
  const [currentFrom, setCurrentFrom] = useState(0);
  const [isLoading, setLoading] = useState(true);

  const next = () => {
    setCurrentFrom((el) => (el += 20));
    setCurrentTo((el) => {
      if (count <= (el += 20)) {
        return count;
      } else {
        return (el += 20);
      }
    });
  };

  const setFilters = () => {};

  useEffect(() => {
    setLoading(true);
    getFavorites(telegramId, currentFrom, currentTo)
      .then((response) => response.json())
      .then((data: ShoppingCartPaginatedResponse) => {
        if (data.items !== undefined) {
          setData((el) => {
            if (data !== undefined) return [...el, ...data.items];
            return el;
          });

          setCount((el) => (el += data.items.length));
          setLoading(false);
        }
      });
  }, [currentFrom, currentTo, telegramId]);

  return (
    <FetchShoppingCartContext.Provider
      value={{
        data,
        isLoading,
        setFilters,
        hasNext: count !== currentTo,
        next,
      }}
    >
      {children}
    </FetchShoppingCartContext.Provider>
  );
};
