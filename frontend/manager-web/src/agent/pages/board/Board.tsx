import React, { useEffect, useMemo, useState } from "react";
import styled from "styled-components";

import eventDomDispatcher from "services/events/EventDomDispatcher";

import MainState from "agent/pages/board/states/MainState";
import { CHANGE_STATE_EVENT, STATES } from "agent/actions/changeStateAction";
import WaitingComponent from "agent/components/caller/WaitingComponent";
import callService from "agent/services/connection/callService";
import Caller from "agent/components/caller/Caller";
import Button from "components/button/Button";
import { disconnectAction } from "agent/actions/connectionActions";

const Board = () => {
  const { addEventListener, removeEventListener } = eventDomDispatcher();
  const [curState, setCurState] = useState(STATES.MAIN_STATE);

  const service = callService();

  const states = {
    [STATES.MAIN_STATE]: <MainState />,
    [STATES.WAITING_STATE]: <WaitingComponent />,
    [STATES.CALLING_STATE]: <Caller />,
  };

  const onStateUpdate = (event: CustomEvent) => {
    const newState = event.detail.state;
    setCurState(newState);
  };

  const Component = useMemo(() => {
    return states[curState];
    // eslint-disable-next-line react-hooks/exhaustive-deps
  }, [curState]);

  useEffect(() => {
    service.init();
    addEventListener(CHANGE_STATE_EVENT, onStateUpdate);
    return () => {
      service.destroy();
      removeEventListener(CHANGE_STATE_EVENT, onStateUpdate);
    };
    // eslint-disable-next-line react-hooks/exhaustive-deps
  }, []);

  const onClockOut = () => {
    disconnectAction(STATES.MAIN_STATE);
  };

  return (
    <Wrap>
      {Component}
      {curState !== STATES.MAIN_STATE && (
        <ClockOutWrap>
          <Button variant={"secondary"} onClick={onClockOut}>
            Clock Out
          </Button>
        </ClockOutWrap>
      )}
    </Wrap>
  );
};

export default Board;

const Wrap = styled.div`
  display: flex;
  width: 100%;
  height: 100%;
`;

const ClockOutWrap = styled.div`
  position: absolute;
  right: 20px;
  top: 20px;
`;
