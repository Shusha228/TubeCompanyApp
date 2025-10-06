import { createContext, type JSX } from "react";
import type { Panel } from "../../panels";

export interface ActivePanel {
  activePanel: () => JSX.Element;
  setActivePanel: (panel: Panel) => void;
}

export const ActivePanelContext = createContext({} as ActivePanel);
