import { useContext } from "react";
import { ActivePanelContext } from "./context";

export { ActivePanelProvider } from "./provider";
export const useActivePanel = () => useContext(ActivePanelContext);
