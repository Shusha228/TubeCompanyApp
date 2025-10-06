import { useContext } from "react";
import { FetchItemsContext } from "./context";

export { FetchItemProvider } from "./provider";
export const useFetchItem = () => useContext(FetchItemsContext);
