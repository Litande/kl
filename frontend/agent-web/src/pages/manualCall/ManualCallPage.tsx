import React from "react";
import styled from "styled-components";

import PageTitle from "components/layout/PageTitle";
import { PageWrapContent, ContentWrap } from "components/layout/AgentLayout";

import ManualPhoneComponent from "./components/ManualPhoneComponent";

const ManualCallPage = () => {
  return (
    <PageWrapContent>
      <PageTitle label={"Manual Call"} />
      <ContentWrap>
        <Container>
          <ManualPhoneComponent />
        </Container>
      </ContentWrap>
    </PageWrapContent>
  );
};

export default ManualCallPage;

const Container = styled.div`
  display: flex;
  justify-content: center;
`;
