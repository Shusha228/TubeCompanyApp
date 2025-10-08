import { getURL } from "@/api";
import type { Item } from "@/models/item";
import { useState, type JSX } from "react";
import { UpdateItemContext } from "./context";

export const UpdateItemProvider = ({ children }: { children: JSX.Element }) => {
  const [item, setItem] = useState<Item>();

  const updateItem = async (updatedItem: Partial<Item>) => {
    if (!item?.id) {
      throw new Error("Item ID is required for update");
    }

    try {
      const response = await fetch(getURL(`Nomenclature/${item.id}`), {
        method: "PUT",
        headers: {
          "Content-Type": "application/json",
        },
        body: JSON.stringify(updatedItem),
      });

      if (!response.ok) {
        throw new Error(`Failed to update item: ${response.statusText}`);
      }

      const updatedItemData = await response.json();
      setItem(updatedItemData);
    } catch (error) {
      console.error("Error updating item:", error);
      throw error;
    }
  };

  return (
    <UpdateItemContext.Provider value={{ item, setItem, updateItem }}>
      {children}
    </UpdateItemContext.Provider>
  );
};
