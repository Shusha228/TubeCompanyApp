import { getURL } from "@/api";
import { useEffect, useState, type JSX } from "react";
import type { FilterSearch } from "../shared/filter-search";
import { useUser } from "../user";
import { FetchShoppingCartContext } from "./context";
import type {
  ShoppingCartItem,
  ShoppingCartPaginatedResponse,
} from "./models/cart-item";

const getFavorites = async (
  id: number,
  from: number,
  to: number,
  term?: string
) =>
  await fetch(
    getURL(
      `Cart/${id}${
        term !== undefined && term !== "" ? "/search" : ""
      }/paged?from=${from}&to=${to}` +
        (term !== undefined && term !== "" ? `&term=${term}` : "")
    )
  );

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
  const [filters, setFilters] = useState<FilterSearch>({});

  const deleteItem = (item: ShoppingCartItem) => {
    setData((data) => data.filter((el) => el.productId != item.productId));
    fetch(
      getURL(`Cart/${telegramId}/items/${item.stockId}/${item.productId}`),
      {
        method: "DELETE",
      }
    );
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
  };

  useEffect(() => {
    setLoading(true);
    getFavorites(telegramId, currentFrom, currentTo, filters.search)
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
  }, [currentFrom, currentTo, telegramId, filters.search]);

  return (
    <FetchShoppingCartContext.Provider
      value={{
        data,
        deleteItem,
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
