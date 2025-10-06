import {
  InputGroup,
  InputGroupAddon,
  InputGroupInput,
} from "@/components/ui/input-group";
import { ItemCardForFavorite } from "@/components/ui/item-card-favorite";
import { SearchIcon } from "lucide-react";

export const FavoritesPanel = () => {
  return (
    <div className="flex flex-col w-full bg-[#F3F3F3] h-max">
      <div className="flex flex-col pt-6 gap-4 w-full bg-white rounded-b-[12px] pb-4.5">
        <h1 className="font-bold text-[#686868] text-center w-full">
          Избранное
        </h1>
        <div className="flex w-full gap-2 px-2 md:px-4">
          <InputGroup>
            <InputGroupInput placeholder="Поиск" />
            <InputGroupAddon>
              <SearchIcon />
            </InputGroupAddon>
          </InputGroup>
        </div>
      </div>
      <div className="w-full h-[10px]"></div>
      <div className="pb-18 bg-white rounded-t-[12px] w-full pt-2.5 md:pt-4.5">
        <div className="grid grid-cols-1 gap-2 px-2 md:px-4">
          {[...Array(24).keys()].map((el) => (
            <ItemCardForFavorite key={el} />
          ))}
        </div>
      </div>
    </div>
  );
};
