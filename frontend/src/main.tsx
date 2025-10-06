import { StrictMode } from "react";
import { createRoot } from "react-dom/client";
import App from "./App.tsx";
import "./index.css";
import { ActiveModalProvider } from "./providers/active-modal/provider.tsx";
import { ActivePanelProvider } from "./providers/active-panel/provider.tsx";

createRoot(document.getElementById("root")!).render(
  <StrictMode>
    <ActiveModalProvider>
      <ActivePanelProvider>
        <App />
      </ActivePanelProvider>
    </ActiveModalProvider>
  </StrictMode>
);
