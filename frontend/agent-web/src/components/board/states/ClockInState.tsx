import React, { useState } from "react";
import styled from "styled-components";

import Button from "components/button/Button";
import boardBg from "images/board_bg.png";
import { useAgent } from "data/user/useAgent";
import { PageWrapContent } from "components/layout/AgentLayout";
import useAgentStatus from "hooks/useAgentStatus";
import { CallType } from "data/user/types";
import { handleMicrophonePermission, MICROPHONE_ERROR } from "utils/microphoneUtils";
import ErrorNotification from "components/error/ErrorNotification";
import { NO_INTERNET_CONNECTION_ERROR, isConnectionOnline } from "components/connection/utils";

const ClockInState = () => {
  const { agent } = useAgent();
  const { callType } = useAgentStatus();
  const [error, setError] = useState(null);

  const onClick = async () => {
    if (isConnectionOnline()) {
      await handleMicrophonePermission(
        () => {
          setError(null);
          callType !== CallType.Manual && agent.waitForCall();
        },
        () => setError(MICROPHONE_ERROR)
      );
    } else {
      setError(NO_INTERNET_CONNECTION_ERROR);
    }
  };

  return (
    <PageWrapContent>
      <Wrap imageUrl={boardBg}>
        <GreetingWrap>
          <h1>{`${agent.firstName} ${agent.lastName}`} Session</h1>
          <ClockInButton
            disabled={callType === CallType.Manual}
            onClick={onClick}
            data-testid="clock-in-button"
          >
            Clock In
          </ClockInButton>
        </GreetingWrap>
      </Wrap>
      {error && <ErrorNotification error={error} />}
    </PageWrapContent>
  );
};

export default ClockInState;

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
  box-shadow: 0 4px 4px ${({ theme }) => theme.colors.border.primary};
  border-radius: 4px;
`;

const ClockInButton = styled(Button)`
  width: 174px;
  margin-top: 3rem;
`;
