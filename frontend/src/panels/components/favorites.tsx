import {
  InputGroup,
  InputGroupAddon,
  InputGroupInput,
} from "@/components/ui/input-group";
import { ItemCardForFavorite } from "@/components/ui/item-card-favorite";
import { useFetchFavorites } from "@/providers/favorites";
import { SearchIcon } from "lucide-react";

export const FavoritesList = () => {
  const { data } = useFetchFavorites();
  return (
    <>
      {data.length == 0 && (
        <div className="flex w-full h-full items-center justify-center">
          Здесь пока ничего нет
        </div>
      )}
      {data.length > 0 &&
        data.map((item) => <ItemCardForFavorite item={item} key={item.id} />)}
    </>
  );
};

const FavoritesSearch = () => {
  const { setFilters } = useFetchFavorites();
  return (
    <InputGroup>
      <InputGroupInput
        placeholder="Поиск"
        onChange={(e) =>
          setFilters({
            search: e.currentTarget.value,
          })
        }
      />
      <InputGroupAddon>
        <SearchIcon />
      </InputGroupAddon>
    </InputGroup>
  );
};

export const FavoritesPanel = () => {
  return (
    <div className="flex flex-col w-full bg-[#F3F3F3] h-max">
      <div className="flex flex-col pt-6 gap-4 w-full bg-white rounded-b-[12px] pb-4.5">
        <h1 className="font-bold text-[#686868] text-center w-full">
          Избранное
        </h1>
        <div className="flex w-full gap-2 px-2 md:px-4">
          <FavoritesSearch />
        </div>
      </div>
      <div className="w-full h-[10px]"></div>
      <div className="pb-24 bg-white rounded-t-[12px] w-full pt-2.5 md:pt-4.5 min-h-full">
        <div className="grid grid-cols-1 gap-2 px-2 md:px-4">
          <FavoritesList />
        </div>
      </div>
    </div>
  );
};
