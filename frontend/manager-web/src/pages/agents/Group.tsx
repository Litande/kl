import React from "react";
import styled, { css } from "styled-components";
import AgentCard from "components/agentCard/AgentCard";
import { columnDefs } from "./constants";
import { useSelector } from "react-redux";
import { filteredAgentsSelector } from "./selector";
import useBehavior from "hooks/useBehavior";
import behavior from "./behavior";

const Group = () => {
  const filteredAgents = useSelector(filteredAgentsSelector);
  const { getGroups } = useBehavior(behavior);

  const onEditAgent = () => {
    getGroups();
  };

  return (
    <Container>
      {columnDefs.map(({ key, sort }) => {
        const agentsByGroup = filteredAgents.filter(agent =>
          Array.isArray(key) ? key.some(i => i === agent.state) : agent.state === key
        );

        if (sort) agentsByGroup.sort(sort);

        return (
          <Column key={Array.isArray(key) ? key.join("-") : key} hasScroll={Array.isArray(key)}>
            {agentsByGroup.map(agent => (
              <StyledCard
                key={agent.id}
                data={{ ...agent, status: agent.state }}
                onEditAgent={onEditAgent}
              />
            ))}
          </Column>
        );
      })}
    </Container>
  );
};

export default Group;

const StyledCard = styled(AgentCard)`
  width: 100%;
`;

const Container = styled.div`
  display: flex;
  padding: 16px;
  gap: 15px;
  background: ${({ theme }) => theme.colors.bg.light};
  border-radius: 0 0 4px 4px;
`;

const scrollCSS = css`
  max-height: 350px;
  overflow-y: auto;
`;

const Column = styled.div<{ hasScroll: boolean }>`
  width: calc(25% - 16px);
  flex: 1;
  display: flex;
  flex-direction: column;
  gap: 16px;
  ${({ hasScroll }) => hasScroll && scrollCSS};
`;
