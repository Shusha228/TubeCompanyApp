import { EllipsisVertical } from "lucide-react";
import { Button } from "./button";
import {
  DropdownMenu,
  DropdownMenuContent,
  DropdownMenuItem,
  DropdownMenuTrigger,
} from "./dropdown-menu";
import { Item, ItemContent, ItemMedia, ItemTitle } from "./item";

export const ItemCardForFavorite = () => {
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
            Item
          </ItemTitle>
          <ItemTitle className="text-xs font-medium pl-2 pt-2">
            Остаток на складе: 1200 М / 63.3 т
          </ItemTitle>
        </ItemContent>
      </div>

      <Button
        size="lg"
        className="bg-[#EC6608] hover:bg-[#EC6608] active:scale-98 w-full sm:w-auto cursor-pointer"
      >
        Добавить в корзину
      </Button>
      <DropdownMenu>
        <DropdownMenuTrigger className="absolute top-2 right-2">
          <EllipsisVertical size="18" color="#686868" />
        </DropdownMenuTrigger>
        <DropdownMenuContent>
          <DropdownMenuItem variant="destructive">Удалить</DropdownMenuItem>
        </DropdownMenuContent>
      </DropdownMenu>
    </Item>
  );
};
