import { useFetchShoppingCart } from "@/providers/shopping-cart";
import type { ShoppingCartItem } from "@/providers/shopping-cart/models/cart-item";
import { EllipsisVertical, Minus, Plus } from "lucide-react";
import { Button } from "./button";
import { Drawer, DrawerContent, DrawerFooter, DrawerTrigger } from "./drawer";
import {
  DropdownMenu,
  DropdownMenuContent,
  DropdownMenuItem,
  DropdownMenuTrigger,
} from "./dropdown-menu";
import { Input } from "./input";
import { InputGroup, InputGroupAddon, InputGroupInput } from "./input-group";
import { Item, ItemContent, ItemMedia, ItemTitle } from "./item";

export const ItemCardForShoppingCart = ({
  item,
}: {
  item: ShoppingCartItem;
}) => {
  const { deleteItem } = useFetchShoppingCart();
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
            {item.product.name}
          </ItemTitle>
          <InputGroup className="w-fit mt-2">
            <Drawer>
              <DrawerTrigger>
                <InputGroupInput
                  className="w-[47px] text-center"
                  defaultValue={item.quantity}
                />
              </DrawerTrigger>
              <DrawerContent>
                <DrawerFooter>
                  <Input placeholder="1200, м" defaultValue={item.quantity} />
                  <Button className="bg-[#EC6608] hover:bg-[#EC6608] active:scale-98 w-full sm:w-auto cursor-pointer">
                    Сохранить изменения
                  </Button>
                </DrawerFooter>
              </DrawerContent>
            </Drawer>
            <InputGroupAddon>
              <Button size="icon-sm" variant="ghost">
                <Minus />
              </Button>
            </InputGroupAddon>
            <InputGroupAddon align="inline-end">
              <Button size="icon-sm" variant="ghost">
                <Plus />
              </Button>
            </InputGroupAddon>
          </InputGroup>
        </ItemContent>
      </div>

      <Button
        size="lg"
        className="bg-[#EC6608] hover:bg-[#EC6608] active:scale-98 w-full sm:w-auto cursor-pointer"
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
