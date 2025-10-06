import { useEffect, useState, type JSX } from "react";
import type { FilterSearch } from "../shared/filter-search";
import { FetchFavoritesContext } from "./context";

export const FetchFavoritesProvider = ({
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
    <FetchFavoritesContext.Provider value={{ data, isLoading, setFilters }}>
      {children}
    </FetchFavoritesContext.Provider>
  );
};
