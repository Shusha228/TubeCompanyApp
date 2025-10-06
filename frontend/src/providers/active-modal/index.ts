import { useContext } from "react";
import { ActiveModalContext } from "./context";

export const useActiveModal = () => useContext(ActiveModalContext);
export { ActiveModalProvider } from "./provider";
