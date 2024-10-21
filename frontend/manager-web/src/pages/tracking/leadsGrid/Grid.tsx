import React, { useState, useEffect, useMemo, FC } from "react";
import { AxiosResponse } from "axios";
import { GetRowIdFunc, GetRowIdParams, GridReadyEvent } from "ag-grid-community";
import styled from "styled-components";
import Grid from "components/grid/Grid";

import columnDefs from "./columnsDefs";

import trackingApi from "services/api/tracking";
import useConnections from "services/websocket/useConnections";

import { CH_LEAD_STATS, LEADS_STATS_SECTION_HEIGHT } from "../constants";

import { IRow } from "./types";
import Button from "components/button/Button";
import { ROUTES } from "router/enums";
import { NavLink } from "react-router-dom";
import { Layout } from "components/layoutButton/types";
import { STATISTIC_WS } from "services/websocket/const";

type Props = {
  layout?: Layout;
};

const AgentsGrid: FC<Props> = ({ layout = Layout.Three }) => {
  const [isLoading, setIsLoading] = useState(true);
  const [data, setData] = useState([]);

  const onGridReady = (params: GridReadyEvent) => {
    params.api.sizeColumnsToFit();
  };

  const onStatsUpdate = (data: IRow[]) => {
    setData(data);
  };

  useEffect(() => {
    trackingApi
      .getLeadsStats()
      .then(({ data }: AxiosResponse<IRow[]>) => {
        // Todo: remove data.result
        setData(data.result || data);
      })
      .finally(() => {
        setIsLoading(false);
      });
  }, []);

  useConnections(STATISTIC_WS, [{ chanelName: CH_LEAD_STATS, onMessage: onStatsUpdate }], [data]);

  const getRowId = useMemo<GetRowIdFunc>(() => (params: GetRowIdParams) => params.data.country, []);

  return (
    <Container layout={layout}>
      <Header>
        New Leads Statistics
        <NavLink to={`/${ROUTES.LEADS}`}>
          <Button>View Leads Queue</Button>
        </NavLink>
      </Header>
      <GridContainer
        isLoading={isLoading}
        rowData={data}
        columnDefs={columnDefs}
        onGridReady={onGridReady}
        getRowId={getRowId}
      />
    </Container>
  );
};

export default AgentsGrid;

const Container = styled.div<{ layout }>`
  flex: 0.89;
  overflow: hidden;
  display: ${({ layout }) => (layout === Layout.Two || layout === Layout.One ? "none" : "block")};
`;

const Header = styled.h3`
  display: flex;
  justify-content: space-between;
  align-items: center;
  box-sizing: border-box;
  height: 60px;
  padding: 15px 0;
`;

const GridContainer = styled(Grid)`
  width: 100%;
  height: ${LEADS_STATS_SECTION_HEIGHT}px;
`;
