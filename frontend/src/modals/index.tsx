import { AllItemsModal } from "./components/all-items";
import { CreateItemModal } from "./components/create-item";
import { CreateOrderModal } from "./components/create-order";
import { ItemModal } from "./components/item";
import { UpdateItemModal } from "./components/update-item";
import { Modal } from "./models/modal";

const ModalComponentDict = {
  [Modal.AllItems]: AllItemsModal,
  [Modal.Item]: ItemModal,
  [Modal.NewItem]: CreateItemModal,
  [Modal.CreateOrder]: CreateOrderModal,
  [Modal.UpdateItem]: UpdateItemModal,
} as const;

export const getComponentByModal = (modal: Modal | undefined) => {
  if (modal === undefined) {
    return undefined;
  }
  return ModalComponentDict[modal];
};
