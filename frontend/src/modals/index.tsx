import { AllItemsModal } from "./components/all-items";
import { CreateItemModal } from "./components/create-item";
import { ItemModal } from "./components/item";
import { Modal } from "./models/modal";

const ModalComponentDict = {
  [Modal.AllItems]: AllItemsModal,
  [Modal.Item]: ItemModal,
  [Modal.NewItem]: CreateItemModal,
} as const;

export const getComponentByModal = (modal: Modal | undefined) => {
  if (modal === undefined) {
    return undefined;
  }
  return ModalComponentDict[modal];
};
