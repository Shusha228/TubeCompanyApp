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

  const _fetchWithWrite = (from: number, to: number) => {
    getFavorites(telegramId, from, to)
      .then((el) => el.json())
      .catch(console.log)
      .then((data: ShoppingCartPaginatedResponse) => {
        setData(data.items);
        setLoading(false);
      });
  };
  const next = () => {
    setCurrentFrom((el) => (el += 20));
    setCurrentTo((el) => {
      if (count <= (el += 20)) {
        return count;
      } else {
        return (el += 20);
      }
    });
    setLoading(true);
    _fetchWithWrite(currentFrom, currentTo);
  };

  const setFilters = () => {};

  useEffect(() => {
    fetch("/")
      .then((el) => el.json())
      .then((data: ShoppingCartPaginatedResponse) => {
        setData(data.items);
        setCount(data.meta.totalCount);
        setLoading(false);
      });
  }, []);

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
