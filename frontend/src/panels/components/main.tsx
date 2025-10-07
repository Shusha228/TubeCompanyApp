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
import { Spinner } from "@/components/ui/spinner";
import { City } from "@/models/city";
import { FetchAllItemsProvider, useFetchAllItems } from "@/providers/all-items";
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
    sub: [
      {
        id: "12",
        name: "Трубы общего назначения",
        sub: [],
      },
      {
        id: "13",
        name: "Котельные трубы",
        sub: [],
      },
      {
        id: "14",
        name: "Крекинговые трубы",
        sub: [],
      },
      {
        id: "15",
        name: "Нержавеющие трубы",
        sub: [],
      },
      {
        id: "16",
        name: "Подшипниковые трубы",
        sub: [],
      },
    ],
  },
  {
    id: "9",
    name: "Сварные трубы промышленного назначения",
    sub: [
      {
        id: "17",
        name: "Трубы общего назначения",
        sub: [],
      },
      {
        id: "18",
        name: "Водогазопроводные",
        sub: [],
      },
      {
        id: "19",
        name: "Профильные",
        sub: [],
      },
      {
        id: "20",
        name: "Оцинкованные",
        sub: [],
      },
    ],
  },
  {
    id: "10",
    name: "Антикоррозионные покрытия на трубы",
    sub: [
      {
        id: "21",
        name: "Наружное одно- и двухслойное эпоксидное",
        sub: [],
      },
      {
        id: "22",
        name: "Наружное двух- и трехслойное полиэтиленовое или полипропиленовое",
        sub: [],
      },
      {
        id: "23",
        name: "Наружное теплогидроизоляционное ППУ в оболочке",
        sub: [],
      },
      {
        id: "24",
        name: "Внутреннее гладкостное",
        sub: [],
      },
      {
        id: "25",
        name: "Внутреннее антикоррозионное типа Amercoat, Scotchcoat",
        sub: [],
      },
    ],
  },
  {
    id: "11",
    name: "Заготовка непрерывнолитая",
    sub: [
      {
        id: "26",
        name: "Круглого сечения",
        sub: [],
      },
      {
        id: "27",
        name: "Квадратного сечения",
        sub: [],
      },
    ],
  },
];

const ifNullStringReturnUndefined = (value: string) =>
  value == "__null__" ? undefined : value;

