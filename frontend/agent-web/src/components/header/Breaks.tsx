import React, { useEffect, useMemo, useState } from "react";
import styled from "styled-components";
import { useAgent } from "data/user/useAgent";
import Button from "components/button/Button";
import Tooltip from "components/tooltipPopover/TooltipPopover";
import { AgentStatus, Break } from "data/user/types";
import useBreaks from "hooks/useBreaks";
import moment from "moment";
import useAgentStatus from "hooks/useAgentStatus";

const Breaks = () => {
  const { agent } = useAgent();

  const { activeBreakTimer, breaks } = useBreaks();
  const { nextAgentStatus, agentStatus } = useAgentStatus();

  const isNextOffline = useMemo(() => {
    return nextAgentStatus === AgentStatus.Offline;
  }, [nextAgentStatus]);

  const toggleBreak = item => {
    if (!item.isAvailable) return;

    if (item.isScheduled) {
      agent.unScheduleBreak(item.id);
    } else {
      agent.scheduleBreak(item);
    }
  };

  const goOffline = () => {
    agent.gotoOffline();
  };

  const goOnline = () => {
    agent.finishBreak();
  };

  const renderTime = ({ isAvailable, startedAt, duration, isActive }: Break) => {
    if (isActive && activeBreakTimer) {
      return <BreakTime>{Math.ceil(activeBreakTimer / 60)} minutes left</BreakTime>;
    }

    if (isAvailable) {
      return null;
    }

    const startDate = moment(startedAt);
    const endDate = moment(startedAt).add(duration, "minutes");

    return (
      <BreakTime>
        {moment(startDate).format("HH:mm")}-{moment(endDate).format("HH:mm")}
      </BreakTime>
    );
  };

  const renderContent = () => {
    if (agentStatus === AgentStatus.Offline) {
      return <Content>You&apos;re not able to take a break</Content>;
    }

    return (
      <Content>
        {breaks.map((item, index) => {
          return (
            <BreakItem
              isAvailable={item.isAvailable}
              key={item.id}
              onClick={() => toggleBreak(item)}
            >
              <div>{index + 1}</div>
              <div>{item.label}</div>
              <BreakButton>{item.isScheduled ? "Taken" : "Take"}</BreakButton>
              {renderTime(item)}
            </BreakItem>
          );
        })}
      </Content>
    );
  };

  const renderTooltipContent = () => {
    return (
      <Container>
        <Header>Action</Header>
        {renderContent()}
        {agentStatus !== AgentStatus.Offline && (
          <Footer>
            {agentStatus === AgentStatus.Break ? (
              <Button onClick={goOnline}>Clock back in</Button>
            ) : (
              <Button disabled={isNextOffline} onClick={goOffline}>
                Clock out
              </Button>
            )}
          </Footer>
        )}
      </Container>
    );
  };

  return (
    <>
      <StyledTooltip width={380} tooltip={renderTooltipContent()} position="bottom">
        <div>
          <BreaksButton>
            <BreakIcon className="icon-ic-breaks" />
            Breaks
          </BreaksButton>
        </div>
      </StyledTooltip>
    </>
  );
};

export default Breaks;

const BreaksButton = styled(Button)`
  background: transparent;
  margin: 0 35px 0 80px;
  border: 1px solid ${({ theme }) => theme.colors.btn.primary_hovered};
`;

const BreakIcon = styled.i`
  margin: -2px 15px 0 -5px;
  font-size: 24px;
`;

const StyledTooltip = styled(Tooltip)`
  margin: 15px 0 0 -140px;
  background: ${({ theme }) => theme.colors.bg.ternary};

  &:after {
    left: calc(100% - 20px);
    background: ${({ theme }) => theme.colors.bg.primary};
  }
`;

const Container = styled.div`
  box-shadow: 0px 2px 8px rgba(0, 0, 0, 0.16);
  border-radius: 4px;
  ${({ theme }) => theme.typography.body1}
`;

const Header = styled.div`
  padding: 16px 20px;
  background: ${({ theme }) => theme.colors.bg.primary};
  color: ${({ theme }) => theme.colors.btn.primary};
  ${({ theme }) => theme.typography.subtitle3};
`;

const Content = styled.div`
  display: flex;
  flex-direction: column;
  gap: 8px;
  padding: 16px;
`;

const Footer = styled.div`
  display: flex;
  justify-content: center;
  padding: 16px;
  margin: 0 16px;
  border-top: 1px solid ${({ theme }) => theme.colors.border.primary};
`;

const BreakItem = styled.div<{ isAvailable: boolean }>`
  display: flex;
  justify-content: space-between;
  align-items: center;
  opacity: ${({ isAvailable }) => (isAvailable ? "1" : "0.3")};
`;

const BreakButton = styled(Button)`
  width: 100px;
`;

const BreakTime = styled.div`
  min-width: 100px;
`;
