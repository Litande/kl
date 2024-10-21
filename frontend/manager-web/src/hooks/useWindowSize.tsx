import { useState, useEffect, useRef } from "react";

import useEventListener from "./useEventListener";

const initialState = buildState();

const useWindowSize = (debounceMS, callback?) => {
  const [state, setState] = useState(initialState);
  const debounce = debounceMS || 100;
  const timeoutRef = useRef<NodeJS.Timeout>();

  const updateState = () => {
    clearTimeout(timeoutRef.current);

    timeoutRef.current = setTimeout(() => {
      setState(buildState());
    }, debounce);
  };

  useEventListener("resize", updateState);

  useEffect(() => {
    setState(buildState());

    return () => {
      clearTimeout(timeoutRef.current);
    };
  }, []);

  useEffect(() => {
    callback && callback(state);
  }, [state, callback]);

  return { ...state };
};

function buildState() {
  const width = getWindowWidth();
  const height = getWindowHeight();

  return {
    width,
    height,
    windowResized: Date.now(),
  };
}

function getWindowWidth() {
  return window.innerWidth || document.documentElement.clientWidth || document.body.clientWidth;
}

function getWindowHeight() {
  return window.innerHeight || document.documentElement.clientHeight || document.body.clientHeight;
}

export default useWindowSize;
