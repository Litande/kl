import { ICellRendererParams } from "ag-grid-community";
import styled from "styled-components";

import { IRow } from "./types";

const AgentsNameCellRenderer = (props: ICellRendererParams<IRow>) => {
  return (
    <Wrap>
      <CommentIcon className={"icon-ic-comment"} />
      {props.data.agentFullName}
    </Wrap>
  );
};

export default AgentsNameCellRenderer;

const Wrap = styled.div`
  display: flex;
  align-items: center;
  gap: 19px;
`;

const CommentIcon = styled.i`
  font-size: 20px;
`;
