import React from "react";
import styled from "styled-components";
import Header from "components/header/Header";
import Navigation from "agent/components/navigation/Navigation";

type Props = {
  children?: React.ReactNode;
};

const AgentLayout: React.FC<Props> = ({ children }) => {
  return (
    <Container>
      <Header />
      <ContentContainer>
        <Navigation />
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
