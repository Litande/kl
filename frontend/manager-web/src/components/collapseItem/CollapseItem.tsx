import React, { useRef } from "react";
import { ICollapse } from "./types";
import styled from "styled-components";
import useExpandCollapseAnimation from "hooks/useExpandCollapseAnimation";

type ComponentProps = ICollapse & {
  header: JSX.Element;
  content: JSX.Element;
  onCollapse: () => void;
};

const CollapseItem = ({ header, content, onCollapse, isOpen }: ComponentProps) => {
  const ref = useRef(null);
  const headerHeight = header.props?.height || 0;
  const { styles } = useExpandCollapseAnimation({
    autoHeight: true,
    isExpanded: isOpen,
    minHeight: headerHeight,
    height: ref.current?.clientHeight + headerHeight || headerHeight,
  });

  const onChange = () => {
    onCollapse();
  };

  return (
    <Wrap styles={styles}>
      <div onClick={onChange}>{header}</div>
      {<BaseAnimationWrap ref={ref}>{content}</BaseAnimationWrap>}
    </Wrap>
  );
};

export default CollapseItem;

const Wrap = styled.div<{ styles }>`
  ${({ styles }) => styles};
`;

const BaseAnimationWrap = styled.div``;
