import { getURL } from "@/api";
import type { Item } from "@/models/item";
import { useState, type JSX } from "react";
import { ModalItemContext } from "./context";

export const ModalItemProvider = ({ children }: { children: JSX.Element }) => {
  const [isLoading, setLoading] = useState(true);

  const [data, setData] = useState<Item>();

  const _showModal = (id: number) => {
    setLoading(true);
    fetch(getURL(`Nomenclature/${id}`))
      .then((el) => el.json())
      .then((data) => {
        setData(data["data"]);
      })
      .finally(() => setLoading(false));
  };
  return (
    <ModalItemContext.Provider
      value={{ isLoading, data, showModal: _showModal }}
    >
      {children}
    </ModalItemContext.Provider>
  );
};
