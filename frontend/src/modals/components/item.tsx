import { getURL } from "@/api";
import { Button } from "@/components/ui/button";
import { Drawer, DrawerContent, DrawerTrigger } from "@/components/ui/drawer";
import { Input } from "@/components/ui/input";
import { Spinner } from "@/components/ui/spinner";
import type { Item } from "@/models/item";
import { useFetchFavorites } from "@/providers/favorites";
import { useModalItem } from "@/providers/modal-item";
import { useUser } from "@/providers/user";

const addItemInCart = (userId: number, item: Item) => {
  fetch(getURL(`/Cart/${userId}/items`), {
    method: "POST",
    body: JSON.stringify({
      stockId: "",
      productId: item.id,
      productName: item.name,
      quantity: 20,
      isInMeters: true,
      unitPrice: 200,
      finalPrice: 2000,
    }),
  });
};

export const ItemModal = () => {
  const { hasInFavorite, toggleFavorite } = useFetchFavorites();
  const { data, isLoading } = useModalItem();
  const { telegramId } = useUser();

  if (data === undefined || isLoading) {
    return <Spinner />;
  }

  return (
    <div className="w-full min-h-dvh h-auto">
      <div className="flex flex-col w-full bg-[#F3F3F3] h-max">
        <div className="flex flex-col sm:flex-row pt-16 gap-4 w-full bg-white rounded-b-[12px] pb-4.5 px-2">
          <img
            src="image.png"
            className="w-full h-auto max-h-[35vh] rounded-[8px] object-cover"
          />
          <div className="w-full justify-center flex flex-col">
            <h1 className="font-bold text-md w-full">{data.name}</h1>
            <p className="text-sm">
              Трубы стальные бесшовные передельные для производства муфт к
              обсадным трубам
            </p>
          </div>
        </div>
        <div className="w-full h-[10px]"></div>
        <div className="pb-9 bg-white rounded-t-[12px] w-full pt-4 md:pt-4.5">
          <div className="grid grid-cols-1 gap-1 px-2 md:px-4">
            <div className="w-full flex gap-2">
              <span className="w-full text-[#686868]">Стандарт (ГОСТ/ТУ):</span>
              <span className="w-full text-[#000]">{data.gost}</span>
            </div>
            <div className="w-full h-[1px] bg-[#e8e8e8]"></div>
            <div className="w-full flex gap-2">
              <span className="w-full text-[#686868]">
                Форма поставки по длине
              </span>
              <span className="w-full text-[#000]">{data.formOfLength}</span>
            </div>
            <div className="w-full h-[1px] bg-[#e8e8e8]"></div>
            <div className="w-full flex gap-2">
              <span className="w-full text-[#686868]">Диаметр, мм</span>
              <span className="w-full text-[#000]">{data.diameter}</span>
            </div>
            <div className="w-full h-[1px] bg-[#e8e8e8]"></div>
            <div className="w-full flex gap-2">
              <span className="w-full text-[#686868]">Толщина стенки, мм</span>
              <span className="w-full text-[#000]">
                {data.pipeWallThickness}
              </span>
            </div>
            <div className="w-full h-[1px] bg-[#e8e8e8]"></div>
            <div className="w-full flex gap-2">
              <span className="w-full text-[#686868]">Завод производитель</span>
              <span className="w-full text-[#000]">{data.manufacturer}</span>
            </div>
          </div>
          <div className="w-full px-2">
            <div className="bg-[#ffebeb] font-medium rounded-2xl p-3 my-3 text-md text-[#ff1e1e] uppercase">
              Остаток на складе: <b>1200 М / 63,6 т</b>
            </div>
          </div>
          <p className="m-2 my-3">
            Применяются для крепления нефтяных и газовых скважин в процессе их
            строительства и эксплуатации.
            <br />
            <br />С вариантом использования обсадных труб, выпускаемых ТМК, вы
            можете познакомиться, посмотрев видеоролик по следующей ссылке:-
            <a
              href="https://rutube.ru/video/6b40c4af15474759ae019fee69f6f480/?r=wd"
              target="_blank"
            >
              Бурение на обсадной колонне
            </a>
          </p>
          <div className="w-full flex flex-col gap-2 px-2">
            <Drawer>
              <DrawerTrigger>
                <Button
                  size="lg"
                  className="bg-[#EC6608] hover:bg-[#EC6608] active:scale-98 w-full"
                >
                  Заказать
                </Button>
              </DrawerTrigger>
              <DrawerContent>
                <div className="flex gap-2 flex-col px-4 pt-4 pb-8">
                  <Input placeholder="Введите длину, м" />
                  <Input placeholder="Введите длину, м" />
                  <Button
                    size="lg"
                    className="bg-[#EC6608] hover:bg-[#EC6608] active:scale-98"
                    onClick={() => addItemInCart(telegramId, data)}
                  >
                    Добавить в корзину
                  </Button>
                </div>
              </DrawerContent>
            </Drawer>
            <Button
              variant="secondary"
              size="lg"
              className="active:scale-98"
              onClick={() => toggleFavorite(data)}
            >
              {hasInFavorite(data.id)
                ? "Удалить из избранного"
                : "Добавить в избранное"}
            </Button>
          </div>
        </div>
      </div>
    </div>
  );
};
