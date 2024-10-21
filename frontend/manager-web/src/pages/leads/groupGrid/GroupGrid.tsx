import React, { useMemo } from "react";
import styled from "styled-components";
import { GetRowIdFunc, GetRowIdParams } from "ag-grid-community";
import Grid from "components/grid/Grid";
import Total from "components/total/Total";

import columnDefs from "./columnsDefs";
import { ILeadGroup } from "../Leads";

interface IPropTypes extends ILeadGroup {
  color: string;
}

const GroupGrid = (props: IPropTypes) => {
  const getRowId = useMemo<GetRowIdFunc>(() => (params: GetRowIdParams) => params.data.leadId, []);

  return (
    <Container>
      <GroupName color={props.color}>
        <GroupText>{props.groupName}</GroupText>
        <Total color={props.color}>{props.leadsCount}</Total>
      </GroupName>
      <Wrapper>
        <GridContainer
          rowData={props.leads}
          columnDefs={columnDefs}
          getRowId={getRowId}
          rowSelection="single"
        />
      </Wrapper>
    </Container>
  );
};

export default GroupGrid;

const HEADER_HEIGHT = "68px";

const GroupName = styled.h3`
  box-sizing: border-box;
  flex: 1;
  display: flex;
  justify-content: space-between;
  align-items: center;
  width: 100%;
  height: ${HEADER_HEIGHT};
  padding: 23px 0 16px;

  color: ${({ color }) => color};
`;

const GroupText = styled.div`
  white-space: nowrap;
  overflow: hidden;
  text-overflow: ellipsis;
`;

const Container = styled.div`
  flex: 1;
  min-width: 400px;
  height: calc(100% - ${HEADER_HEIGHT});
`;

const Wrapper = styled.div`
  display: flex;
  flex-direction: row;
  flex-wrap: wrap;
  height: 100%;
  gap: 15px;
`;

const GridContainer = styled(Grid)``;
