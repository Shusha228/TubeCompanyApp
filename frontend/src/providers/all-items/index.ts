import { useContext } from "react";
import { FetchAllItemsContext } from "./context";

export { FetchAllItemsProvider } from "./provider";
export const useFetchAllItems = () => useContext(FetchAllItemsContext);
