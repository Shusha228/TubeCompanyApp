interface Remnants {
  idStock: string;
  inStockT: number;
  inStockM: number;
}

export interface Item {
  id: number;
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
  remnants: Remnants[];
}
