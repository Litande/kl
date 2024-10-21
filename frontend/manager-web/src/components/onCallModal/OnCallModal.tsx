import styled from "styled-components";
import Draggable from "react-draggable";
import { useEffect } from "react";
import moment from "moment";

import CallService from "services/callService/CallService";
import { useDraggable } from "hooks/useDraggable";
import Button from "components/button/Button";
import useTimer from "hooks/useTimer";
import useMute from "hooks/useMute";

export default function OnCallModal() {
  const { defaultPosition, draggableRef, stopHandler, handle, handleClassName } = useDraggable({});
  const { isMute, toggleMute } = useMute();
  const callStatus = CallService.getInstance().status.getValue();
  const callInfo = CallService.getInstance().callInfo || {
    phone: "380674837879",
    mode: "ListenOnly",
    firstName: "Ivan",
    lastName: "Komar",
    callTime: "2023-03-03T12:00:46.229263+00:00",
    agent: "Valeria Rise",
  };

  const { total, start, pause } = useTimer({
    timestampStart: moment(callInfo?.callTime).valueOf(),
  });

  useEffect(() => {
    start();
    return () => pause();
  }, []);

  return (
    <Draggable
      nodeRef={draggableRef}
      onStop={stopHandler}
      handle={handle}
      bounds="parent"
      defaultPosition={defaultPosition}
    >
      <Wrapper className={handleClassName} ref={draggableRef}>
        <Header>
          {callStatus} - {callInfo?.mode}
        </Header>
        <Main>
          <ContentBlock>
            <InfoTitle>Agent:</InfoTitle>
            <InfoText>{callInfo.agent}</InfoText>
          </ContentBlock>
          <span>__</span>
          <ContentBlock>
            <InfoTitle>Lead:</InfoTitle>
            <InfoText>
              {!callInfo?.firstName && !callInfo?.lastName
                ? callInfo?.phone
                : `${callInfo?.firstName} ${callInfo?.lastName}`}
            </InfoText>
          </ContentBlock>
          <ContentBlock>
            <InfoTitle>Time:</InfoTitle>
            <InfoText>{total}</InfoText>
          </ContentBlock>
        </Main>
        <Footer>
          <Button
            style={{ display: "none" }}
            onClick={() => {
              CallService.getInstance().stop();
            }}
            variant="secondary"
          >
            Hold
          </Button>
          <Button onClick={toggleMute} variant={isMute ? "primary" : "secondary"}>
            {isMute ? "Unmute" : "Mute"}
          </Button>
          <Button
            onClick={() => {
              CallService.getInstance().endCall();
            }}
            variant="primary"
          >
            End Call
          </Button>
        </Footer>
      </Wrapper>
    </Draggable>
  );
}

const Wrapper = styled.div`
  display: flex;
  flex-direction: column;
  position: absolute;
  z-index: 100;
  width: 454px;
  height: 195px;
  background: #ffffff;
  box-shadow: 0px 4px 8px rgba(0, 0, 0, 0.25);
  border-radius: 4px;
`;

const Header = styled.header`
  background-color: ${({ theme }) => theme.colors.bg.primary};
  ${({ theme }) => theme.typography.subtitle3};
  text-transform: uppercase;
  color: #fff;
  border-radius: 4px;
  padding: 1rem;
`;

const Main = styled.div`
  display: flex;
  flex-direction: row;
  justify-content: space-around;
  align-items: center;
  padding: 1rem;
`;

const ContentBlock = styled.div`
  display: flex;
  flex-direction: column;
`;

const InfoTitle = styled.div`
  ${({ theme }) => theme.typography.smallText1};
  color: ${({ theme }) => theme.colors.fg.secondary_light};
  margin: 0.5rem 0;
`;
const InfoText = styled.div`
  ${({ theme }) => theme.typography.body1};
  color: #000;
`;

const Footer = styled.footer`
  display: flex;
  justify-content: center;
  align-items: center;
  gap: 20px;
  margin: auto;
`;
