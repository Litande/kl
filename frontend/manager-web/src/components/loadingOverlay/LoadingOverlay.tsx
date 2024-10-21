import styled from "styled-components";
import Loader from "components/loader/Loader";

const LoadingOverlay = () => {
  return (
    <Container>
      <StyledLoader />
    </Container>
  );
};

export default LoadingOverlay;

const Container = styled.div`
  position: absolute;
  top: 0;
  right: 0;
  bottom: 0;
  left: 0;
  display: flex;
  justify-content: center;
  align-items: center;
`;

const StyledLoader = styled(Loader)`
  width: 6rem;
  height: 6rem;
`;
