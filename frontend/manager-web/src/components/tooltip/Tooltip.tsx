import React, { Fragment, ReactNode } from "react";
import styled from "styled-components";
import { Tooltip } from "react-tooltip";

interface ITooltip {
  id: string | number;
  isOpen: boolean;
  children: ReactNode;
  tooltip: ReactNode | string;
}

const CustomTooltip = ({ id, children, tooltip, isOpen }: ITooltip) => (
  <Fragment>
    <StyledTooltip anchorId={`${id}`} isOpen={isOpen} positionStrategy="fixed">
      {tooltip}
    </StyledTooltip>
    <div id={`${id}`}>{children}</div>
  </Fragment>
);

export default CustomTooltip;

const StyledTooltip = styled(Tooltip)`
  z-index: 10;
  background: white !important;
  box-shadow: 0 1px 4px rgba(0, 0, 0, 0.16);
  border-radius: 4px;
  opacity: 1 !important;
  color: inherit !important;

  > div:last-child {
    box-shadow: 1px 1px 1px rgb(0 0 0 / 16%);
  }
`;
