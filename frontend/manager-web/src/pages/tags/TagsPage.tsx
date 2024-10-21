import React, { useEffect } from "react";
import styled from "styled-components";

import useBehavior from "hooks/useBehavior";
import { PageTitle } from "components/layout/Layout";

import TagsFilter from "./components/TagsFilter";
import behavior from "./behavior";
import TagsGrid from "./components/tagsGrid/TagsGrid";

const TagsPage = () => {
  const { getAllTags, getTeams } = useBehavior(behavior);

  useEffect(() => {
    getAllTags();
    getTeams();
    // eslint-disable-next-line react-hooks/exhaustive-deps
  }, []);

  return (
    <Container>
      <TagsPageTitle>Tags</TagsPageTitle>
      <TagsFilter />
      <TagsGrid />
    </Container>
  );
};

export default TagsPage;

const TagsPageTitle = styled(PageTitle)`
  margin-bottom: 1rem;
  margin-right: 0;
`;

const Container = styled.div`
  height: 100%;
`;
