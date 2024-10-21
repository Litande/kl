import React, { useEffect, useMemo, useState } from "react";
import styled from "styled-components";
import { convertSecondsToTimeString } from "utils/timerUtils";
import { typography } from "globalStyles/theme/fonts";
import { useAgent } from "data/user/useAgent";
import { AgentStatus } from "data/user/types";
import useAgentStatus from "hooks/useAgentStatus";
import { FEEDBACK_TIME_DEFAULT } from "data/user/lead/types";

type ComponentProps = {
  label: string;
};
const PageTitle = ({ label }: ComponentProps) => {
  const { agent } = useAgent();
  const [time, setTime] = useState<number>(agent?.lead?.feedback?.timeToEnd?.getValue() || 0);
  const { agentStatus, updater } = useAgentStatus();

  useEffect(() => {
    let subscribe;
    if (agentStatus === AgentStatus.FillingFeedback) {
      subscribe = agent.lead?.feedback?.timeToEnd.subscribe(value => {
        setTime(value);
      });
    }
    return () => {
      subscribe && subscribe.unsubscribe();
    };
  }, [agentStatus, updater]);

  const isShowFeedbackTimer = useMemo(() => {
    return agentStatus === AgentStatus.FillingFeedback;
  }, [agentStatus]);

  const feedbackTimeoutPercent = useMemo(() => {
    const max = agent.settings.getValue().pageTimeout || FEEDBACK_TIME_DEFAULT;
    const cur = time;
    return Math.round(100 - (cur / max) * 100);
  }, [time]);

  return (
    <Wrap data-testid="page-title">
      {label}
      {isShowFeedbackTimer && (
        <TimerWrap>
          <Timer>FeedBack Timeout {convertSecondsToTimeString(time)}</Timer>
          <LoaderLine percent={feedbackTimeoutPercent} />
        </TimerWrap>
      )}
    </Wrap>
  );
};

export default PageTitle;

const Wrap = styled.h1`
  display: flex;
  flex-direction: row;
  white-space: nowrap;
  text-transform: uppercase;
  margin: 0 1.5rem 1.5rem 0;
  padding-bottom: 1rem;
  border-radius: 4px;
  border-bottom: 1px solid ${({ theme }) => theme.colors.border.primary};
`;

type LoaderPercent = {
  percent: number;
};

const LoaderLine = styled.div<LoaderPercent>`
  border-bottom: 3px solid rgba(82, 148, 195, 1);
  border-radius: 4px;
  width: ${props => `${100 - props.percent}%`};
`;

const TimerWrap = styled.div`
  min-width: 215px;
  display: flex;
  flex-direction: column;
  margin-left: auto;
  border: 1px solid rgba(0, 0, 0, 0.12);
  padding-top: 10px;
  padding-left: 1rem;
  padding-right: 1rem;
  ${typography.subtitle4};
  text-transform: none;
  color: black;
`;

const Timer = styled.div`
  display: flex;
  align-content: flex-start;
`;
