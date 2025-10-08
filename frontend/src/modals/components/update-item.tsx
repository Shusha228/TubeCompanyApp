import { Button } from "@/components/ui/button";
import { Input } from "@/components/ui/input";
import { Label } from "@/components/ui/label";
import { Textarea } from "@/components/ui/textarea";
import { useUpdateItem } from "@/providers/update-item";
import { useState } from "react";

export const UpdateItemModal = () => {
  const { item, updateItem } = useUpdateItem();
  const [formData, setFormData] = useState({
    idCat: item?.idCat || "",
    idTypeNew: item?.idTypeNew || "",
    name: item?.name || "",
    gost: item?.gost || "",
    manufacturer: item?.manufacturer || "",
    steelGrade: item?.steelGrade || "",
    diameter: item?.diameter || "",
    pipeWallThickness: item?.pipeWallThickness || "",
    formOfLength: item?.formOfLength || "",
    productionType: item?.productionType || "",
    idFunctionType: item?.idFunctionType || "",
    application: "", // Новое поле для применения
    comment: "", // Новое поле для комментария
  });
  const [isLoading, setIsLoading] = useState(false);

  const handleInputChange = (field: string, value: string) => {
    setFormData((prev) => ({
      ...prev,
      [field]: value,
    }));
  };

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();
    if (!item?.id) return;

    setIsLoading(true);
    try {
      await updateItem({
        idCat: formData.idCat,
        idTypeNew: formData.idTypeNew,
        name: formData.name,
        gost: formData.gost,
        manufacturer: formData.manufacturer,
        steelGrade: formData.steelGrade,
        diameter: Number(formData.diameter),
        pipeWallThickness: Number(formData.pipeWallThickness),
        formOfLength: formData.formOfLength,
        productionType: formData.productionType,
        idFunctionType: formData.idFunctionType,
      });
      // Здесь можно добавить уведомление об успешном обновлении
      console.log("Item updated successfully");
    } catch (error) {
      console.error("Error updating item:", error);
      // Здесь можно добавить уведомление об ошибке
    } finally {
      setIsLoading(false);
    }
  };

  return (
    <div className="w-full h-auto">
      <div className="flex flex-col w-full bg-[#F3F3F3] h-max">
        <div className="flex flex-col pt-6 gap-4 w-full bg-white rounded-b-[12px] pb-4.5">
          <h1 className="font-bold text-xl text-[#686868] text-center w-full">
            Изменить товар
          </h1>
          <div className="flex w-full gap-2 px-2 md:px-4"></div>
        </div>
        <div className="w-full h-[10px]"></div>
        <form onSubmit={handleSubmit}>
          <div className="pb-8 bg-white rounded-t-[12px] w-full pt-2.5 md:pt-4.5 px-2">
            <div className="grid grid-cols-1 gap-2 px-2 md:px-4">
              <Label className="font-bold text-md">Основная информация</Label>
              <Input
                placeholder="Категория трубы*"
                defaultValue={formData.idCat}
                onChange={(e) => handleInputChange("idCat", e.target.value)}
              />
              <Input
                placeholder="Подкатегория*"
                defaultValue={formData.idTypeNew}
                onChange={(e) => handleInputChange("idTypeNew", e.target.value)}
              />
              <Input
                placeholder="Полное наименование позиции*"
                defaultValue={formData.name}
                onChange={(e) => handleInputChange("name", e.target.value)}
              />
              <Input
                placeholder="Стандарт (ГОСТ/ТУ)*"
                defaultValue={formData.gost}
                onChange={(e) => handleInputChange("gost", e.target.value)}
              />
              <Label className="font-bold text-md pt-3">
                Технические характеристики
              </Label>
              <Input
                placeholder="Длина, м*"
                defaultValue={formData.formOfLength}
                onChange={(e) =>
                  handleInputChange("formOfLength", e.target.value)
                }
              />
              <Input
                placeholder="Диаметр, мм*"
                defaultValue={formData.diameter}
                onChange={(e) => handleInputChange("diameter", e.target.value)}
              />
              <Input
                placeholder="Толщина стенки, мм*"
                defaultValue={formData.pipeWallThickness}
                onChange={(e) =>
                  handleInputChange("pipeWallThickness", e.target.value)
                }
              />
              <Label className="font-bold text-md pt-3">
                Производство и наличие
              </Label>
              <Input
                placeholder="Производитель"
                defaultValue={formData.manufacturer}
                onChange={(e) =>
                  handleInputChange("manufacturer", e.target.value)
                }
              />
              <Input
                placeholder="Марка стали"
                defaultValue={formData.steelGrade}
                onChange={(e) =>
                  handleInputChange("steelGrade", e.target.value)
                }
              />
              <Input
                placeholder="Тип производства"
                defaultValue={formData.productionType}
                onChange={(e) =>
                  handleInputChange("productionType", e.target.value)
                }
              />
              <Input
                placeholder="Функциональный тип"
                defaultValue={formData.idFunctionType}
                onChange={(e) =>
                  handleInputChange("idFunctionType", e.target.value)
                }
              />
              <Label className="font-bold text-md pt-3">Применение</Label>
              <Input
                placeholder="Где применяется"
                defaultValue={formData.application}
                onChange={(e) =>
                  handleInputChange("application", e.target.value)
                }
              />
              <Label className="font-bold text-md pt-3">
                Дополнительная информация
              </Label>
              <Textarea
                className="h-20"
                placeholder="Комментарий/полезные ссылки"
                defaultValue={formData.comment}
                onChange={(e) => handleInputChange("comment", e.target.value)}
              />
              <Label className="font-bold text-md pt-3">Фото</Label>
              <Input type="file" accept="image/*" />
              <Button
                type="submit"
                disabled={isLoading}
                className="bg-[#EC6608] hover:bg-[#EC6608] active:scale-98"
              >
                {isLoading ? "Обновление..." : "Обновить товар"}
              </Button>
            </div>
          </div>
        </form>
      </div>
    </div>
  );
};
