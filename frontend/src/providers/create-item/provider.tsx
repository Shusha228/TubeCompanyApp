import { getURL } from "@/api";
import type { Item } from "@/models/item";
import { useCallback, useState, type JSX } from "react";
import { CreateItemContext, type CreateItemFormData } from "./context";

const initialFormData: CreateItemFormData = {
  idCat: "",
  idType: 0,
  idTypeNew: "",
  productionType: "",
  idFunctionType: "",
  name: "",
  gost: "",
  formOfLength: "",
  manufacturer: "",
  steelGrade: "",
  diameter: 0,
  profileSize2: 0,
  pipeWallThickness: 0,
  status: "",
  koef: 0,
};

export const CreateItemProvider = ({ children }: { children: JSX.Element }) => {
  const [isLoading, setIsLoading] = useState<boolean>(false);
  const [formData, setFormDataState] =
    useState<CreateItemFormData>(initialFormData);

  const setFormData = useCallback((data: Partial<CreateItemFormData>) => {
    setFormDataState((prev) => ({ ...prev, ...data }));
  }, []);

  const resetForm = useCallback(() => {
    setFormDataState(initialFormData);
  }, []);

  const createItem = useCallback(async (): Promise<Item | null> => {
    setIsLoading(true);
    try {
      const response = await fetch(getURL("Nomenclature"), {
        method: "POST",
        headers: {
          accept: "text/plain",
          "Content-Type": "application/json",
        },
        body: JSON.stringify({
          id: 0,
          ...formData,
        }),
      });

      if (!response.ok) {
        throw new Error(`Request failed: ${response.status}`);
      }

      const createdItem = (await response.json()) as Item;
      return createdItem;
    } catch (error) {
      console.error("Error creating item:", error);
      return null;
    } finally {
      setIsLoading(false);
    }
  }, [formData]);

  return (
    <CreateItemContext.Provider
      value={{
        isLoading,
        formData,
        setFormData,
        createItem,
        resetForm,
      }}
    >
      {children}
    </CreateItemContext.Provider>
  );
};
