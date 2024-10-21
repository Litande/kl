import { ReactElement, ReactNode } from "react";

export type Position = "top" | "right" | "bottom" | "left";

export type CoordsType = {
  left: number;
  top: number;
};

export interface ITooltip {
  width: number;
  children: ReactElement;
  tooltip: ReactNode;
  position?: Position;
  onToggle?: (props: { isShown: boolean }) => void;
  className?: string;
  showOnHover?: boolean;
}
