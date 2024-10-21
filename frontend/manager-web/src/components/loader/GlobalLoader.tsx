import styled from "styled-components";
import LoaderSpinner from "components/loader/Loader";
import { typography } from "globalStyles/theme/fonts";

const GlobalLoader = () => {
  return (
    <Inner>
      <Container>
        <StyledLoaderSpinner />
        <LoadingTitle>Loading</LoadingTitle>
      </Container>
    </Inner>
  );
};

export default GlobalLoader;

const StyledLoaderSpinner = styled(LoaderSpinner)`
  width: 128px;
  height: 128px;
`;

const Inner = styled.div`
  top: 0;
  left: 0;
  position: absolute;
  display: flex;
  box-sizing: border-box;
  flex-direction: column;
  justify-content: center;
  align-items: center;
  width: 100%;
  height: 100%;
  rgba(239, 247, 250, 0.4)
  animation: tp-fade-in 0.5s forwards ease;
  z-index: 4;
`;

const Container = styled.div`
  margin-left: 230px; //menu width
`;

const LoadingTitle = styled.div`
  ${typography.body1};
  width: 100%;
  text-align: center;
  margin-top: 2rem;
  text-transform: uppercase;
  color: ${({ theme }) => theme.colors.bg.primary};
`;
