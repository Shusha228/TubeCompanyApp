import { getURL } from "@/api";
import { cn } from "@/lib/utils";
import { Modal } from "@/modals/models/modal";
import type { Item as ItemType } from "@/models/item";
import { Panel } from "@/panels";
import { useActiveModal } from "@/providers/active-modal";
import { useActivePanel } from "@/providers/active-panel";
import { useFetchFavorites } from "@/providers/favorites";
import { useModalItem } from "@/providers/modal-item";
import { useUser } from "@/providers/user";
import { EllipsisVertical } from "lucide-react";
import { useState } from "react";
import { Button } from "./button";
import { Drawer, DrawerContent, DrawerTitle, DrawerTrigger } from "./drawer";
import {
  DropdownMenu,
  DropdownMenuContent,
  DropdownMenuItem,
  DropdownMenuTrigger,
} from "./dropdown-menu";
import { Input } from "./input";
import { Item, ItemContent, ItemMedia, ItemTitle } from "./item";

const _addItemInCart = (
  userId: number,
  item: ItemType,
  quantity: number,
  isInMeters: boolean
) => {
  return fetch(getURL(`Cart/${userId}/items`), {
    method: "POST",
    headers: {
      "Content-Type": "application/json;",
    },
    body: JSON.stringify({
      stockId: item.remnants[0].idStock,
      productId: item.id,
      productName: item.name,
      quantity: quantity,
      isInMeters: isInMeters,
    }),
  });
};

export const ItemCardForFavorite = ({ item }: { item: ItemType }) => {
  const { toggleFavorite } = useFetchFavorites();
  const { telegramId } = useUser();
  const { closeModal } = useActiveModal();
  const { setActivePanel } = useActivePanel();
  const activeModal = useActiveModal();
  const modalItem = useModalItem();

  const [quantity, _setQuantity] = useState(0);
  const [isInMeter, setInMeter] = useState<boolean>(false);

  const setQuantity = (value: number, inMeter: boolean) => {
    _setQuantity(value);
    setInMeter(inMeter);
  };
  const showModal = () => {
    modalItem.showModal(item.id);
    activeModal.showModal(Modal.Item);
  };

  const addInCart = () => {
    if (quantity == 0) return;
    if (Number.isNaN(quantity)) return;

    if (item !== undefined) {
      _addItemInCart(telegramId, item, quantity, isInMeter).then(() => {
        closeModal();
        setActivePanel(Panel.Cart);
      });
    }
  };

  return (
    <Item
      variant="outline"
      className="relative p-0 w-full gap-2 pb-2 border-0 bg-none sm:flex-row flex-col sm:justify-between"
    >
      <div className="w-full sm:w-auto h-auto flex gap-2">
        <ItemMedia
          variant="image"
          className="w-[96px] h-auto"
          onClick={showModal}
        >
          <img src="/image.png" className="w-full h-auto" />
        </ItemMedia>
        <ItemContent className="w-full justify-center">
          <ItemTitle className="flex w-full font-medium text-[14px] max-w-[400px] line-clamp-3 uppercase px-2 text-ellipsis wrap-break-word">
            {item.name}
          </ItemTitle>
          <ItemTitle className="text-xs font-medium pl-2 pt-2">
            Остаток на складе: {item.remnants[0].inStockM} М /{" "}
            {item.remnants[0].inStockT} т
          </ItemTitle>
        </ItemContent>
      </div>
      <Drawer>
        <DrawerTrigger asChild={true}>
          <Button
            size="lg"
            className="bg-[#EC6608] hover:bg-[#EC6608] active:scale-98 w-full sm:w-auto cursor-pointer"
          >
            Добавить в корзину
          </Button>
        </DrawerTrigger>
        <DrawerContent>
          <DrawerTitle className="text-[14px] fond-medium opacity-50 pl-4 pt-2">
            Впишите количество в одно из полей
          </DrawerTitle>
          <div className="flex gap-2 flex-col px-4 pt-4 pb-8">
            <Input
              placeholder="Введите длину, м"
              type="number"
              className={cn(
                quantity == 0 ? "border-destructive" : "border-input"
              )}
              value={isInMeter ? quantity : 0}
              onChange={(e) => setQuantity(Number(e.currentTarget.value), true)}
            />
            <Input
              className={cn(
                quantity == 0 ? "border-destructive" : "border-input"
              )}
              placeholder="Введите длину, м"
              value={!isInMeter ? quantity : 0}
              type="number"
              onChange={(e) =>
                setQuantity(Number(e.currentTarget.value), false)
              }
            />
            <Button
              size="lg"
              className="bg-[#EC6608] hover:bg-[#EC6608] active:scale-98"
              onClick={() => addInCart()}
            >
              Добавить в корзину
            </Button>
          </div>
        </DrawerContent>
      </Drawer>

      <DropdownMenu>
        <DropdownMenuTrigger className="absolute top-2 right-2">
          <EllipsisVertical size="18" color="#686868" />
        </DropdownMenuTrigger>
        <DropdownMenuContent>
          <DropdownMenuItem
            variant="destructive"
            onClick={() => toggleFavorite(item)}
          >
            Удалить
          </DropdownMenuItem>
        </DropdownMenuContent>
      </DropdownMenu>
    </Item>
  );
};
