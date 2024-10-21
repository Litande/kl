import React, { useEffect } from "react";
import { PageTitle } from "components/layout/Layout";
import GroupWrapper from "./GroupWrapper";
import Group from "./Group";
import Filters from "./Filters";
import styled from "styled-components";
import behavior from "./behavior";
import useBehavior from "hooks/useBehavior";
import { agentsSelector, groupsSelector, isLoadingSelector } from "./selector";
import { useSelector } from "react-redux";
import useConnections from "services/websocket/useConnections";
import { CH_AGENTS_LIST } from "pages/tracking/constants";
import LoadingOverlay from "components/loadingOverlay/LoadingOverlay";
import { TRACKING_WS } from "services/websocket/const";

const Agents = () => {
  const { getGroups, getTags, getTeams, updateAgents } = useBehavior(behavior);
  const isLoading = useSelector(isLoadingSelector);
  const groups = useSelector(groupsSelector);
  const agents = useSelector(agentsSelector);

  useEffect(() => {
    getGroups();
    getTags();
    getTeams();
    // eslint-disable-next-line react-hooks/exhaustive-deps
  }, []);

  const handleUpdate = data => {
    updateAgents({ data, agents });
  };

  useConnections(
    TRACKING_WS,
    [{ chanelName: CH_AGENTS_LIST, onMessage: handleUpdate }],
    [groups, agents]
  );

  return (
    <Container>
      <StyledPageTitle>
        <div>Agents</div>
        <Filters onAddAgent={getGroups} />
      </StyledPageTitle>
      <GroupsContainer>
        {groups.map(group => (
          <GroupWrapper component={<Group />} key={group.value} group={group} />
        ))}
        {isLoading && <LoadingOverlay />}
      </GroupsContainer>
    </Container>
  );
};

const Container = styled.div`
  display: flex;
  flex-direction: column;
  min-width: 1000px;
  height: 100%;
  overflow: auto;
`;

const GroupsContainer = styled.div`
  position: relative;
  display: flex;
  flex-direction: column;
  height: 100%;
  gap: 1rem;
`;

const StyledPageTitle = styled(PageTitle)`
  display: flex;
  flex-wrap: nowrap;
  align-items: flex-end;
  justify-content: space-between;
  margin: -1rem 0 1.5rem 0;
  padding: 1rem 0;
`;

export default Agents;
