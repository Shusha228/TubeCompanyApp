import { useContext } from "react";
import { ModalItemContext } from "./context";

export { ModalItemProvider } from "./provider";
export const useModalItem = () => useContext(ModalItemContext);
