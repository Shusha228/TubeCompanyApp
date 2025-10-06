import { Badge } from "@/components/ui/badge";
import { Button } from "@/components/ui/button";
import { Checkbox } from "@/components/ui/checkbox";
import {
  Drawer,
  DrawerClose,
  DrawerContent,
  DrawerFooter,
  DrawerTitle,
  DrawerTrigger,
} from "@/components/ui/drawer";
import {
  InputGroup,
  InputGroupAddon,
  InputGroupButton,
  InputGroupInput,
} from "@/components/ui/input-group";
import { ItemCard } from "@/components/ui/item-card";
import { Label } from "@/components/ui/label";
import { ScrollArea, ScrollBar } from "@/components/ui/scroll-area";
import {
  Select,
  SelectContent,
  SelectItem,
  SelectTrigger,
  SelectValue,
} from "@/components/ui/select";
import { City } from "@/models/city";
import { FilterIcon, SearchIcon, X } from "lucide-react";
import { useMemo, useRef, useState } from "react";

const categories = [
  {
    IDType: "a58d54b6-25ea-4818-afc9-1faa8c630d9a",
    Type: "Бесшовные холоднодеформированные",
    IDParentType: "",
  },
  {
    IDType: "d21de9df-f3dd-442f-8b9f-89d8f1865698",
    Type: "Прямошовные",
    IDParentType: "",
  },
  {
    IDType: "4ae51348-d619-4bf2-a35f-1c6931b68bf1",
    Type: "Холоднокатаные",
    IDParentType: "",
  },
  {
    IDType: "403389a6-3f81-4cd0-bf1d-e43fe6adaa48",
    Type: "Горячекатаные",
    IDParentType: "",
  },
];

const checkBoxes = [
  {
    id: "1",
    name: "Трубы нарезные OCTG",
    sub: [],
  },
  {
    id: "2",
    name: "Трубы нефтегазопроводные",
    sub: [
      {
        id: "3",
        name: "Бесшовные горячедеформированные",
        sub: [],
      },
      {
        id: "4",
        name: "Сварные",
        sub: [],
      },
      {
        id: "5",
        name: "Повышенной коррозионной стойкости",
        sub: [],
      },
    ],
  },
  {
    id: "6",
    name: "Трубы сварные большого диаметра",
    sub: [],
  },
  {
    id: "7",
    name: "Нержавеющие трубы",
    sub: [],
  },
  {
    id: "8",
    name: "Бесшовные трубы промышленного назначения",
    sub: [],
  },
  {
    id: "9",
    name: "Сварные трубы промышленного назначения",
    sub: [],
  },
  {
    id: "10",
    name: "Антикоррозионные покрытия на трубы",
    sub: [],
  },
  {
    id: "11",
    name: "Заготовка непрерывнолитая",
    sub: [],
  },
];

