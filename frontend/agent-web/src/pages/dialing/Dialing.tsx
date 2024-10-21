import React, { useMemo } from "react";
import { PageWrap } from "components/layout/AgentLayout";
import { AgentStatus, CallType } from "data/user/types";
import ClockInState from "components/board/states/ClockInState";
import WaitingsState from "components/board/states/WaitingsState";
import useAgentStatus from "hooks/useAgentStatus";
import FillingFeedbackState from "components/board/states/FillingFeedbackState";
import CallingState from "components/board/states/CallingState";
import BreakState from "components/board/states/BreakState";
import GlobalLoader from "components/loader/GlobalLoader";

const states = {
  [AgentStatus.WaitingForTheCall]: <WaitingsState />,
  [AgentStatus.Offline]: <ClockInState />,
  [AgentStatus.Break]: <BreakState />,
  [AgentStatus.InTheCall]: <CallingState />,
  [AgentStatus.FillingFeedback]: <FillingFeedbackState />,
  [AgentStatus.Unknown]: <GlobalLoader />,
};

const Dialing = () => {
  const { agentStatus, callType } = useAgentStatus();

  // TODO Temporary (board.ts)
  const content = useMemo(() => {
    if (callType === CallType.Manual) {
      return <ClockInState />;
    }
    const res = states[agentStatus];
    return res ? res : <div> Hello </div>;
  }, [agentStatus]);

  return <PageWrap>{content}</PageWrap>;
};

export default Dialing;
