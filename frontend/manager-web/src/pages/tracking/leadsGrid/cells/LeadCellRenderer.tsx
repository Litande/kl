import React from "react";
import styled from "styled-components";
import { ICellRendererParams } from "ag-grid-community";

const LeadCellRenderer = (props: ICellRendererParams) => {
  return (
    <Container>
      <UserInfo>
        <UserIcon className="icon-user" />
        {props.value}
      </UserInfo>
    </Container>
  );
};

export default LeadCellRenderer;

const Container = styled.div`
  display: flex;
  justify-content: space-between;
  align-items: center;
`;

const UserInfo = styled.div`
  display: flex;
  align-items: center;
  margin: 0 10px 0 0;
`;

const UserIcon = styled.i`
  position: relative;
  margin: 0 16px 0 0;
  font-size: 20px;
  color: ${({ theme }) => theme.colors.icons.primary};
`;
