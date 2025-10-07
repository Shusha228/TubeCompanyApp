import {
  InputGroup,
  InputGroupAddon,
  InputGroupInput,
} from "@/components/ui/input-group";
import { ItemCardForAll } from "@/components/ui/item-card-all";
import { Spinner } from "@/components/ui/spinner";
import { FetchAllItemsProvider, useFetchAllItems } from "@/providers/all-items";
import { SearchIcon } from "lucide-react";

const AllItemList = () => {
  const { data, isLoading } = useFetchAllItems();

  if (data === undefined || isLoading) {
    return (
      <div className="w-full h-full flex justify-center items-center">
        <Spinner />
      </div>
    );
  }

  return (
    <div>
      {data && data.map((item) => <ItemCardForAll key={item.id} item={item} />)}
    </div>
  );
};

const AllItemsSearch = () => {
  const { setFilters } = useFetchAllItems();
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

export const AllItemsModal = () => {
  return (
    <FetchAllItemsProvider>
      <div className="w-full h-dvh bg-white">
        <div className="flex flex-col w-full bg-[#F3F3F3] h-max">
          <div className="flex flex-col pt-6 gap-4 w-full bg-white rounded-b-[12px] pb-4.5">
            <h1 className="font-bold text-[#686868] text-center w-full">
              Все товары
            </h1>
            <div className="flex w-full gap-2 px-2 md:px-4">
              <AllItemsSearch />
            </div>
          </div>
          <div className="w-full h-[10px]"></div>
          <div className="pb-6 bg-white rounded-t-[12px] h-full w-full pt-2.5 md:pt-4.5">
            <div className="grid grid-cols-1 gap-2 px-2 md:px-4 h-full bg-white">
              <AllItemList />
            </div>
          </div>
        </div>
      </div>
    </FetchAllItemsProvider>
  );
};
