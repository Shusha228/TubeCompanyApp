import {
  Item,
  ItemContent,
  ItemDescription,
  ItemMedia,
  ItemTitle,
} from "./item";

export const ItemCard = () => {
  return (
    <Item variant="outline" className="w-full">
      <ItemMedia variant="image" className="w-full h-auto">
        <img src="/image.png" className="w-full h-auto" />
      </ItemMedia>
      <ItemContent className="w-full">
        <ItemTitle>Item</ItemTitle>
        <ItemDescription>Item</ItemDescription>
      </ItemContent>
    </Item>
  );
};