export const MainPanel = () => {
  const [category, _setCategory] = useState<string[]>([]);
  const [activeCheckboxes, setActiveCheckBoxes] = useState<string[]>([]);
  const [citySearch, _setCitySearch] = useState<string>("");
  const [, setActiveCity] = useState<City>();
  const citySearchInput = useRef<HTMLInputElement>(null);
  const [allowDeleteCity, setAllowDeleteCity] = useState<boolean>(false);
  const [gost, _setGost] = useState<string>();
  const [diameter, _setDiameter] = useState<string>();
  const [pipeWallThickness, _setPipeWallThicknesses] = useState<string>();
  const [steelGrade, _setSteelGrades] = useState<string>();

  const setGost = (value: string) =>
    _setGost(ifNullStringReturnUndefined(value));

  const setDiameter = (value: string) =>
    _setDiameter(ifNullStringReturnUndefined(value));

  const setPipeWallThicknesses = (value: string) =>
    _setPipeWallThicknesses(ifNullStringReturnUndefined(value));

  const setSteelGrades = (value: string) =>
    _setSteelGrades(ifNullStringReturnUndefined(value));

  const setCategory = (value?: string) => {
    _setCategory(value !== undefined ? [value] : []);
    _setGost(undefined);
    _setDiameter(undefined);
    _setPipeWallThicknesses(undefined);
    _setSteelGrades(undefined);
  };

  const saveChangesInModal = () => {
    _setCategory(activeCheckboxes);
  };

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

  const SearchBar = () => {
    const { setFilters } = useFetchAllItems();
    return (
      <InputGroup>
        <InputGroupInput
          placeholder="Поиск"
          onChange={(e) => setFilters({ search: e.currentTarget.value })}
        />
        <InputGroupAddon>
          <SearchIcon />
        </InputGroupAddon>
      </InputGroup>
    );
  };

  const CityInput = () => (
    <InputGroup>
      <InputGroupInput
        placeholder="Введите город"
        value={citySearch}
        onChange={(e) => setCitySearch(e.currentTarget.value)}
        ref={citySearchInput}
      />
      <InputGroupAddon>
        <SearchIcon />
      </InputGroupAddon>
      {allowDeleteCity && (
        <InputGroupButton onClick={() => setSearchableCityNull()}>
          <X />
        </InputGroupButton>
      )}
    </InputGroup>
  );

  const ProductTypeList = () => (
    <ScrollArea className="w-full min-h-[200px] overflow-auto">
      <ScrollBar orientation="horizontal" />
      <div className="flex flex-col gap-1 h-auto w-full">
        {checkBoxes.map((el) => (
          <div key={el.id} className="flex flex-col gap-0">
            <div
              className="flex gap-1 py-2"
              onClick={() => toggleCheckBox(el.name)}
            >
              <Checkbox checked={activeCheckboxes.includes(el.name)} />
              <Label>{el.name}</Label>
            </div>
            {activeCheckboxes.includes(el.name) &&
              el.sub.map((subEl) => (
                <div
                  key={subEl.id}
                  className="flex gap-1 p-2"
                  onClick={() => toggleCheckBox(subEl.name)}
                >
                  <Checkbox checked={activeCheckboxes.includes(subEl.name)} />
                  <Label>{subEl.name}</Label>
                </div>
              ))}
          </div>
        ))}
      </div>
    </ScrollArea>
  );

  const AttributeSelects = () => {
    const { data, isLoading } = useFetchAllItems();
    const visibleData = useMemo(
      () =>
        category.length > 0
          ? data.filter(
              (el) =>
                category.length > 0 || category.includes(el.productionType)
            )
          : [],
      [data]
    );

    if (isLoading || visibleData.length == 0) {
      return <></>;
    }

    const gosts = [...new Set(visibleData.map((el) => el.gost))];
    const diameters = [
      ...new Set(visibleData.map((el) => el.diameter.toString())),
    ];
    const pipeWallThicknesses = [
      ...new Set(visibleData.map((el) => el.pipeWallThickness.toString())),
    ];
    const steelGrades = [
      ...new Set(visibleData.map((el) => el.steelGrade.toString())),
    ];

    return (
      <ScrollArea className="w-full">
        <div className="flex w-full gap-2 px-2 md:pl-4 overflow-auto">
          <Select onValueChange={setGost} value={gost}>
            <SelectTrigger className="w-[100px]">
              <SelectValue placeholder="ГОСТ" />
            </SelectTrigger>
            <SelectContent>
              <SelectItem value={"__null__"}>Отменить выбор</SelectItem>
              {gosts.map((el, i) => (
                <SelectItem key={i} value={el}>
                  {el}
                </SelectItem>
              ))}
            </SelectContent>
          </Select>
          <Select onValueChange={setDiameter} value={diameter}>
            <SelectTrigger className="w-[120px]">
              <SelectValue placeholder="Диаметр" />
            </SelectTrigger>
            <SelectContent>
              <SelectItem value={"__null__"}>Отменить выбор</SelectItem>
              {diameters.map((el, i) => (
                <SelectItem key={i} value={el}>
                  {el}
                </SelectItem>
              ))}
            </SelectContent>
          </Select>
          <Select
            onValueChange={setPipeWallThicknesses}
            value={pipeWallThickness}
          >
            <SelectTrigger className="w-[100px]">
              <SelectValue placeholder="Стенка" />
            </SelectTrigger>
            <SelectContent>
              <SelectItem value={"__null__"}>Отменить выбор</SelectItem>
              {pipeWallThicknesses.map((el, i) => (
                <SelectItem
                  key={i}
                  value={el}
                  onClick={() => setPipeWallThicknesses(el)}
                >
                  {el}
                </SelectItem>
              ))}
            </SelectContent>
          </Select>
          <Select onValueChange={setSteelGrades} value={steelGrade}>
            <SelectTrigger className="w-[140px]">
              <SelectValue placeholder="Марка стали" />
            </SelectTrigger>
            <SelectContent>
              <SelectItem value={"__null__"}>Отменить выбор</SelectItem>
              {steelGrades.map((el, i) => (
                <SelectItem
                  key={i}
                  value={el}
                  onClick={() => setSteelGrades(el)}
                >
                  {el}
                </SelectItem>
              ))}
            </SelectContent>
          </Select>
        </div>
        <ScrollBar className="hidden" orientation="horizontal" />
      </ScrollArea>
    );
  };

  const ItemsGrid = () => {
    const { data, isLoading } = useFetchAllItems();
    const filteredData = useMemo(
      () =>
        data.filter(
          (el) =>
            (category.length == 0 || category.includes(el.productionType)) &&
            (gost === undefined || el.gost === gost) &&
            (pipeWallThickness === undefined ||
              el.pipeWallThickness.toString() === pipeWallThickness) &&
            (diameter === undefined || el.diameter.toString() === diameter) &&
            (steelGrade === undefined || el.steelGrade === steelGrade)
        ),
      [data]
    );
    return (
      <div className="relative grid lg:grid-cols-6 md:grid-cols-4 grid-cols-2 gap-2 px-2 md:px-4 w-full">
        {isLoading && <Spinner />}{" "}
        {filteredData.length > 0 &&
          !isLoading &&
          filteredData.map((item) => <ItemCard key={item.id} item={item} />)}
        {filteredData.length == 0 && (
          <div className="absolute w-dvw top-2 text-center">
            Упс! Ничего не нашлось
          </div>
        )}
      </div>
    );
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
        .find((el) => el.name == checkbox)
        ?.sub.map((el) => el.name);

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
    <FetchAllItemsProvider>
      <div className="flex flex-col w-full h-auto bg-[#F3F3F3]">
        <div className="flex flex-col pt-6 gap-4 w-full bg-white rounded-b-[12px] pb-4.5">
          <div className="flex flex-col gap-2 w-full">
            <div className="flex w-full gap-2 px-2 md:px-4">
              <SearchBar />
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
                      <CityInput />
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
                    <ProductTypeList />
                    <DrawerClose asChild={true}>
                      <Button
                        className="bg-[#EC6608] hover:bg-[#EC6608] active:scale-98"
                        onClick={saveChangesInModal}
                      >
                        Сохранить
                      </Button>
                    </DrawerClose>
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
                    onClick={
                      category.includes(el.Type)
                        ? () => setCategory(undefined)
                        : () => setCategory(el.Type)
                    }
                    key={el.IDType}
                    className="cursor-pointer"
                    variant={category.includes(el.Type) ? "default" : "outline"}
                  >
                    {el.Type}
                  </Badge>
                ))}
              </div>
              <ScrollBar className="hidden" orientation="horizontal" />
            </ScrollArea>
          </div>
          {category && <AttributeSelects />}
        </div>
        <div className="w-full h-[10px]"></div>
        <div className="pb-18 bg-white rounded-t-[12px] w-full pt-2.5 md:pt-4.5">
          <ItemsGrid />
        </div>
      </div>
    </FetchAllItemsProvider>
  );
};
