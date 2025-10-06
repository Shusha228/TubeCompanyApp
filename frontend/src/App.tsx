import { ArrowLeft, Book, Home, ShoppingCartIcon } from "lucide-react";
import { Avatar, AvatarFallback } from "./components/ui/avatar";
import { Button } from "./components/ui/button";
import { ScrollArea, ScrollBar } from "./components/ui/scroll-area";
import { cn } from "./lib/utils";
import { Panel } from "./panels";
import { useActiveModal } from "./providers/active-modal";
import { useActivePanel } from "./providers/active-panel";

function App() {
  const { activePanel, setActivePanel, _activeObject } = useActivePanel();
  const { activeModal, closeModal } = useActiveModal();
  return (
    <>
      {activePanel()}

      {activeModal && (
        <div className="fixed w-dvw h-dvh top-0 right-0 z-[10] overflow-hidden">
          <ScrollArea className="relative w-dvw h-dvh">
            {activeModal()}

            <Button
              variant="ghost"
              size="icon"
              onClick={() => closeModal()}
              className="absolute left-2 top-4.5 z-[11] bg-[#ffffffc6] backdrop-blur-2xl"
            >
              <ArrowLeft color="#686868" />
            </Button>
            <ScrollBar orientation="vertical" />
          </ScrollArea>
        </div>
      )}

      <div className="fixed bottom-0 w-full px-4 flex py-3 pb-5.5 bg-[#fffffff6] backdrop-blur-md border-t-1 border-[#0000000d] rounded-t-2xl">
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
        <div
          onClick={() => setActivePanel(Panel.Profile)}
          className="w-full py-2 flex justify-center"
        >
          <Avatar>
            <AvatarFallback
              className={cn(
                "text-white",
                _activeObject == Panel.Profile
                  ? "bg-[#EC6608] "
                  : "bg-[#AAAAAA]"
              )}
            >
              Я
            </AvatarFallback>
          </Avatar>
        </div>
      </div>
    </>
  );
}

export default App;
