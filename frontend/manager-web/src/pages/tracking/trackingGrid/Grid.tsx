import { useEffect, useCallback, useState, FC } from "react";
import { IsFullWidthRowParams, RowHeightParams } from "ag-grid-community";
import styled from "styled-components";
import { AxiosResponse } from "axios";
import unionBy from "lodash.unionby";
import Grid from "components/grid/Grid";

import trackingApi from "services/api/tracking";
import useConnections from "services/websocket/useConnections";
import columnDefs from "./ColumnDefsNew";

import { IRow } from "./types";

import { CH_AGENTS_LIST } from "../constants";
import AgentsFullWidthCellRenderer from "./cells/AgentsFullWidthCellRenderer";
import Settings from "../settings/Settings";
import { AgentStatusStr } from "../../../types";
import { Layout } from "components/layoutButton/types";
import { TRACKING_WS } from "services/websocket/const";

const prepareData = data => {
  const mockDataCopy = [...data, ...data];

  for (let i = data.length - 1; i >= 0; i--) {
    mockDataCopy[i * 2] = mockDataCopy[i];
    mockDataCopy[i * 2 + 1] = {
      ...mockDataCopy[i],
      id: mockDataCopy[i].id + "-copy",
      fullWidth: true,
    };
  }

  return mockDataCopy;
};

const filterByTeams = (rawData, teams) => {
  if (!teams.length) return rawData;
  return rawData.filter(({ teamIds }) =>
    teamIds.some(id => teams.find(({ teamId }) => id === teamId))
  );
};

const filterOffline = rawData => {
  return rawData.filter(item => item.state !== AgentStatusStr.Offline);
};

type Props = {
  layout?: Layout;
};

const AgentsGrid: FC<Props> = ({ layout = Layout.Three }) => {
  const [isSettingsModalShown, setIsSettingsModalShown] = useState<boolean>(false);
  const [data, setData] = useState<IRow[]>([]);
  const [isLoading, setIsLoading] = useState(true);
  const [selectedTeams, setSelectedTeams] = useState<{ teamId: number; name: string }[]>([]);

  const handleUpdate = (wsData: IRow[]) => {
    setData(unionBy(wsData, data, "id"));
  };

  useConnections(TRACKING_WS, [{ chanelName: CH_AGENTS_LIST, onMessage: handleUpdate }], [data]);

  const handleSaveSettings = ({ teams }) => {
    setSelectedTeams(teams);
    setIsSettingsModalShown(false);
  };

  useEffect(() => {
    trackingApi
      .getLiveTracking()
      .then(({ data }: AxiosResponse<IRow[]>) => {
        setData(data);
      })
      .finally(() => {
        setIsLoading(false);
      });
  }, []);

  const isFullWidthRow = useCallback((params: IsFullWidthRowParams) => {
    return params.rowNode.data.fullWidth;
  }, []);

  const getRowHeight = useCallback((params: RowHeightParams) => {
    // you can have normal rows and full width rows any height that you want
    const isFullWidth = params.node.data.fullWidth;
    if (isFullWidth) {
      return 38;
    }
  }, []);
  const rowData = prepareData(filterOffline(filterByTeams(data, selectedTeams)));

  return (
    <Container layout={layout}>
      <HeaderWrapper>
        <Header>Live Tracking</Header>
        <SettingsIcon className="icon-settings" onClick={() => setIsSettingsModalShown(true)} />
      </HeaderWrapper>
      <GridContainer
        isLoading={isLoading}
        rowData={rowData}
        columnDefs={columnDefs}
        rowMultiSelectWithClick
        isFullWidthRow={isFullWidthRow}
        getRowHeight={getRowHeight}
        fullWidthCellRenderer={AgentsFullWidthCellRenderer}
      />
      {isSettingsModalShown && (
        <Settings
          handleClose={() => setIsSettingsModalShown(false)}
          selectedTeams={selectedTeams}
          handleSave={handleSaveSettings}
        />
      )}
    </Container>
  );
};

export default AgentsGrid;

const Container = styled.div<{ layout }>`
  width: 100%;
  height: ${({ layout }) => (layout === Layout.One ? "100vh" : "50vh")};
`;

const HeaderWrapper = styled.div`
  display: flex;
  justify-content: space-between;
  align-items: center;
`;

const Header = styled.h3`
  box-sizing: border-box;
  height: 60px;
  padding: 15px 0;
`;

const SettingsIcon = styled.i`
  font-size: 20px;
  padding: 7px;
  border-radius: 4px;
  background: ${({ theme }) => theme.colors.btn.secondary};
  color: ${({ theme }) => theme.colors.btn.primary};
  cursor: pointer;
`;

const GridContainer = styled(Grid)`
  width: 100%;
  height: calc(100% - 60px);
`;
