import { StrictMode } from "react";
import { createRoot } from "react-dom/client";
import App from "./App.tsx";
import "./index.css";
import { ActiveModalProvider } from "./providers/active-modal/provider.tsx";
import { ActivePanelProvider } from "./providers/active-panel/provider.tsx";
import { UserProvider } from "./providers/user/provider.tsx";

createRoot(document.getElementById("root")!).render(
  <StrictMode>
    <UserProvider>
      <ActiveModalProvider>
        <ActivePanelProvider>
          <App />
        </ActivePanelProvider>
      </ActiveModalProvider>
    </UserProvider>
  </StrictMode>
);
