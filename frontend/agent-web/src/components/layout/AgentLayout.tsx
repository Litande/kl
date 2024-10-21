import React, { useEffect, useMemo, useState } from "react";
import styled from "styled-components";
import Header from "components/header/Header";
import Navigation from "components/navigation/Navigation";
import { useAgent } from "data/user/useAgent";
import { useNavigate } from "react-router";
import { AgentStatus, CallType } from "data/user/types";
import { ROUTES } from "router/enums";
import useAgentStatus from "hooks/useAgentStatus";
import ErrorNotification from "components/error/ErrorNotification";
import ManualWidget from "components/manual/ManualWidget";
import EventDomDispatcher from "services/events/EventDomDispatcher";
import { LOGOUT_EVENT } from "services/events/consts";

type Props = {
  children?: React.ReactNode;
};

const AgentLayout: React.FC<Props> = ({ children }) => {
  const { agent, logout } = useAgent();
  const { callType, agentStatus } = useAgentStatus();
  const navigate = useNavigate();
  const [error, setError] = useState(null);

  useEffect(() => {
    if (agentStatus === AgentStatus.InTheCall && callType === CallType.Predictive) {
      navigate(`/${ROUTES.DIALING}`);
    }
  }, [agentStatus, callType]);

  useEffect(() => {
    const subscribe = agent.temporaryError.subscribe(value => {
      setError(value);
    });
    return () => {
      subscribe.unsubscribe();
    };
  }, []);

  useEffect(() => {
    const { addEventListener, removeEventListener } = EventDomDispatcher();
    addEventListener(LOGOUT_EVENT, logout);
    return () => {
      removeEventListener(LOGOUT_EVENT, logout);
    };
  }, []);

  const isShowManual = useMemo(() => {
    return callType === CallType.Manual;
  }, [agentStatus, callType]);

  return (
    <Container>
      <Header />
      <ContentContainer>
        <Navigation />
        {isShowManual && <ManualWidget />}
        {error && <ErrorNotification error={error} />}
        <div>
          <audio hidden controls autoPlay id="audioCtl" />
        </div>
        <Content>{children}</Content>
      </ContentContainer>
    </Container>
  );
};

export default AgentLayout;

const Container = styled.div`
  display: flex;
  flex-direction: column;
`;

const ContentContainer = styled.div`
  display: flex;
  flex-direction: row;
`;

const Content = styled.div`
  display: flex;
  box-sizing: border-box;
  height: calc(100vh - 70px);
  width: 100%;
  padding: 16px;
`;

export const PageWrap = styled.div`
  height: 90vh;
  width: 100%;
  overflow-y: auto;
`;

export const PageWrapContent = styled.div`
  width: 100%;
  height: 100%;
  display: flex;
  flex-direction: column;
`;

export const ContentWrap = styled.div`
  display: flex;
  flex-direction: column;
  overflow-y: auto;
  gap: 0.5rem;
  margin-bottom: 1rem;
`;
