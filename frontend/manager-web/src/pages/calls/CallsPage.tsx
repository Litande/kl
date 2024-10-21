import styled from "styled-components";
import { PageTitle } from "components/layout/Layout";
import SearchForm from "./components/SearchForm";
import Grid from "./components/grid/Grid";

const CallsPage = () => {
  return (
    <Container>
      <PageName>Call Recordings</PageName>
      <SearchForm />
      <Grid />
    </Container>
  );
};

export default CallsPage;

const Container = styled.div`
  height: 100%;
  display: flex;
  flex-direction: column;
`;

const PageName = styled(PageTitle)`
  box-sizing: border-box;
  height: 72px;
  padding: 16px 0;
  margin: 0;
  border-bottom: 1px solid ${({ theme }) => theme.colors.border.primary};
`;
