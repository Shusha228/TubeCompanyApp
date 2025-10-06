import { useEffect, useState, type JSX } from "react";
import { FetchItemsContext, type Filters } from "./context";

export const FetchItemProvider = ({ children }: { children?: JSX.Element }) => {
  const [data, setData] = useState([]);
  const [isLoading, setLoading] = useState(true);
  const [, setFilters] = useState<Filters>();
  useEffect(() => {
    setData([]);
    setLoading(false);
  }, []);

  return (
    <FetchItemsContext.Provider
      value={{
        data,
        isLoading,
        setFilters,
      }}
    >
      {children}
    </FetchItemsContext.Provider>
  );
};
