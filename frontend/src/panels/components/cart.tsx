import {
  InputGroup,
  InputGroupAddon,
  InputGroupInput,
} from "@/components/ui/input-group";
import { ItemCardForShoppingCart } from "@/components/ui/item-card-cart";
import { Spinner } from "@/components/ui/spinner";
import { useObservable } from "@/hooks/observer";
import {
  FetchShoppingCartProvider,
  useFetchShoppingCart,
} from "@/providers/shopping-cart";
import { SearchIcon } from "lucide-react";
import { useRef } from "react";

export const ShoppingCartList = () => {
  const { data, isLoading, next, hasNext } = useFetchShoppingCart();
  const observableElement = useRef<HTMLDivElement>(null);
  console.log(data);

  useObservable({
    ref: observableElement,
    onIntersect: () => next(),
  });
  return (
    <>
      {data.map((item) => (
        <ItemCardForShoppingCart
          item={item}
          key={`${item.stockId}-${item.productId}`}
        />
      ))}
      {isLoading && (
        <div className="w-full flex justify-center">
          <Spinner />
        </div>
      )}
      {data.length == 0 && (
        <div className="w-full text-center">Здесь пока пусто!</div>
      )}
      {data.length >= 20 && !isLoading && hasNext && (
        <div ref={observableElement}></div>
      )}
    </>
  );
};

const ShoppingCartSearch = () => {
  const { setFilters } = useFetchShoppingCart();
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

export const CartPanel = () => {
  return (
    <FetchShoppingCartProvider>
      <div className="flex flex-col w-full bg-[#F3F3F3] h-max">
        <div className="flex flex-col pt-6 gap-4 w-full bg-white rounded-b-[12px] pb-4.5">
          <h1 className="font-bold text-[#686868] text-center w-full">
            Корзина
          </h1>
          <div className="flex w-full gap-2 px-2 md:px-4">
            <ShoppingCartSearch />
          </div>
        </div>
        <div className="w-full h-[10px]"></div>
        <div className="pb-18 bg-white rounded-t-[12px] w-full pt-2.5 md:pt-4.5">
          <div className="grid grid-cols-1 gap-4 px-2 md:px-4">
            <ShoppingCartList />
          </div>
        </div>
      </div>
    </FetchShoppingCartProvider>
  );
};
