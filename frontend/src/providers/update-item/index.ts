import { useContext } from "react";
import { UpdateItemContext } from "./context";

export { UpdateItemProvider } from "./provider";
export const useUpdateItem = () => useContext(UpdateItemContext);
