import type { Item } from "@/models/item";
import { useEffect, useState, type JSX } from "react";
import type { FilterSearch } from "../shared/filter-search";
import { FetchFavoritesContext } from "./context";

export const FetchFavoritesProvider = ({
  children,
}: {
  children: JSX.Element;
}) => {
  const [_data, _setData] = useState<Item[]>([]);
  const [visibleData, setVisibleData] = useState(_data);
  const [, _setFilters] = useState<FilterSearch>({});

  const hasInFavorite = (id: number) => {
    return _data.find((el) => el.id == id) !== undefined;
  };

  const setFilters = (filters: FilterSearch) => {
    _setFilters(filters);
    if (filters.search !== undefined && filters.search !== "") {
      setVisibleData(
        _data.filter((el) => {
          if (filters.search !== undefined) {
            return el.name
              .toLocaleLowerCase()
              .includes(filters.search.toLocaleLowerCase());
          } else {
            return true;
          }
        })
      );
    }
  };

  const toggleFavorite = (item: Item) => {
    if (hasInFavorite(item.id)) {
      _setData((el) => el.filter((i) => i.id !== item.id));
    } else {
      _setData((el) => [item, ...el]);
    }
    setVisibleData(_data);
    localStorage.setItem("favorites", JSON.stringify(_data));
  };

  useEffect(() => {
    const favoritesPersiste = localStorage.getItem("favorites");
    if (favoritesPersiste !== null) {
      _setData(JSON.parse(favoritesPersiste));
      setVisibleData(_data);
    }
  }, []);

  return (
    <FetchFavoritesContext.Provider
      value={{ data: visibleData, setFilters, hasInFavorite, toggleFavorite }}
    >
      {children}
    </FetchFavoritesContext.Provider>
  );
};
