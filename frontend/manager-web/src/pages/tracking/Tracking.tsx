import React, { useState } from "react";
import styled from "styled-components";
import AgentsGrid from "./trackingGrid/Grid";
import LeadsGrid from "./leadsGrid/Grid";
import Groups from "./groups/Groups";
import { PageTitle } from "components/layout/Layout";
import LayoutButton from "components/layoutButton/LayoutButton";
import { getDefaultLayout } from "components/layoutButton/utils";
import { Layout } from "components/layoutButton/types";

const Tracking = () => {
  const [layout, setLayout] = useState(() => getDefaultLayout());
  const handleLayoutChange = index => setLayout(index);

  return (
    <Content>
      <StyledPageTitle>
        Tracking
        <LayoutButton onChange={handleLayoutChange} />
      </StyledPageTitle>
      <ContentTop layout={layout}>
        <Groups layout={layout} />
        <LeadsGrid layout={layout} />
      </ContentTop>
      <AgentsGrid layout={layout} />
    </Content>
  );
};

const Content = styled.div`
  display: flex;
  flex-direction: column;
  height: 100%;
`;

const ContentTop = styled.div<{ layout }>`
  display: flex;
  gap: 25px;
  height: ${({ layout }) => (layout === Layout.One ? "61px" : "initial")};
`;

const StyledPageTitle = styled(PageTitle)`
  margin: 0 0 0.5rem 0;
  display: flex;
  justify-content: space-between;
`;

export default Tracking;
