import React from "react";
import styled from "styled-components";
import Navigation from "components/navigation/Navigation";
import Header from "components/header/Header";
import OnCallModal from "components/onCallModal/OnCallModal";
import { CallStatus } from "services/callService/types";
import useCallStatus from "hooks/useCallStatus";

type Props = {
  children?: React.ReactNode;
};

const Layout: React.FC<Props> = ({ children }) => {
  const { callStatus } = useCallStatus();

  return (
    <>
      {callStatus !== CallStatus.OFFLINE && <OnCallModal />}
      <Container>
        <Header />
        <ContentContainer>
          <Navigation />
          <Content>{children}</Content>
        </ContentContainer>
      </Container>
    </>
  );
};

export default Layout;

const Container = styled.div`
  display: flex;
  flex-direction: column;
`;

const ContentContainer = styled.div`
  display: flex;
  flex-direction: row;
`;

const Content = styled.div`
  box-sizing: border-box;
  height: calc(100vh - 70px);
  width: 100%;
  padding: 16px;
  overflow: hidden;
  overflow-y: auto;
`;

export const PageTitle = styled.h1`
  white-space: nowrap;
  text-transform: uppercase;
  margin: 0 0 1.5rem 0;
  padding: 1rem 0;
  border-bottom: 1px solid ${({ theme }) => theme.colors.border.primary};
  color: ${({ theme }) => theme.colors.fg.primary};
  font-weight: 700;
`;
