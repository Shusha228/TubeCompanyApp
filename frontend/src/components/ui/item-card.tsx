import { Modal } from "@/modals/models/modal";
import type { Item as ItemType } from "@/models/item";
import { useActiveModal } from "@/providers/active-modal";
import { useModalItem } from "@/providers/modal-item";
import { Item, ItemContent, ItemMedia, ItemTitle } from "./item";

export const ItemCard = ({ item }: { item: ItemType }) => {
  const activeModal = useActiveModal();
  const modalItem = useModalItem();

  const showModal = () => {
    modalItem.showModal(item.id);
    activeModal.showModal(Modal.Item);
  };

  return (
    <Item
      onClick={() => showModal()}
      variant="outline"
      className="p-0 w-full gap-2 pb-2 border-0 bg-none cursor-pointer"
    >
      <ItemMedia variant="image" className="w-full h-auto">
        <img src="/image.png" className="w-full h-auto" />
      </ItemMedia>
      <ItemContent className="w-full">
        <ItemTitle className="text-md uppercase pl-2 w-full overflow-hidden">
          {item.name.split(", ").slice(1, 5).join(" ")}
        </ItemTitle>
      </ItemContent>
    </Item>
  );
};
