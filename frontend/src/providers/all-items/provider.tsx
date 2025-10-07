import { getURL } from "@/api";
import type { Item } from "@/models/item";
import {
  useCallback,
  useEffect,
  useMemo,
  useRef,
  useState,
  type JSX,
} from "react";
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

  const queryString = useMemo(() => {
    const params = new URLSearchParams();
    if (filters.search && filters.search.trim().length > 0) {
      params.set("search", filters.search.trim());
    }
    const qs = params.toString();
    return qs.length > 0 ? `?${qs}` : "";
  }, [filters]);

  const fetchItems = useCallback(async () => {
    if (abortRef.current) {
      abortRef.current.abort();
    }
    const controller = new AbortController();
    abortRef.current = controller;
    setLoading(true);
    try {
      const response = await fetch(getURL(`Nomenclature${queryString}`), {
        method: "GET",
        headers: {
          accept: "text/plain, application/json",
        },
        signal: controller.signal,
      });

      if (!response.ok) {
        throw new Error(`Request failed: ${response.status}`);
      }

      const contentType = response.headers.get("content-type") || "";
      let payload: unknown;
      if (contentType.includes("application/json")) {
        payload = await response.json();
      } else {
        // Backend may return JSON with text/plain; try to parse
        const text = await response.text();
        try {
          payload = JSON.parse(text);
        } catch {
          // If not JSON, fallback to empty list
          payload = [];
        }
      }

      const items = Array.isArray(payload) ? (payload as Item[]) : [];
      setData(items);
    } catch (error) {
      if ((error as Error).name !== "AbortError") {
        setData([]);
      }
    } finally {
      setLoading(false);
    }
  }, [queryString]);

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
    <FetchAllItemsContext.Provider value={{ data, isLoading, setFilters }}>
      {children}
    </FetchAllItemsContext.Provider>
  );
};
