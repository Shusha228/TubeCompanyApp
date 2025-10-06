import {
  InputGroup,
  InputGroupAddon,
  InputGroupInput,
} from "@/components/ui/input-group";
import { ItemCardForAll } from "@/components/ui/item-card-all";
import { Spinner } from "@/components/ui/spinner";
import { SearchIcon } from "lucide-react";
import type { JSX } from "react";

const useAllItem = () => ({
  isLoading: false,
  data: [...Array(24).keys()],
});

const AllItemList = () => {
  const { data, isLoading } = useAllItem();

  if (isLoading) {
    return (
      <div className="w-full h-full flex justify-center items-center">
        <Spinner />
      </div>
    );
  }

  return (
    <div>
      {data.map((el) => (
        <ItemCardForAll key={el} />
      ))}
    </div>
  );
};

const AllItemProvider = ({ children }: { children: JSX.Element }) => (
  <>{children}</>
);

export const AllItemsModal = () => {
  return (
    <AllItemProvider>
      <div className="w-full h-auto">
        <div className="flex flex-col w-full bg-[#F3F3F3] h-max">
          <div className="flex flex-col pt-6 gap-4 w-full bg-white rounded-b-[12px] pb-4.5">
            <h1 className="font-bold text-[#686868] text-center w-full">
              Все товары
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
          <div className="pb-6 bg-white rounded-t-[12px] w-full pt-2.5 md:pt-4.5">
            <div className="grid grid-cols-1 gap-2 px-2 md:px-4">
              <AllItemList />
            </div>
          </div>
        </div>
      </div>
    </AllItemProvider>
  );
};
