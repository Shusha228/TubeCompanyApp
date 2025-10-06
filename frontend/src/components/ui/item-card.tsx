import { Item, ItemContent, ItemMedia, ItemTitle } from "./item";

export const ItemCard = () => {
  return (
    <Item
      variant="outline"
      className="p-0 w-full gap-2 pb-2 border-0 bg-none cursor-pointer"
    >
      <ItemMedia variant="image" className="w-full h-auto">
        <img src="/image.png" className="w-full h-auto" />
      </ItemMedia>
      <ItemContent className="w-full">
        <ItemTitle className="text-md uppercase pl-2">Item</ItemTitle>
      </ItemContent>
    </Item>
  );
};
