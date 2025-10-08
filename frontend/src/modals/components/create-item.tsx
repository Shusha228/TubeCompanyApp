import { Button } from "@/components/ui/button";
import { Input } from "@/components/ui/input";
import { Label } from "@/components/ui/label";
import { Textarea } from "@/components/ui/textarea";
import { useFetchAllItems } from "@/providers/all-items";
import { CreateItemContext } from "@/providers/create-item";
import { useContext, useState } from "react";

export const CreateItemModal = () => {
  const { formData, setFormData, createItem, isLoading, resetForm } =
    useContext(CreateItemContext);
  const { addItem } = useFetchAllItems();
  const [error, setError] = useState<string>("");

  const handleSubmit = async () => {
    setError("");

    // Валидация обязательных полей
    if (!formData.name || !formData.idCat || !formData.gost) {
      setError("Пожалуйста, заполните все обязательные поля");
      return;
    }

    try {
      const createdItem = await createItem();
      if (createdItem) {
        addItem(createdItem);
        resetForm();
        // Здесь можно добавить уведомление об успешном создании
        console.log("Item created successfully:", createdItem);
      } else {
        setError("Ошибка при создании товара");
      }
    } catch {
      setError("Ошибка при создании товара");
    }
  };

  return (
    <div className="w-full h-auto">
      <div className="flex flex-col w-full bg-[#F3F3F3] h-max">
        <div className="flex flex-col pt-6 gap-4 w-full bg-white rounded-b-[12px] pb-4.5">
          <h1 className="font-bold text-xl text-[#686868] text-center w-full">
            Новый товар
          </h1>
          <div className="flex w-full gap-2 px-2 md:px-4"></div>
        </div>
        <div className="w-full h-[10px]"></div>
        <div className="pb-8 bg-white rounded-t-[12px] w-full pt-2.5 md:pt-4.5 px-2">
          <div className="grid grid-cols-1 gap-2 px-2 md:px-4">
            <Label className="font-bold text-md">Основная информация</Label>
            <Input
              placeholder="Категория трубы*"
              value={formData.idCat}
              onChange={(e) => setFormData({ idCat: e.target.value })}
            />
            <Input
              placeholder="Тип товара*"
              value={formData.idTypeNew}
              onChange={(e) => setFormData({ idTypeNew: e.target.value })}
            />
            <Input
              placeholder="Полное наименование позиции*"
              value={formData.name}
              onChange={(e) => setFormData({ name: e.target.value })}
            />
            <Input
              placeholder="Стандарт (ГОСТ/ТУ)*"
              value={formData.gost}
              onChange={(e) => setFormData({ gost: e.target.value })}
            />
            <Label className="font-bold text-md pt-3">
              Технические характеристики
            </Label>
            <Input
              placeholder="Форма длины*"
              value={formData.formOfLength}
              onChange={(e) => setFormData({ formOfLength: e.target.value })}
            />
            <Input
              placeholder="Диаметр, мм*"
              type="number"
              value={formData.diameter}
              onChange={(e) =>
                setFormData({ diameter: Number(e.target.value) })
              }
            />
            <Input
              placeholder="Толщина стенки, мм*"
              type="number"
              value={formData.pipeWallThickness}
              onChange={(e) =>
                setFormData({ pipeWallThickness: Number(e.target.value) })
              }
            />
            <Input
              placeholder="Размер профиля 2"
              type="number"
              value={formData.profileSize2}
              onChange={(e) =>
                setFormData({ profileSize2: Number(e.target.value) })
              }
            />
            <Label className="font-bold text-md pt-3">
              Производство и наличие
            </Label>
            <Input
              placeholder="Производитель*"
              value={formData.manufacturer}
              onChange={(e) => setFormData({ manufacturer: e.target.value })}
            />
            <Input
              placeholder="Марка стали*"
              value={formData.steelGrade}
              onChange={(e) => setFormData({ steelGrade: e.target.value })}
            />
            <Input
              placeholder="Тип производства*"
              value={formData.productionType}
              onChange={(e) => setFormData({ productionType: e.target.value })}
            />
            <Input
              placeholder="Функциональный тип*"
              value={formData.idFunctionType}
              onChange={(e) => setFormData({ idFunctionType: e.target.value })}
            />
            <Input
              placeholder="Статус*"
              value={formData.status}
              onChange={(e) => setFormData({ status: e.target.value })}
            />
            <Input
              placeholder="Коэффициент"
              type="number"
              value={formData.koef}
              onChange={(e) => setFormData({ koef: Number(e.target.value) })}
            />
            <Label className="font-bold text-md pt-3">
              Дополнительная информация
            </Label>
            <Textarea
              className="h-20"
              placeholder="Комментарий/полезные ссылки"
            />
            <Label className="font-bold text-md pt-3">Фото</Label>
            <Input type="file" accept="image/*" />

            {error && <div className="text-red-500 text-sm">{error}</div>}

            <Button
              className="bg-[#EC6608] hover:bg-[#EC6608] active:scale-98"
              onClick={handleSubmit}
              disabled={isLoading}
            >
              {isLoading ? "Создание..." : "Разместить на сайте"}
            </Button>
          </div>
        </div>
      </div>
    </div>
  );
};
