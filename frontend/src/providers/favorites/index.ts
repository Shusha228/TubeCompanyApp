import { useContext } from "react";
import { FetchFavoritesContext } from "./context";

export { FetchFavoritesProvider } from "./provider";
export const useFetchFavorites = () => useContext(FetchFavoritesContext);
