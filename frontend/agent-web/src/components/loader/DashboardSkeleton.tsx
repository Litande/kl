import React from "react";
import styled from "styled-components";
import Navigation from "components/navigation/Navigation";
import HeaderSkeleton from "components/loader/items/HeaderSkeleton";

type Props = {
  children?: React.ReactNode;
};

const DashboardSkeleton: React.FC<Props> = ({ children }) => {
  return (
    <Container>
      <HeaderSkeleton />
      <ContentContainer>
        <Navigation />
        <Content>{children}</Content>
      </ContentContainer>
    </Container>
  );
};

export default DashboardSkeleton;

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
