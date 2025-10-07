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
    setData((data) =>
      data.filter(
        (el) =>
          !(el.productId === item.productId && el.stockId === item.stockId)
      )
    );
    fetch(
      getURL(`Cart/${telegramId}/items/${item.stockId}/${item.productId}`),
      {
        method: "DELETE",
      }
    );
  };

  const next = () => {
    setCurrentFrom((prev) => prev + 20);
    setCurrentTo((prev) => {
      const proposed = prev + 20;
      return proposed > count ? count : proposed;
    });
  };

  // Reset pagination and data when search term or user changes
  useEffect(() => {
    setData([]);
    setCount(0);
    setCurrentFrom(0);
    setCurrentTo(20);
  }, [filters.search, telegramId]);

  useEffect(() => {
    setLoading(true);
    getFavorites(telegramId, currentFrom, currentTo, filters.search)
      .then((response) => response.json())
      .then((data: ShoppingCartPaginatedResponse) => {
        if (data.items !== undefined) {
          // Merge uniquely by composite key stockId+productId
          setData((prev) => {
            const seen = new Set<string>();
            const merge = [...prev, ...data.items];
            const unique = merge.filter((it) => {
              const key = `${it.stockId}-${it.productId}`;
              if (seen.has(key)) return false;
              seen.add(key);
              return true;
            });
            return unique;
          });

          // Use server-reported total count for pagination
          setCount(data.meta?.totalCount ?? data.items.length);
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
        hasNext: data.length < count,
        next,
      }}
    >
      {children}
    </FetchShoppingCartContext.Provider>
  );
};
