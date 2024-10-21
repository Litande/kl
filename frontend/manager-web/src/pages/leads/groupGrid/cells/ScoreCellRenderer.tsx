import React from "react";
import styled from "styled-components";
import { ICellRendererParams } from "ag-grid-community";

const ScoreCellRenderer = (props: ICellRendererParams) => {
  return <Container>{props.value}</Container>;
};

const Container = styled.div`
  display: flex;
  justify-content: space-between;
  align-items: center;
`;

export default ScoreCellRenderer;
