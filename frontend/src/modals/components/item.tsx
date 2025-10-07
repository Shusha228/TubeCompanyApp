import { getURL } from "@/api";
import { Button } from "@/components/ui/button";
import {
  Drawer,
  DrawerContent,
  DrawerTitle,
  DrawerTrigger,
} from "@/components/ui/drawer";
import { Input } from "@/components/ui/input";
import { Spinner } from "@/components/ui/spinner";
import { cn } from "@/lib/utils";
import type { Item } from "@/models/item";
import { Panel } from "@/panels";
import { useActiveModal } from "@/providers/active-modal";
import { useActivePanel } from "@/providers/active-panel";
import { useFetchFavorites } from "@/providers/favorites";
import { useModalItem } from "@/providers/modal-item";
import { useUser } from "@/providers/user";
import { useState } from "react";

const _addItemInCart = (
  userId: number,
  item: Item,
  quantity: number,
  isInMeters: boolean
) => {
  fetch(getURL(`Cart/${userId}/items`), {
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

export const ItemModal = () => {
  const { closeModal } = useActiveModal();
  const { setActivePanel } = useActivePanel();
  const { hasInFavorite, toggleFavorite } = useFetchFavorites();
  const { data, isLoading } = useModalItem();
  const { telegramId } = useUser();
  const [quantity, _setQuantity] = useState(0);
  const [isInMeter, setInMeter] = useState<boolean>(false);

  const setQuantity = (value: number, inMeter: boolean) => {
    _setQuantity(value);
    setInMeter(inMeter);
  };

  const addInCart = () => {
    if (quantity == 0) return;
    if (Number.isNaN(quantity)) return;

    if (data !== undefined) {
      _addItemInCart(telegramId, data, quantity, isInMeter);
      closeModal();
      setActivePanel(Panel.Cart);
    }
  };

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
              Остаток на складе:{" "}
              <b>
                {data.remnants[0].inStockM} М / {data.remnants[0].inStockT} т
              </b>
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
              <DrawerTrigger asChild={true}>
                <Button
                  size="lg"
                  className="bg-[#EC6608] hover:bg-[#EC6608] active:scale-98 w-full"
                >
                  Заказать
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
                    onChange={(e) =>
                      setQuantity(Number(e.currentTarget.value), true)
                    }
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
