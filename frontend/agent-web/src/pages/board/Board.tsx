import React, { useMemo } from "react";
import styled from "styled-components";
import ClockInState from "components/board/states/ClockInState";
import useAgentStatus from "hooks/useAgentStatus";
import { AgentStatus, CallType } from "data/user/types";
import ClockOutState from "components/board/states/ClockOutState";
import WaitingsState from "components/board/states/WaitingsState";
import BreakState from "components/board/states/BreakState";
import CallingState from "components/board/states/CallingState";
import GlobalLoader from "components/loader/GlobalLoader";

const states = {
  [AgentStatus.FillingFeedback]: <ClockOutState />,
  [AgentStatus.Offline]: <ClockInState />,
  [AgentStatus.Break]: <BreakState />,
  [AgentStatus.InTheCall]: <CallingState />,
  [AgentStatus.Unknown]: <GlobalLoader />,
};

const Board = () => {
  const { agentStatus, callType } = useAgentStatus();
  // TODO Temporary (dialing.ts)
  const content = useMemo(() => {
    if (callType === CallType.Manual) {
      return <ClockInState />;
    }
    const res = states[agentStatus];
    return res ? res : <WaitingsState />;
  }, [agentStatus, callType]);

  return <Wrap>{content}</Wrap>;
};

export default Board;

const Wrap = styled.div`
  display: flex;
  width: 100%;
  height: 90vh;
`;
