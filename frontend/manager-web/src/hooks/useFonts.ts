import { useEffect, useState } from "react";

export const useFonts = (...fontNames: string[]) => {
  const [isLoaded, setIsLoaded] = useState(false);

  useEffect(() => {
    if (!document || !document.fonts) {
      // eslint-disable-next-line no-console
      console.warn("Browser does not support document.fonts API");
      return;
    }

    Promise.all(fontNames.map(fontName => document.fonts.load(`16px "${fontName}"`))).then(() => {
      setIsLoaded(true);
    });
  }, [fontNames]);

  return isLoaded;
};
