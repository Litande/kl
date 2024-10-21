import React, { useEffect, useState } from "react";
import { AxiosResponse } from "axios";
import { NavLink } from "react-router-dom";
import styled, { useTheme } from "styled-components";
import Button from "components/button/Button";
import { PageTitle } from "components/layout/Layout";
import leadsApi from "services/api/leads";
import useConnections from "services/websocket/useConnections";
import { ROUTES } from "router/enums";
import unionBy from "lodash.unionby";

import GroupGrid from "./groupGrid/GroupGrid";
import Blacklist from "./Blacklist";
import LoadingOverlay from "components/loadingOverlay/LoadingOverlay";
import { TRACKING_WS } from "services/websocket/const";

export type Lead = {
  leadId: string;
  leadScore: number;
};

export interface ILeadGroup {
  groupName: string;
  leadsCount: number;
  leads: Lead[];
}

const Leads = () => {
  const [data, setData] = useState<ILeadGroup[]>([]);
  const [isBlacklistShown, setIsBlacklistShown] = useState(false);
  const [isLoading, setIsLoading] = useState(true);
  const theme = useTheme();
  const g = theme.colors.leadGroups;
  const colors = [g.green, g.blue, g.darkBlue, g.dark, g.orange, g.red, g.grey];

  useEffect(() => {
    leadsApi
      .getLeadsQueue()
      .then(({ data }: AxiosResponse<ILeadGroup[]>) => {
        setData(data);
      })
      .finally(() => {
        setIsLoading(false);
      });
  }, []);

  const showBlacklist = () => setIsBlacklistShown(true);

  const hideBlacklist = () => {
    leadsApi.getLeadsQueue().then(({ data }: AxiosResponse<ILeadGroup[]>) => {
      setData(data);
    });
    setIsBlacklistShown(false);
  };

  const updateBlackList = () => {
    setIsBlacklistShown(false);
  };

  const onStatsUpdate = (newData: ILeadGroup[]) => {
    const updatedData = unionBy(data, newData, "groupName");

    setData(updatedData);
  };

  useConnections(TRACKING_WS, [{ chanelName: "leads_queue", onMessage: onStatsUpdate }], [data]);

  return (
    <Container>
      <Header>
        <TitleHeader>Leads Queues</TitleHeader>
        <ActionContainer>
          <NavLink to={ROUTES.LEADS_SEARCH}>
            <Button>Search Leads</Button>
          </NavLink>
          <Button onClick={showBlacklist}>
            <ButtonIcon className="icon-blacklist"></ButtonIcon>
            Black List
          </Button>
          <Button>
            <ButtonIcon className="icon-download"></ButtonIcon>
            Import Leads
          </Button>
        </ActionContainer>
      </Header>
      <Wrapper>
        {data.map((leadsGroup: ILeadGroup, index) => (
          <GroupGrid key={leadsGroup.groupName} color={colors[index]} {...leadsGroup} />
        ))}
        {isLoading && <LoadingOverlay />}
      </Wrapper>

      {isBlacklistShown && <Blacklist onSave={updateBlackList} onClose={hideBlacklist} />}
    </Container>
  );
};

export default Leads;

const HEADER_HEIGHT = "72px";

const TitleHeader = styled(PageTitle)`
  margin: 0;
  border-bottom: none;
`;

const Header = styled.div`
  display: flex;
  align-items: center;
  justify-content: space-between;
  height: ${HEADER_HEIGHT};
  border-bottom: 1px solid ${({ theme }) => theme.colors.border.primary};
`;

const ActionContainer = styled.div`
  display: flex;
  flex-direction: row;
  gap: 16px;
`;

const Container = styled.div`
  height: 100%;
`;

const Wrapper = styled.div`
  position: relative;
  display: flex;
  flex-direction: row;
  flex-wrap: nowrap;
  height: calc(100% - ${HEADER_HEIGHT});
  gap: 15px;
  overflow-x: auto;
`;

const ButtonIcon = styled.i`
  margin: 0 8px 0 0;
  font-size: 18px;
`;
