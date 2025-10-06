import { useActivePanel } from "../../providers/active-panel";
import { Panel } from "../models/panel";

export const MainPanel = () => {
  const { setActivePanel } = useActivePanel();
  return (
    <div className="">
      <h1>Main</h1>
      <button onClick={() => setActivePanel(Panel.Cart)}>Cart</button>
    </div>
  );
};
