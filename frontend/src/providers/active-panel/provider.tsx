import { useMemo, useState, type JSX } from "react";
import { getComponentByPanel, Panel } from "../../panels";
import { ActivePanelContext } from "./context";

export const ActivePanelProvider = ({
  children,
}: {
  children?: JSX.Element;
}) => {
  const [panel, setPanel] = useState<Panel>(Panel.Main);
  const activePanel = useMemo(() => getComponentByPanel(panel), [panel]);

  return (
    <ActivePanelContext.Provider
      value={{
        _activeObject: panel,
        activePanel: activePanel,
        setActivePanel: setPanel,
      }}
    >
      {children}
    </ActivePanelContext.Provider>
  );
};
