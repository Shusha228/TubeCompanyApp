import { CartPanel } from "./components/cart";
import { FavoritesPanel } from "./components/favorites";
import { MainPanel } from "./components/main";
import { ProfilePanel } from "./components/profile";
import { Panel } from "./models/panel";

export { Panel } from "./models/panel";

const PanelComponentDict = {
  [Panel.Main]: MainPanel,
  [Panel.Cart]: CartPanel,
  [Panel.Favorites]: FavoritesPanel,
  [Panel.Profile]: ProfilePanel,
} as const;

export const getComponentByPanel = (panel: Panel) => PanelComponentDict[panel];
