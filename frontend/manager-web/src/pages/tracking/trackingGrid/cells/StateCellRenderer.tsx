import React from "react";
import styled from "styled-components";
import { ICellRendererParams } from "ag-grid-community";
import { AgentStatusStr, agentStatusToLabel } from "types";
import { hexToRgbA } from "utils/hexToRgbA";
import { IRow } from "../types";

const StateCellRenderer = (props: ICellRendererParams<IRow>) => {
  const status = props.data.state;

  return <Container status={status}>{agentStatusToLabel[status]}</Container>;
};

export default StateCellRenderer;

const Container = styled.div<{ status: AgentStatusStr }>`
  border-radius: 16px;
  background: #777;
  display: inline-block;
  height: 32px;
  padding: 0 15px;
  line-height: 32px;
  background: ${({ theme, status }) =>
    hexToRgbA(
      theme.colors.agentStatus[status].card ||
        theme.colors.agentStatus[AgentStatusStr.Offline].card,
      0.3
    )};
`;
