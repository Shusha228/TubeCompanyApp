import type { Item } from "@/models/item";
import { createContext } from "react";

export interface CreateItemFormData {
  idCat: string;
  idType: number;
  idTypeNew: string;
  productionType: string;
  idFunctionType: string;
  name: string;
  gost: string;
  formOfLength: string;
  manufacturer: string;
  steelGrade: string;
  diameter: number;
  profileSize2: number;
  pipeWallThickness: number;
  status: string;
  koef: number;
}

export interface CreateItem {
  isLoading: boolean;
  formData: CreateItemFormData;
  setFormData: (data: Partial<CreateItemFormData>) => void;
  createItem: () => Promise<Item | null>;
  resetForm: () => void;
}

export const CreateItemContext = createContext({} as CreateItem);
