import { useEffect } from "react";

// Функция для вычисления ширины полосы прокрутки
function getScrollbarWidth() {
  return window.innerWidth - document.documentElement.clientWidth;
}

// Хук для блокировки прокрутки
export const useScrollLock = (isLocked: boolean) => {
  useEffect(() => {
    if (isLocked) {
      // 1. Сохраняем текущее состояние прокрутки
      const scrollY = window.scrollY;
      const body = document.body;

      // 2. Рассчитываем ширину полосы прокрутки
      const scrollbarWidth = getScrollbarWidth();

      // 3. Блокируем прокрутку и компенсируем исчезновение скроллбара
      body.style.overflow = "hidden";
      body.style.position = "fixed";
      body.style.top = `-${scrollY}px`;
      body.style.width = "100%";

      // Добавляем отступ, чтобы контент не "прыгал"
      if (scrollbarWidth) {
        body.style.paddingRight = `${scrollbarWidth}px`;
      }

      // 4. Функция для очистки (разблокировки)
      return () => {
        body.style.overflow = "";
        body.style.position = "";
        body.style.top = "";
        body.style.width = "";
        body.style.paddingRight = "";

        // Восстанавливаем позицию прокрутки
        window.scrollTo(0, scrollY);
      };
    }
  }, [isLocked]);
};
