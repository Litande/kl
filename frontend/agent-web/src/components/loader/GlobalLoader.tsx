import { useEffect, useState } from "react";
import { useNavigate } from "react-router-dom";
import styled from "styled-components";
import LoaderSpinner from "components/loader/LoaderSpinner";
import { typography } from "globalStyles/theme/fonts";
import { ITimeout, timerService } from "services/timerService/TimerService";
import Button from "components/button/Button";

type ComponentProps = {
  isShowRefresh?: boolean;
};

const DELAY_TO_SHOW_REFRESH = 5000;
const GlobalLoader = ({ isShowRefresh }: ComponentProps) => {
  const [isShowButton, setIsShowButton] = useState(false);
  const { addTimeout, removeTimeout } = timerService();
  const navigate = useNavigate();

  useEffect(() => {
    let timeout: ITimeout = null;
    if (isShowRefresh) {
      timeout = addTimeout(DELAY_TO_SHOW_REFRESH, () => setIsShowButton(true));
    }
    return () => {
      timeout && removeTimeout(timeout);
    };
  }, []);

  const refreshPage = () => {
    navigate(0);
  };

  return (
    <Inner>
      <Container>
        <LoaderSpinner />
        <LoadingTitle>Loading</LoadingTitle>
        {isShowButton && (
          <ErrorTitle>Unable to perform request please reload page and try again</ErrorTitle>
        )}
        {isShowButton && <StyledButton onClick={refreshPage}>Refresh</StyledButton>}
      </Container>
    </Inner>
  );
};

export default GlobalLoader;

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
  display: flex;
  flex-direction: column;
  width: 100%;
  height: 300px;
  align-items: center;
  gap: 1rem;
`;

const StyledButton = styled(Button)`
  min-width: 120px;
`;

const LoadingTitle = styled.div`
  ${typography.body1};
  width: 100%;
  text-align: center;
  margin-top: 2rem;
  text-transform: uppercase;
  color: ${({ theme }) => theme.colors.bg.primary};
`;

const ErrorTitle = styled.div`
  color: ${({ theme }) => theme.colors.error.baseError};
  ${({ theme }) => theme.typography.body1};
`;
