import { getURL } from "@/api";
import { Modal } from "@/modals/models/modal";
import { useActiveModal } from "@/providers/active-modal";
import { useCreateOrder } from "@/providers/create-order";
import { useFetchShoppingCart } from "@/providers/shopping-cart";
import type { ShoppingCartItem } from "@/providers/shopping-cart/models/cart-item";
import { useUser } from "@/providers/user";
import { EllipsisVertical, Minus, Plus } from "lucide-react";
import { useState } from "react";
import { Button } from "./button";
import {
  Drawer,
  DrawerClose,
  DrawerContent,
  DrawerFooter,
  DrawerTrigger,
} from "./drawer";
import {
  DropdownMenu,
  DropdownMenuContent,
  DropdownMenuItem,
  DropdownMenuTrigger,
} from "./dropdown-menu";
import { Input } from "./input";
import { InputGroup, InputGroupAddon, InputGroupInput } from "./input-group";
import { Item, ItemContent, ItemMedia, ItemTitle } from "./item";

const _updateCount = (
  telegramId: number,
  stockId: string,
  productId: number,
  newQuantity: number
) => {
  fetch(getURL(`Cart/${telegramId}/items/${stockId}/${productId}`), {
    method: "PUT",
    headers: {
      "Content-Type": "application/json",
    },
    body: JSON.stringify({
      newQuantity,
    }),
  });
};

export const ItemCardForShoppingCart = ({
  item,
}: {
  item: ShoppingCartItem;
}) => {
  const { telegramId } = useUser();
  const { deleteItem } = useFetchShoppingCart();
  const [quantity, _setQuantity] = useState(item.quantity);
  const [inputValue, setInputValue] = useState(quantity);

  const updateQuantity = (value: number) => {
    _updateCount(telegramId, item.stockId, item.productId, value);
    _setQuantity(value);
    setInputValue(0);
  };

  const { showModal } = useActiveModal();
  const { setItem } = useCreateOrder();

  const buy = () => {
    setItem(item);
    showModal(Modal.CreateOrder);
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
          <ItemTitle className="text-md font-medium text-[18px] uppercase pl-2">
            {item.productName}
          </ItemTitle>
          <InputGroup className="w-fit mt-2">
            <Drawer>
              <DrawerTrigger>
                <InputGroupInput
                  className="w-[47px] text-center"
                  value={quantity}
                  onChange={() => {}}
                />
              </DrawerTrigger>
              <DrawerContent>
                <DrawerFooter>
                  <Input
                    placeholder="1200, м"
                    defaultValue={quantity}
                    value={inputValue}
                    type="number"
                    onChange={(e) =>
                      setInputValue(Number(e.currentTarget.value))
                    }
                  />
                  <DrawerClose asChild={true}>
                    <Button
                      className="bg-[#EC6608] hover:bg-[#EC6608] active:scale-98 w-full sm:w-auto cursor-pointer"
                      onClick={() => updateQuantity(inputValue)}
                    >
                      Сохранить изменения
                    </Button>
                  </DrawerClose>
                </DrawerFooter>
              </DrawerContent>
            </Drawer>
            <InputGroupAddon>
              <Button
                size="icon-sm"
                variant="ghost"
                onClick={() => updateQuantity(quantity - 1)}
              >
                <Minus />
              </Button>
            </InputGroupAddon>
            <InputGroupAddon align="inline-end">
              <Button
                size="icon-sm"
                variant="ghost"
                onClick={() => updateQuantity(quantity + 1)}
              >
                <Plus />
              </Button>
            </InputGroupAddon>
          </InputGroup>
        </ItemContent>
      </div>

      <Button
        size="lg"
        className="bg-[#EC6608] hover:bg-[#EC6608] active:scale-98 w-full sm:w-auto cursor-pointer"
        onClick={buy}
      >
        Перейти к оформлению
      </Button>
      <DropdownMenu>
        <DropdownMenuTrigger className="absolute top-2 right-2">
          <EllipsisVertical size="18" color="#686868" />
        </DropdownMenuTrigger>
        <DropdownMenuContent>
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
