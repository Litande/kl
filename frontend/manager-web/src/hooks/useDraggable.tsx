import { useRef, useState } from "react";

interface IPosition {
  x?: number;
  y?: number;
}
const savePosition = (elementId: string, pos: IPosition) => {
  const prev = restorePosition(elementId);
  localStorage.setItem(elementId, JSON.stringify({ ...prev, ...pos }));
};

const getPositionInsideOfViewPort = (position: IPosition) => {
  const maxWidth = defaultPosition().x;
  const maxHeight = defaultPosition().y;

  return {
    ...position,
    x: Math.min(maxWidth, position.x),
    y: Math.min(maxHeight, position.y),
  };
};

const defaultPosition = (): IPosition => {
  return {
    x: (window.innerWidth || document.documentElement.clientWidth) / 2 - 227, // it shifts back the visible chat block inside the view port
    y: (window.innerHeight || document.documentElement.clientHeight) / 2 - 95,
  };
};

const restorePosition = (elementId: string): IPosition => {
  try {
    return (JSON.parse(localStorage.getItem(elementId)) as IPosition) || defaultPosition();
  } catch (e) {
    return defaultPosition();
  }
};

const DEFAULT_HANDLE_CLASS_NAME = "modal-drag-handle";

type UseDraggableProps = {
  handleClassName?: typeof DEFAULT_HANDLE_CLASS_NAME;
};

export const useDraggable = ({
  handleClassName = DEFAULT_HANDLE_CLASS_NAME,
}: UseDraggableProps) => {
  const [positionX] = useState(getPositionInsideOfViewPort(restorePosition("onCallModal")).x);
  const [positionY] = useState(getPositionInsideOfViewPort(restorePosition("onCallModal")).y);

  const draggableRef = useRef(null);
  const handle = `.${handleClassName}`;

  const stopHandler = (_event, { x, y }) => {
    savePosition("onCallModal", { x, y });
  };

  const defaultPosition = { x: positionX, y: positionY };

  return {
    defaultPosition,
    draggableRef,
    stopHandler,
    handle,
    handleClassName,
  };
};
