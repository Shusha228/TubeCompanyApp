import { StrictMode } from "react";
import { createRoot } from "react-dom/client";
import App from "./App.tsx";
import "./index.css";
import { ActivePanelProvider } from "./providers/active-panel/provider.tsx";

createRoot(document.getElementById("root")!).render(
  <StrictMode>
    <ActivePanelProvider>
      <App />
    </ActivePanelProvider>
  </StrictMode>
);
