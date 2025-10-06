import { useEffect, useState, type JSX } from "react";
import type { FilterSearch } from "../shared/filter-search";
import { FetchShoppingCartContext } from "./context";

export const FetchShoppingCartProvider = ({
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
    <FetchShoppingCartContext.Provider value={{ data, isLoading, setFilters }}>
      {children}
    </FetchShoppingCartContext.Provider>
  );
};
