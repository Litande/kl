import styled from "styled-components";
import Button from "components/button/Button";
import { useAgent } from "data/user/useAgent";
import { PageWrapContent } from "components/layout/AgentLayout";
import useAgentStatus from "hooks/useAgentStatus";
import { AgentStatus } from "data/user/types";
import { useMemo } from "react";

const ClockOutState = () => {
  const { agent } = useAgent();
  const { nextAgentStatus } = useAgentStatus();
  const onClick = () => {
    agent.gotoOffline();
  };

  const isNextOffline = useMemo(() => {
    return nextAgentStatus === AgentStatus.Offline;
  }, [nextAgentStatus]);

  return (
    <PageWrapContent>
      <GreetingWrap>
        <h1>{`${agent.firstName} ${agent.lastName}`} Session</h1>
        <ClockOutButton disabled={isNextOffline} onClick={onClick} data-testid="clock-out-button">
          Clock Out
        </ClockOutButton>
      </GreetingWrap>
    </PageWrapContent>
  );
};

export default ClockOutState;

const Wrap = styled.div<{ imageUrl: string }>`
  display: flex;
  align-items: center;
  justify-content: center;
  width: 100%;
  height: 100%;
  // background: url(${({ imageUrl }) => imageUrl});
`;

const GreetingWrap = styled.div`
  min-height: 273px;
  min-width: 500px;
  box-sizing: border-box;
  padding: 90px 90px 60px;
  display: flex;
  flex-direction: column;
  align-items: center;
  background: ${({ theme }) => theme.colors.bg.ternary};
  box-shadow: 0px 4px 4px ${({ theme }) => theme.colors.border.primary};
  border-radius: 4px;
`;

const ClockOutButton = styled(Button)`
  width: 174px;
  margin-top: 3rem;
`;
