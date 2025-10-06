import { Book, Home, ShoppingCartIcon } from "lucide-react";
import { Panel } from "./panels";
import { useActivePanel } from "./providers/active-panel";

function App() {
  const { activePanel, setActivePanel, _activeObject } = useActivePanel();
  return (
    <>
      {activePanel()}
      <div className="fixed bottom-0 w-full px-4 flex py-3 bg-[#ffffffeb] backdrop-blur-md border-t-1 border-[#0000000d] rounded-t-2xl">
        <div
          onClick={() => setActivePanel(Panel.Main)}
          className="w-full py-2 flex justify-center"
        >
          <Home color={_activeObject == Panel.Main ? "#EC6608" : "#AAAAAA"} />
        </div>
        <div
          onClick={() => setActivePanel(Panel.Favorites)}
          className="w-full py-2 flex justify-center"
        >
          <Book
            color={_activeObject == Panel.Favorites ? "#EC6608" : "#AAAAAA"}
          />
        </div>
        <div
          onClick={() => setActivePanel(Panel.Cart)}
          className="w-full py-2 flex justify-center"
        >
          <ShoppingCartIcon
            color={_activeObject == Panel.Cart ? "#EC6608" : "#AAAAAA"}
          />
        </div>
      </div>
    </>
  );
}

export default App;
