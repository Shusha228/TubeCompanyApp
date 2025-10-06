import { Badge } from "@/components/ui/badge";
import { Button } from "@/components/ui/button";
import {
  InputGroup,
  InputGroupAddon,
  InputGroupInput,
} from "@/components/ui/input-group";
import { ItemCard } from "@/components/ui/item-card";
import {
  Select,
  SelectContent,
  SelectItem,
  SelectTrigger,
  SelectValue,
} from "@/components/ui/select";
import { FilterIcon, SearchIcon } from "lucide-react";
import { useState } from "react";

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

export const MainPanel = () => {
  const [category, setCategory] = useState<string>();
  return (
    <div className="p-6 flex flex-col gap-4 w-full">
      <div className="flex flex-col gap-2 w-full">
        <div className="flex w-full gap-2">
          <InputGroup>
            <InputGroupInput placeholder="Search..." />
            <InputGroupAddon>
              <SearchIcon />
            </InputGroupAddon>
          </InputGroup>
          <Button variant="ghost">
            <FilterIcon />
          </Button>
        </div>
        <div className="flex w-full gap-2 overflow-auto">
          {categories.map((el) => (
            <Badge
              onClick={() => setCategory(el.IDType)}
              key={el.IDType}
              variant={category == el.IDType ? "default" : "outline"}
            >
              {el.Type}
            </Badge>
          ))}
        </div>
      </div>
      <div className="flex w-full gap-2 overflow-auto">
        <Select>
          <SelectTrigger className="w-[120px]">
            <SelectValue placeholder="ГОСТ" />
          </SelectTrigger>
          <SelectContent>
            <SelectItem value="light">ГОСТ 20295-85</SelectItem>
            <SelectItem value="dark">ГОСТ 10704-91 / ГОСТ 10706-76</SelectItem>
            <SelectItem value="system">ГОСТ ISO 3183-2015</SelectItem>
          </SelectContent>
        </Select>
        <Select>
          <SelectTrigger className="w-[180px]">
            <SelectValue placeholder="Стенка" />
          </SelectTrigger>
          <SelectContent>
            <SelectItem value="light">Light</SelectItem>
            <SelectItem value="dark">Dark</SelectItem>
            <SelectItem value="system">System</SelectItem>
          </SelectContent>
        </Select>
        <Select>
          <SelectTrigger className="w-[180px]">
            <SelectValue placeholder="Марка стали" />
          </SelectTrigger>
          <SelectContent>
            <SelectItem value="light">Light</SelectItem>
          </SelectContent>
        </Select>
      </div>
      <div className="grid lg:grid-cols-6 md:grid-cols-4 grid-cols-2 gap-2">
        {[...Array(24).keys()].map((el) => (
          <ItemCard key={el} />
        ))}
      </div>
    </div>
  );
};
