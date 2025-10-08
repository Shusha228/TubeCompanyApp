import { getURL } from "@/api";
import type { Item } from "@/models/item";
import { useCallback, useEffect, useRef, useState, type JSX } from "react";
import type { FilterSearch } from "../shared/filter-search";
import { FetchAllItemsContext } from "./context";

export const FetchAllItemsProvider = ({
  children,
}: {
  children: JSX.Element;
}) => {
  const [data, setData] = useState<Item[]>([]);
  const [isLoading, setLoading] = useState<boolean>(true);
  const [filters, _setFilters] = useState<FilterSearch>({});
  const abortRef = useRef<AbortController | null>(null);

  const deleteItem = (item: Item) => {
    fetch(getURL(`Nomenclature/${item.id}`), {
      method: "DELETE",
    });
    setData((e) => e.filter((el) => el.id != item.id));
  };

  const addItem = (item: Item) => {
    setData((prev) => [item, ...prev]);
  };

  const fetchItems = useCallback(async () => {
    if (abortRef.current) {
      abortRef.current.abort();
    }
    const controller = new AbortController();
    abortRef.current = controller;
    setLoading(true);
    try {
      const response = await fetch(
        getURL(
          `Nomenclature${
            filters.search !== undefined && filters.search !== ""
              ? `/search?term=${filters.search}`
              : ""
          }`
        ),
        {
          method: "GET",
          headers: {
            accept: "text/plain, application/json",
          },
          signal: controller.signal,
        }
      );

      if (!response.ok) {
        throw new Error(`Request failed: ${response.status}`);
      }

      const payload = ((await response.json()) as { data: Item[] })["data"];

      const items = Array.isArray(payload) ? (payload as Item[]) : [];
      setData(items);
    } catch (error) {
      if ((error as Error).name !== "AbortError") {
        setData([]);
      }
    } finally {
      setLoading(false);
    }
  }, [filters.search]);

  const refreshItems = useCallback(() => {
    fetchItems();
  }, [fetchItems]);

  useEffect(() => {
    fetchItems();
    return () => {
      abortRef.current?.abort();
    };
  }, [fetchItems]);

  const setFilters = useCallback((next: FilterSearch) => {
    _setFilters((prev) => ({ ...prev, ...next }));
  }, []);

  return (
    <FetchAllItemsContext.Provider
      value={{ data, isLoading, setFilters, deleteItem, addItem, refreshItems }}
    >
      {children}
    </FetchAllItemsContext.Provider>
  );
};
