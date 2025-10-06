import { CartPanel } from "./components/cart";
import { FavoritesPanel } from "./components/favorites";
import { ItemPanel } from "./components/item";
import { MainPanel } from "./components/main";
import { Panel } from "./models/panel";

export { Panel } from "./models/panel";

const PanelComponentDict = {
  [Panel.Main]: MainPanel,
  [Panel.Cart]: CartPanel,
  [Panel.Item]: ItemPanel,
  [Panel.Favorites]: FavoritesPanel,
} as const;

export const getComponentByPanel = (panel: Panel) => PanelComponentDict[panel];
