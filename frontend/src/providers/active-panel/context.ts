import { createContext, type JSX } from "react";
import type { Panel } from "../../panels";

export interface ActivePanel {
  activePanel: () => JSX.Element;
  _activeObject: Panel;
  setActivePanel: (panel: Panel) => void;
}

export const ActivePanelContext = createContext({} as ActivePanel);
