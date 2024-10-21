import React, { FC, useRef, useState } from "react";
import styled from "styled-components";

import useToggle from "hooks/useToggle";
import { useOutsideAlerter } from "hooks/useOutsideAlterer";

import LayoutIcon1SVG from "./icons/LayoutIcon1SVG";
import LayoutIcon2SVG from "./icons/LayoutIcon2SVG";
import LayoutIcon3SVG from "./icons/LayoutIcon3SVG";
import { getDefaultLayout, saveLayout } from "./utils";
import { Layout } from "./types";

const layouts = [LayoutIcon1SVG, LayoutIcon2SVG, LayoutIcon3SVG];

type Props = {
  onChange: (index: number) => void;
};

const LayoutButton: FC<Props> = ({ onChange }) => {
  const menuRef = useRef(null);
  const [isExpanded, toggleExpanded] = useToggle(false);
  const [selectedLayout, setSelectedLayout] = useState(() => getDefaultLayout());

  useOutsideAlerter(
    menuRef,
    () => {
      if (isExpanded) {
        toggleExpanded();
      }
    },
    [isExpanded]
  );

  const selectLayout = index => {
    setSelectedLayout(index);
    saveLayout(index);
    onChange && onChange(index);
  };

  const renderItems = () => {
    return (
      <ItemsWrap>
        <Item isHidden={selectedLayout === Layout.One} onClick={() => selectLayout(Layout.One)}>
          <LayoutIcon1SVG />
        </Item>
        <Item isHidden={selectedLayout === Layout.Two} onClick={() => selectLayout(Layout.Two)}>
          <LayoutIcon2SVG />
        </Item>
        <Item isHidden={selectedLayout === Layout.Three} onClick={() => selectLayout(Layout.Three)}>
          <LayoutIcon3SVG />
        </Item>
      </ItemsWrap>
    );
  };

  return (
    <Wrap onClick={toggleExpanded} ref={menuRef}>
      {layouts[selectedLayout]()}
      {isExpanded && renderItems()}
    </Wrap>
  );
};

export default LayoutButton;

const Wrap = styled.div`
  position: relative;
  box-sizing: border-box;
  display: flex;
  cursor: pointer;
  width: 40px;
`;

const ItemsWrap = styled.div`
  position: absolute;
  z-index: 1;
  right: 0;
  top: 40px;
  left: -3px;
  display: flex;
  flex-direction: column;
  color: initial;
  box-shadow: 0 1px 4px ${({ theme }) => theme.colors.border.primary};
  background: ${({ theme }) => theme.colors.bg.ternary};
  border-radius: 4px;
  overflow: hidden;
`;

const Item = styled.div<{ isHidden: boolean }>`
  box-sizing: border-box;
  padding: 3px;
  display: ${({ isHidden }) => (isHidden ? "none" : "flex")};
  align-items: center;
  justify-content: center;

  &:hover {
    background: ${({ theme }) => theme.colors.bg.active};
  }
`;
