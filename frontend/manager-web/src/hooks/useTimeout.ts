import { useEffect, useLayoutEffect, useRef } from "react";

function useTimeout(callback: () => void, delay: number | null) {
  const savedCallback = useRef(callback);

  useLayoutEffect(() => {
    savedCallback.current = callback;
  }, [callback]);

  useEffect(() => {
    if (!delay && delay !== 0) {
      return;
    }

    const timeout = setTimeout(() => savedCallback.current(), delay);

    return () => clearTimeout(timeout);
  }, [delay]);
}

export default useTimeout;
