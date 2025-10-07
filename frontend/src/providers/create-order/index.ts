import { useContext } from "react";
import { CreateOrderContext } from "./context";

export { CreateOrderProvider } from "./provider";
export const useCreateOrder = () => useContext(CreateOrderContext);
