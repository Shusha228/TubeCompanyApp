import { Modal } from "@/modals/models/modal";
import type { Item as ItemType } from "@/models/item";
import { useActiveModal } from "@/providers/active-modal";
import { useFetchAllItems } from "@/providers/all-items";
import { useModalItem } from "@/providers/modal-item";
import { useUpdateItem } from "@/providers/update-item";
import { EllipsisVertical } from "lucide-react";
import { Button } from "./button";
import {
  DropdownMenu,
  DropdownMenuContent,
  DropdownMenuItem,
  DropdownMenuTrigger,
} from "./dropdown-menu";
import { Item, ItemContent, ItemMedia, ItemTitle } from "./item";

export const ItemCardForAll = ({ item }: { item: ItemType }) => {
  const { deleteItem } = useFetchAllItems();
  const activeModal = useActiveModal();
  const modalItem = useModalItem();
  const updateItem = useUpdateItem();

  const goToItem = () => {
    modalItem.showModal(item.id);
    activeModal.showModal(Modal.Item);
  };

  const goToUpdate = () => {
    updateItem.setItem(item);
    activeModal.showModal(Modal.UpdateItem);
  };

  return (
    <Item
      variant="outline"
      className="relative p-0 w-full gap-2 pb-2 border-0 bg-none sm:flex-row flex-col sm:justify-between"
    >
      <div className="w-full sm:w-auto h-auto flex gap-2">
        <ItemMedia variant="image" className="w-[96px] h-auto">
          <img src="/image.png" className="w-full h-auto" />
        </ItemMedia>
        <ItemContent className="w-full justify-center">
          <ItemTitle className="flex w-full font-medium text-[14px] max-w-[400px] line-clamp-3 uppercase px-2 text-ellipsis wrap-break-word">
            {item.name}
          </ItemTitle>
          <ItemTitle className="flex w-full font-normal opacity-70 text-[12px] max-w-[400px] line-clamp-3 px-2 text-ellipsis wrap-break-word">
            Применяются для крепления нефтяных и газовых скважин в процессе их
            строительства и эксплуатации.
          </ItemTitle>
        </ItemContent>
      </div>

      <Button className="bg-[#EC6608] hover:bg-[#EC6608] active:scale-98 w-full sm:w-auto cursor-pointer">
        Добавить в корзину
      </Button>
      <DropdownMenu>
        <DropdownMenuTrigger className="absolute top-2 right-2">
          <EllipsisVertical size="18" color="#686868" />
        </DropdownMenuTrigger>
        <DropdownMenuContent>
          <DropdownMenuItem onClick={goToItem}>
            Перейти к товару
          </DropdownMenuItem>
          <DropdownMenuItem onClick={goToUpdate}>
            Редактировать
          </DropdownMenuItem>
          <DropdownMenuItem
            variant="destructive"
            onClick={() => deleteItem(item)}
          >
            Удалить
          </DropdownMenuItem>
        </DropdownMenuContent>
      </DropdownMenu>
    </Item>
  );
};
