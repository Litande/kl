import { useCallback, useState } from "react";
import debounce from "lodash.debounce";

/**
 * Carousel effect
 * debounceStartCarousel() - starts carousel after CAROUSEL_AUTOSTART_DELAY time
 * carouselIndex:  -1 - OFF, >=0 - ON
 * onMount - debounceStartCarousel is called first time via updateData()
 * onCellClick - debounceStartCarousel - temporarily stops when User clicks on any row,
 *   restarts again with delay time after the last click on a row
 * startCarousel() - sets carouselIndex to 0
 * stopCarousel() - sets carouselIndex to -1
 * showNextRowInCarousel() - set selected ro in the grid and send custom event to geo map to show tooltip
 * useEffect(...showNextRowInCarousel()...) - repeats it every CAROUSEL_SHOW_NEXT_COUNTRY_DELAY until carouse is stopped
 *
 */
const CAROUSEL_AUTOSTART_DELAY = 30 * 1000; // 30 sec

export default function useCarousel() {
  const [carouselIndex, setCarouselIndex] = useState(-1);

  const startCarousel = () => {
    setTimeout(() => {
      setCarouselIndex(0);
    }, 200);
  };
  const stopCarousel = () => {
    setCarouselIndex(-1);
  };

  const toggleCarousel = () => {
    if (carouselIndex === -1) {
      startCarousel();
    } else {
      stopCarousel();
    }
  };

  // triggers startCarousel() after a delay
  // eslint-disable-next-line react-hooks/exhaustive-deps
  const debouncedStartCarousel = useCallback(debounce(startCarousel, CAROUSEL_AUTOSTART_DELAY), []);

  return {
    carouselIndex,
    setCarouselIndex,
    startCarousel,
    stopCarousel,
    toggleCarousel,
    debouncedStartCarousel,
  };
}
