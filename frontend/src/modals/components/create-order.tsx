import { Button } from "@/components/ui/button";
import { Input } from "@/components/ui/input";
import { Label } from "@/components/ui/label";
import { Textarea } from "@/components/ui/textarea";

export const CreateOrderModal = () => {
  return (
    <div className="w-full h-auto">
      <div className="flex flex-col w-full bg-[#F3F3F3] h-max">
        <div className="flex flex-col pt-6 gap-4 w-full bg-white rounded-b-[12px] pb-4.5">
          <h1 className="font-bold text-xl text-[#686868] text-center w-full">
            Оформление заказа
          </h1>
          <div className="flex w-full gap-2 px-2 md:px-4"></div>
        </div>
        <div className="w-full h-[10px]"></div>
        <div className="pb-8 bg-white rounded-t-[12px] w-full pt-2.5 md:pt-4.5 px-2">
          <div className="grid grid-cols-1 gap-2 px-2 md:px-4">
            <Label className="font-bold text-md">Основная информация</Label>
            <Input placeholder="Категория трубы*" />
            <Input placeholder="Подкатегория*" />
            <Input placeholder="Полное наименование позиции*" />
            <Input placeholder="Стандарт (ГОСТ/ТУ)*" />
            <Label className="font-bold text-md pt-3">
              Технические характеристики
            </Label>
            <Input placeholder="Длина, м*" />
            <Input placeholder="Диаметр, мм*" />
            <Input placeholder="Толщина стенки, мм*" />
            <Label className="font-bold text-md pt-3">
              Производство и наличие
            </Label>
            <Label className="font-bold text-md pt-3">Применение</Label>
            <Input placeholder="Где применяется" />
            <Label className="font-bold text-md pt-3">
              Дополнительная информация
            </Label>
            <Textarea
              className="h-20"
              placeholder="Комментарий/полезные ссылки"
            />
            <Label className="font-bold text-md pt-3">Фото</Label>
            <Input type="file" accept="image/*" />
            <Button className="bg-[#EC6608] hover:bg-[#EC6608] active:scale-98">
              Разместить на сайте
            </Button>
          </div>
        </div>
      </div>
    </div>
  );
};
