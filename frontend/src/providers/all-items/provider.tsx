import { useEffect, useState, type JSX } from "react";
import type { FilterSearch } from "../shared/filter-search";
import { FetchAllItemsContext } from "./context";

export const FetchAllItemsProvider = ({
  children,
}: {
  children: JSX.Element;
}) => {
  const [data, setData] = useState([]);
  const [isLoading, setLoading] = useState(true);
  const [, setFilters] = useState<FilterSearch>();

  useEffect(() => {
    setData([]);
    setLoading(false);
  }, []);

  return (
    <FetchAllItemsContext.Provider value={{ data, isLoading, setFilters }}>
      {children}
    </FetchAllItemsContext.Provider>
  );
};