export const MainPanel = () => {
  const [category, setCategory] = useState<string>();
  const [activeCheckboxes, setActiveCheckBoxes] = useState<string[]>([]);
  const [citySearch, _setCitySearch] = useState<string>("");
  const [, setActiveCity] = useState<City>();
  const citySearchInput = useRef<HTMLInputElement>(null);
  const [allowDeleteCity, setAllowDeleteCity] = useState<boolean>(false);
  const findedCities = useMemo(
    () =>
      citySearch.length > 2 && !allowDeleteCity
        ? Object.values(City)
            .filter((el) =>
              el.toLocaleLowerCase().includes(citySearch.toLocaleLowerCase())
            )
            .slice(0, 3)
        : [],
    [citySearch, allowDeleteCity]
  );

  const setCitySearch = (value: string) => {
    _setCitySearch(value);
    setAllowDeleteCity(false);
  };

  const setSearchableCity = (activeCity: City) => {
    citySearchInput.current?.blur();
    _setCitySearch(activeCity);
    setAllowDeleteCity(true);
    setActiveCity(activeCity);
  };

  const setSearchableCityNull = () => {
    citySearchInput.current?.focus();
    _setCitySearch("");
    setAllowDeleteCity(false);
    setActiveCity(undefined);
  };

  const toggleCheckBox = (checkbox: string) => {
    if (activeCheckboxes.includes(checkbox)) {
      const subItems = checkBoxes
        .find((el) => el.id == checkbox)
        ?.sub.map((el) => el.id);

      setActiveCheckBoxes((checkboxes) =>
        checkboxes
          .filter((element) => checkbox != element)
          .filter((element) => !subItems?.includes(element))
      );
    } else {
      setActiveCheckBoxes((checkboxes) => [checkbox, ...checkboxes]);
    }
  };

  return (
    <div className="flex flex-col w-full h-auto bg-[#F3F3F3]">
      <div className="flex flex-col pt-6 gap-4 w-full bg-white rounded-b-[12px] pb-4.5">
        <div className="flex flex-col gap-2 w-full">
          <div className="flex w-full gap-2 px-2 md:px-4">
            <InputGroup>
              <InputGroupInput placeholder="Поиск" />
              <InputGroupAddon>
                <SearchIcon />
              </InputGroupAddon>
            </InputGroup>
            <Drawer>
              <DrawerTrigger asChild={true}>
                <Button variant="ghost" size="icon">
                  <FilterIcon color="#EC6608" />
                </Button>
              </DrawerTrigger>
              <DrawerContent>
                <DrawerFooter className="min-h-[484px]">
                  <DrawerTitle className="text-start">Склад</DrawerTitle>
                  <div className="flex flex-col gap-2">
                    <InputGroup>
                      <InputGroupInput
                        placeholder="Введите город"
                        value={citySearch}
                        onChange={(e) => setCitySearch(e.currentTarget.value)}
                      />
                      <InputGroupAddon>
                        <SearchIcon />
                      </InputGroupAddon>
                      {allowDeleteCity && (
                        <InputGroupButton
                          onClick={() => setSearchableCityNull()}
                        >
                          <X />
                        </InputGroupButton>
                      )}
                    </InputGroup>
                    {findedCities.length > 0 && (
                      <div className="flex flex-col gap-1">
                        {findedCities.map((el, i) => (
                          <Button
                            variant="ghost"
                            key={i}
                            onClick={() => setSearchableCity(el)}
                            className="justify-start"
                          >
                            {el}
                          </Button>
                        ))}
                      </div>
                    )}
                  </div>
                  <DrawerTitle className="pt-2">Вид продукции</DrawerTitle>
                  <div className="flex flex-col gap-1">
                    {checkBoxes.map((el) => (
                      <div key={el.id} className="flex flex-col gap-0">
                        <div
                          className="flex gap-1 py-2"
                          onClick={() => toggleCheckBox(el.id)}
                        >
                          <Checkbox
                            checked={activeCheckboxes.includes(el.id)}
                          />
                          <Label>{el.name}</Label>
                        </div>
                        {activeCheckboxes.includes(el.id) &&
                          el.sub.map((subEl) => (
                            <div
                              key={subEl.id}
                              className="flex gap-1 p-2"
                              onClick={() => toggleCheckBox(subEl.id)}
                            >
                              <Checkbox
                                checked={activeCheckboxes.includes(subEl.id)}
                              />
                              <Label>{subEl.name}</Label>
                            </div>
                          ))}
                      </div>
                    ))}
                  </div>
                  <Button className="bg-[#EC6608] hover:bg-[#EC6608] active:scale-98">
                    Сохранить
                  </Button>
                  <DrawerClose>
                    <Button
                      asChild={true}
                      variant="ghost"
                      size="icon"
                      className="absolute right-4 top-4"
                    >
                      <X className="w-6 h-6 opacity-50" />
                    </Button>
                  </DrawerClose>
                </DrawerFooter>
              </DrawerContent>
            </Drawer>
          </div>
          <ScrollArea className="w-auto">
            <div className="flex gap-2 px-2 md:pl-4 w-max">
              {categories.map((el) => (
                <Badge
                  onClick={() => setCategory(el.IDType)}
                  key={el.IDType}
                  className="cursor-pointer"
                  variant={category == el.IDType ? "default" : "outline"}
                >
                  {el.Type}
                </Badge>
              ))}
            </div>
            <ScrollBar className="hidden" orientation="horizontal" />
          </ScrollArea>
        </div>
        <ScrollArea className="w-full">
          <div className="flex w-full gap-2 px-2 md:pl-4 overflow-auto">
            <Select>
              <SelectTrigger className="w-[100px]">
                <SelectValue placeholder="ГОСТ" />
              </SelectTrigger>
              <SelectContent>
                <SelectItem value="light">ГОСТ 20295-85</SelectItem>
                <SelectItem value="dark">
                  ГОСТ 10704-91 / ГОСТ 10706-76
                </SelectItem>
                <SelectItem value="system">ГОСТ ISO 3183-2015</SelectItem>
              </SelectContent>
            </Select>
            <Select>
              <SelectTrigger className="w-[120px]">
                <SelectValue placeholder="Диаметр" />
              </SelectTrigger>
              <SelectContent>
                <SelectItem value="light">Light</SelectItem>
                <SelectItem value="dark">Dark</SelectItem>
                <SelectItem value="system">System</SelectItem>
              </SelectContent>
            </Select>
            <Select>
              <SelectTrigger className="w-[100px]">
                <SelectValue placeholder="Стенка" />
              </SelectTrigger>
              <SelectContent>
                <SelectItem value="light">Light</SelectItem>
                <SelectItem value="dark">Dark</SelectItem>
                <SelectItem value="system">System</SelectItem>
              </SelectContent>
            </Select>
            <Select>
              <SelectTrigger className="w-[140px]">
                <SelectValue placeholder="Марка стали" />
              </SelectTrigger>
              <SelectContent>
                <SelectItem value="light">Light</SelectItem>
              </SelectContent>
            </Select>
          </div>
          <ScrollBar className="hidden" orientation="horizontal" />
        </ScrollArea>
      </div>
      <div className="w-full h-[10px]"></div>
      <div className="pb-18 bg-white rounded-t-[12px] w-full pt-2.5 md:pt-4.5">
        <div className="grid lg:grid-cols-6 md:grid-cols-4 grid-cols-2 gap-2 px-2 md:px-4 w-full">
          {[...Array(24).keys()].map((el) => (
            <ItemCard key={el} />
          ))}
        </div>
      </div>
    </div>
  );
};
