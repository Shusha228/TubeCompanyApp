import { useScrollLock } from "@/hooks/scroll-lock";
import { getComponentByModal } from "@/modals";
import type { Modal } from "@/modals/models/modal";
import { useMemo, useState, type JSX } from "react";
import { ActiveModalContext } from "./context";

export const ActiveModalProvider = ({
  children,
}: {
  children?: JSX.Element;
}) => {
  const [modal, showModal] = useState<Modal | undefined>(undefined);
  const activeModal = useMemo(() => getComponentByModal(modal), [modal]);
  const closeModal = () => showModal(undefined);

  useScrollLock(modal !== undefined);

  return (
    <ActiveModalContext.Provider
      value={{
        _activeObject: modal,
        activeModal,
        showModal,
        closeModal,
      }}
    >
      {children}
    </ActiveModalContext.Provider>
  );
};
