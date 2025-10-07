import { type RefObject, useEffect, useRef } from "react";

interface UseObservableOptions<T extends Element> {
  ref: RefObject<T | null>;
  onIntersect: () => void;
  threshold?: number;
  rootMargin?: string;
  once?: boolean;
}

export function useObservable<T extends Element>({
  ref,
  onIntersect,
  threshold = 0.1,
  rootMargin = "0px",
  once = false,
}: UseObservableOptions<T>): void {
  const hasIntersected = useRef(false);

  useEffect(() => {
    const element = ref.current;
    if (!element) return;

    // Если уже сработало и опция once=true, не создаем новый observer
    if (once && hasIntersected.current) return;

    const observer = new IntersectionObserver(
      (entries) => {
        const [entry] = entries;

        if (entry.isIntersecting) {
          onIntersect();
          hasIntersected.current = true;

          // Если once=true, отключаем наблюдение после первого пересечения
          if (once) {
            observer.unobserve(element);
          }
        }
      },
      {
        threshold,
        rootMargin,
      }
    );

    observer.observe(element);

    return () => {
      observer.unobserve(element);
    };
  }, [ref, onIntersect, threshold, rootMargin, once]);
}
