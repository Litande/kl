import React from "react";
import styled from "styled-components";
import { ICellRendererParams } from "ag-grid-community";
import { IRow } from "../types";

const AgentsFullWidthCellRenderer = (props: ICellRendererParams<IRow>) => {
  const keysToRender = [
    "leadName",
    "leadId",
    "brandName",
    "affiliateId",
    "registrationTime",
    "callDuration",
    "phoneNumber",
    "country",
  ];

  return (
    <Container>
      {keysToRender.map(
        key => props.data[key] && <InfoBlock key={key}>{props.data[key]}</InfoBlock>
      )}
    </Container>
  );
};

export default AgentsFullWidthCellRenderer;

const InfoBlock = styled.div`
  margin: 0 22px 0 0;
  font-weight: 400;

  &:after {
    display: inline-block;
    content: "|";
    margin: 0 0 0 22px;
  }

  &:last-child:after {
    content: "";
  }
`;

const Container = styled.div`
  display: flex;
  align-items: center;
  box-sizing: border-box;
  height: 100%;
  padding: 0 17px 0 35px;
  font-size: 12px;
  font-family: "Inter light", serif;
  white-space: nowrap;
  color: ${({ theme }) => theme.colors.fg.secondary_light};
`;
