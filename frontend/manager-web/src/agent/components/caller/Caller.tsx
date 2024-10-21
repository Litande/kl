import styled from "styled-components";
import callService from "agent/services/connection/callService";
import React from "react";
import { disconnectAction } from "agent/actions/connectionActions";
import { STATES } from "agent/actions/changeStateAction";
import Button from "components/button/Button";

const Caller = () => {
  const { getLead } = callService();
  const lead = getLead();

  const closeCall = () => {
    disconnectAction(STATES.WAITING_STATE);
  };

  console.warn(lead);

  return (
    <Wrap>
      <label>Phone:</label>
      <div>{lead?.phone}</div>
      <label>Name:</label>
      <div>
        {lead?.firstName} {lead?.lastName}
      </div>
      <label>WebRtcEndpointUrl:</label>
      <div>{lead?.webRtcEndpointUrl}</div>
      <div>
        <audio controls autoPlay id="audioCtl" />
      </div>
      <ButtonsWrap>
        <Button onClick={closeCall}>Close</Button>
      </ButtonsWrap>
    </Wrap>
  );
};

export default Caller;

const Wrap = styled.div`
  display: flex;
  flex-direction: column;
  gap: 0.5rem;
`;

const ButtonsWrap = styled.div`
  display: flex;
  flex-direction: row;
  margin-bottom: 1rem;
  margin-left: 1rem;
  gap: 1rem;
`;
