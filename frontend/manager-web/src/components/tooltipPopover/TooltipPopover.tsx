import React, { useEffect, useRef, useState } from "react";
import styled from "styled-components";
import Portal, { PORTAL_ID } from "components/portal/Portal";
import { getTooltipCoords, getContainerTransform, getArrowStyles } from "./utils";
import { ITooltip, Position, CoordsType } from "./types";

const TooltipPopover = ({
  width,
  children,
  tooltip,
  onToggle,
  position = "top",
  className,
  showOnHover = false,
}: ITooltip) => {
  const [isShown, setIsShown] = useState(false);
  const [coords, setCoords] = useState<CoordsType>({ left: 0, top: 0 });
  const containerRef = useRef(null);
  const tooltipContentRef = useRef(null);

  const updateTooltipCoords = () => {
    const rect = containerRef.current.getBoundingClientRect();
    setCoords(getTooltipCoords(width, position, rect));
  };

  useEffect(() => {
    updateTooltipCoords();
  }, [position]);

  const handleClick = (e: React.MouseEvent<HTMLButtonElement>) => {
    updateTooltipCoords();
    setIsShown(!isShown);
    onToggle && onToggle({ isShown: !isShown, event: e });
  };

  const handleMouseOver = () => {
    if (showOnHover) {
      return () => {
        updateTooltipCoords();
        setIsShown(true);
      };
    }
  };

  const handleMouseOut = () => {
    if (showOnHover) {
      return () => {
        setIsShown(false);
      };
    }
  };
  useEffect(() => {
    const handleScroll = () => {
      setIsShown(false);
    };

    document.addEventListener("scroll", handleScroll);

    return () => {
      document.removeEventListener("scroll", handleScroll);
    };
  }, [isShown]);

  useEffect(() => {
    if (!tooltipContentRef.current) return;

    const handleClick = e => {
      const modal = document.getElementById(PORTAL_ID.MODAL);
      if (e.target === modal || modal.contains(e.target)) {
        return;
      }

      if (
        tooltipContentRef.current.contains(e.target) &&
        e.target.classList.contains("icon-close")
      ) {
        setIsShown(false);
      }

      if (
        !tooltipContentRef.current.contains(e.target) &&
        containerRef.current !== e.target &&
        !containerRef.current.contains(e.target)
      ) {
        setIsShown(false);
        onToggle({ isShown: false, event: e });
      }
    };

    document.addEventListener("click", handleClick);

    return () => document.removeEventListener("click", handleClick);
  }, [isShown, onToggle]);

  return (
    <>
      {React.cloneElement(children, {
        ref: containerRef,
        onClick: e => {
          handleClick(e);
          if (children.props?.onClick) {
            children.props.onClick(e);
          }
        },
        onMouseOver: handleMouseOver(),
        onMouseOut: handleMouseOut(),
      })}
      {isShown && (
        <Portal id={PORTAL_ID.TOOLTIP}>
          <TooltipContainer
            ref={tooltipContentRef}
            width={width}
            left={coords.left}
            top={coords.top}
            position={position}
            className={className}
          >
            <Content>{tooltip}</Content>
          </TooltipContainer>
        </Portal>
      )}
    </>
  );
};

interface ITooltipContainer extends CoordsType {
  width: number;
  position?: Position;
}

const TooltipContainer = styled.div<ITooltipContainer>`
  position: absolute;
  z-index: 1;
  width: ${({ width }) => `${width}px`};
  top: ${({ top }) => top}px;
  left: ${({ left }) => left}px;
  ${({ position, width }) => getContainerTransform(position, width)};

  &:after {
    content: "";
    position: absolute;
    width: 8px;
    height: 8px;
    background: ${({ theme }) => theme.colors.bg.ternary};
    ${({ position }) => getArrowStyles(position)};
  }
`;

const Content = styled.div`
  height: 100%;
  white-space: pre-wrap;
`;

export default TooltipPopover;