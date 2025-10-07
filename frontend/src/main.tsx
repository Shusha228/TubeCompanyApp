import { StrictMode } from "react";
import { createRoot } from "react-dom/client";
import App from "./App.tsx";
import "./index.css";
import { ActiveModalProvider } from "./providers/active-modal/provider.tsx";
import { ActivePanelProvider } from "./providers/active-panel/provider.tsx";
import { CreateOrderProvider } from "./providers/create-order/provider.tsx";
import { FetchFavoritesProvider } from "./providers/favorites/provider.tsx";
import { ModalItemProvider } from "./providers/modal-item/provider.tsx";
import { UserProvider } from "./providers/user/provider.tsx";

createRoot(document.getElementById("root")!).render(
  <StrictMode>
    <UserProvider>
      <CreateOrderProvider>
        <FetchFavoritesProvider>
          <ActiveModalProvider>
            <ModalItemProvider>
              <ActivePanelProvider>
                <App />
              </ActivePanelProvider>
            </ModalItemProvider>
          </ActiveModalProvider>
        </FetchFavoritesProvider>
      </CreateOrderProvider>
    </UserProvider>
  </StrictMode>
);
