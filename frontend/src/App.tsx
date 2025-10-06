import "./App.css";
import { useActivePanel } from "./providers/active-panel";

function App() {
  const { activePanel } = useActivePanel();
  return activePanel();
}

export default App;
