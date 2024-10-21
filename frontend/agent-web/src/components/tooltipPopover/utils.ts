import { css } from "styled-components";
import { Position } from "./types";

export const getTooltipCoords = (width: number, position: Position, rect: DOMRect) => {
  switch (position) {
    case "top":
      return {
        left: rect.x + rect.width / 2,
        top: rect.y + window.scrollY - 8,
      };
    case "bottom":
      return {
        left: rect.x + rect.width / 2,
        top: rect.y + rect.height + 8,
      };
    case "right":
      return {
        left: rect.x + width / 2 + rect.width,
        top: rect.y + rect.height / 2,
      };
    case "left":
      return {
        left: rect.x - width / 2 - rect.width,
        top: rect.y + rect.height / 2,
      };
  }
};

export const getArrowStyles = (position: Position) => {
  switch (position) {
    case "top":
      return css`
        left: calc(50% - 4px);
        bottom: -3px;
        transform: rotate(45deg);
        box-shadow: 1px 1px 1px ${({ theme }) => theme.colors.border.primary};
      `;
    case "bottom":
      return css`
        left: calc(50% - 4px);
        top: -3px;
        transform: rotate(45deg);
        box-shadow: -1px -1px 0px ${({ theme }) => theme.colors.border.primary};
      `;
    case "left":
      return css`
        right: -3px;
        top: calc(50% - 4px);
        transform: rotate(135deg);
        box-shadow: -1px -1px 0px 0px ${({ theme }) => theme.colors.border.primary};
      `;
    case "right":
      return css`
        left: -3px;
        top: calc(50% - 4px);
        transform: rotate(45deg);
        box-shadow: -1px 1px 1px 0 ${({ theme }) => theme.colors.border.primary};
      `;
  }
};

export const getContainerTransform = (position: Position, width: number) => {
  switch (position) {
    case "left":
    case "right":
      return css`
        transform: translate(-${width / 2}px, -50%);
      `;
    case "bottom":
      return css`
        transform: translate(-${width / 2}px, 0);
      `;
    default:
      return css`
        transform: translate(-${width / 2}px, -100%);
      `;
  }
};
