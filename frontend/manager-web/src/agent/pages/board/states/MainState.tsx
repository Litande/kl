import styled from "styled-components";
import Button from "components/button/Button";
import { clockInAction } from "agent/actions/clockInAction";

const MainState = () => {
  const onClick = () => {
    clockInAction();
  };

  return (
    <Wrap>
      <GreetingWrap>
        <h1>Welcome Brad Pitt!</h1>
        <ClockInButton onClick={onClick}>Clock In</ClockInButton>
      </GreetingWrap>
    </Wrap>
  );
};

export default MainState;

const Wrap = styled.div`
  display: flex;
  align-self: center;
  justify-content: center;
  width: 100%;
`;

const GreetingWrap = styled.div`
  width: 350px;
  display: flex;
  flex-direction: column;
  align-items: center;
`;

const ClockInButton = styled(Button)`
  width: 174px;
`;
