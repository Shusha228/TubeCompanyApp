import type { Modal } from "@/modals/models/modal";
import { createContext, type JSX } from "react";

export interface ActiveModal {
  activeModal: (() => JSX.Element) | undefined;
  _activeObject: Modal | undefined;
  showModal: (modal: Modal) => void;
  closeModal: () => void;
}

export const ActiveModalContext = createContext({} as ActiveModal);
