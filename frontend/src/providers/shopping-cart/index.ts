import { useContext } from "react";
import { FetchShoppingCartContext } from "./context";

export { FetchShoppingCartProvider } from "./provider";
export const useFetchShoppingCart = () => useContext(FetchShoppingCartContext);
