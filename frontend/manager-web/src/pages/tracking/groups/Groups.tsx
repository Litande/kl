import React, { FC, useEffect, useState } from "react";
import styled from "styled-components";
import { NavLink } from "react-router-dom";

import InfoCard, { ICardData } from "components/infoCard/InfoCard";
import { useTheme } from "styled-components";
import { AxiosResponse } from "axios";
import unionBy from "lodash.unionby";

import useConnections from "services/websocket/useConnections";
import trackingApi from "services/api/tracking";

import { CH_LEAD_GROUP, LEADS_GROUPS_SECTION_HEIGHT } from "../constants";
import LoadingOverlay from "components/loadingOverlay/LoadingOverlay";
import { Layout } from "components/layoutButton/types";
import { ROUTES } from "router/enums";
import Button from "components/button/Button";
import { STATISTIC_WS } from "services/websocket/const";

interface IGroup {
  name: string;
  leadQueueId: number;
  agents: number;
  ratio: number;
  dropRatePercentage: number; // string??
  leadsCount: number;
  dropRateLowerThreshold: number;
  dropRateUpperThreshold: number;
  dropRatePeriod: number;
  maxRatio: number | null;
  minRatio: number | null;
  ratioFreezeTime: number;
  ratioStep: number;
  type: string;
}

enum AttributesKeys {
  agents = "agents",
  ratio = "ratio",
  dropRatePercentage = "dropRatePercentage",
  leadsCount = "leadsCount",
}

enum attributesLabels {
  agents = "Agents",
  ratio = "Ratio",
  dropRatePercentage = "Drop rate",
  leadsCount = "New leads",
}

const attributes = [
  AttributesKeys.agents,
  AttributesKeys.ratio,
  AttributesKeys.dropRatePercentage,
  AttributesKeys.leadsCount,
];

const parseGroup = (group: IGroup): ICardData => {
  const attr = attributes.map(key => ({
    key,
    label: attributesLabels[key],
    value: group[key],
    editable: key === AttributesKeys.ratio,
  }));
  const dropRateParameters = {
    dropRateUpperThreshold: group.dropRateUpperThreshold,
    dropRateLowerThreshold: group.dropRateLowerThreshold,
    dropRatePeriod: group.dropRatePeriod,
    ratioStep: group.ratioStep,
    maxRatio: group.maxRatio,
    minRatio: group.minRatio,
    ratioFreezeTime: group.ratioFreezeTime,
  };

  return { name: group.name, id: group.leadQueueId, attributes: attr, dropRateParameters };
};

type Props = {
  layout?: Layout;
};

const Groups: FC<Props> = ({ layout = Layout.Three }) => {
  const [isLoading, setIsLoading] = useState(true);
  const [groups, setGroups] = useState<IGroup[]>([]);

  const theme = useTheme();
  const g = theme.colors.leadGroups;
  const colors = [g.green, g.blue, g.darkBlue, g.dark, g.orange, g.red, g.grey];

  const onGroupUpdates = (data: IGroup[]) => {
    const updatedData = unionBy(groups, data, "name");

    setGroups(updatedData);
  };

  const handleAttributeEdit = ({ id }, attribute) => {
    const ratio = attribute[AttributesKeys.ratio];

    if (ratio) {
      const group = groups.find(({ leadQueueId }) => leadQueueId === id);
      group.ratio = ratio;
      setGroups([...groups]);
      trackingApi.updateLeadQueueRatio({ id, ratio });
    }
  };

  useEffect(() => {
    trackingApi
      .getGroups()
      .then(({ data }: AxiosResponse<IGroup[]>) => {
        setGroups(data);
      })
      .finally(() => {
        setIsLoading(false);
      });
  }, []);

  useConnections(
    STATISTIC_WS,
    [{ chanelName: CH_LEAD_GROUP, onMessage: onGroupUpdates }],
    [groups]
  );

  const renderedHeaderInfoDependsOnLayout = () => (
    <HeaderDependsOnLayout layout={layout}>
      New Leads Statistics
      <NavLink to={`/${ROUTES.LEADS}`}>
        <Button>View Leads Queue</Button>
      </NavLink>
    </HeaderDependsOnLayout>
  );

  return (
    <Container>
      <Header>
        <Title>Lead Groups</Title>
        {renderedHeaderInfoDependsOnLayout()}
      </Header>
      <ScrollContainer>
        <Cards>
          {groups.map((group, index) => (
            <InfoCardContainer
              data={parseGroup(group)}
              onAttributeEdit={handleAttributeEdit}
              key={group.name}
              color={colors[index]}
            />
          ))}
        </Cards>
        {isLoading && <LoadingOverlay />}
      </ScrollContainer>
    </Container>
  );
};

export default Groups;

const ScrollContainer = styled.div`
  position: relative;
  box-sizing: border-box;
  width: 100%;
  height: ${LEADS_GROUPS_SECTION_HEIGHT}px;
  padding: 15px 17px 15px 17px;
  border: 1px solid ${({ theme }) => theme.colors.border.primary};
  border-radius: 4px;
  background: ${({ theme }) => theme.colors.bg.light};
  overflow-y: auto;
`;

const Container = styled.div`
  flex: 1.11;
  overflow: hidden;
`;

const Header = styled.h3`
  box-sizing: border-box;
  height: 60px;
  padding: 15px 0;
  display: flex;
`;

const Cards = styled.div`
  box-sizing: border-box;
  display: flex;
  flex-wrap: wrap;
  gap: 16px;
  justify-content: space-between;
  min-width: 700px;
  border-radius: 8px;
`;

const InfoCardContainer = styled(InfoCard)`
  width: calc(50% - 8px);
`;

const HeaderDependsOnLayout = styled.div<{ layout }>`
  align-items: center;
  display: ${({ layout }) => (layout === Layout.Two || layout === Layout.One ? "flex" : "none")};
  width: 50%;
  justify-content: space-between;
`;

const Title = styled.div`
  width: 50%;
`;
