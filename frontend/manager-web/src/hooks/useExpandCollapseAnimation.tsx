import { useEffect, useState } from "react";
import { CSSObject } from "styled-components";

const getStyles = ({
  isHidden,
  isExpanded,
  height,
  isAutoHeight = false,
  minHeight = 0,
}): CSSObject => {
  const expandedHeight = isAutoHeight ? "auto" : `${height}px`;
  const collapsedHeight = isAutoHeight ? `${height}px` : `${minHeight}px`;

  return {
    transition: "height 0.5s ease-in",
    height: isExpanded ? expandedHeight : collapsedHeight,
    overflow: isHidden ? "initial" : "hidden",
  };
};

function useExpandCollapseAnimation({ autoHeight = false, isExpanded, height, minHeight = 0 }) {
  const [styles, setStyles] = useState<CSSObject>(() =>
    getStyles({ isExpanded, height, minHeight, isHidden })
  );
  const [isAutoHeight, setIsAutoHeight] = useState(false);
  const [isHidden, setIsHidden] = useState(false);

  useEffect(() => {
    if (!autoHeight) return;

    if (isExpanded) {
      setTimeout(() => setIsAutoHeight(true), 500);
    } else {
      setIsAutoHeight(false);
    }
  }, [isExpanded, autoHeight]);

  useEffect(() => {
    if (isExpanded) {
      setTimeout(() => setIsHidden(true), 500);
    } else {
      setIsHidden(false);
    }
  }, [isExpanded]);

  useEffect(() => {
    setStyles(getStyles({ isHidden, isAutoHeight, isExpanded, height, minHeight }));
  }, [isExpanded, isAutoHeight, isHidden, height, minHeight]);

  return {
    styles,
  };
}

export default useExpandCollapseAnimation;
