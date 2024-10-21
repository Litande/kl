import styled from "styled-components";
import Button from "components/button/Button";
import boardBg from "images/board_bg.png";
import { useAgent } from "data/user/useAgent";
import { PageWrapContent } from "components/layout/AgentLayout";
import useAgentStatus from "hooks/useAgentStatus";
import { CallType } from "data/user/types";
import useBreaks from "hooks/useBreaks";
import moment from "moment";

const ClockInState = () => {
  const { agent } = useAgent();
  const { callType } = useAgentStatus();
  const { activeBreakTimer, breaks } = useBreaks();

  const onClick = () => {
    agent.finishBreak();
  };

  const activeBreak = breaks.find(item => item.isActive);

  if (!activeBreak)
    return (
      <PageWrapContent>
        <Wrap imageUrl={boardBg}>
          <GreetingWrap>
            <BreakIsOut>
              <h1>Break was finished, please Clock In</h1>
            </BreakIsOut>
            <ClockInButton disabled={callType === CallType.Manual} onClick={onClick}>
              Clock Back In
            </ClockInButton>
          </GreetingWrap>
        </Wrap>
      </PageWrapContent>
    );

  const startDate = moment(activeBreak.startedAt);
  const timeLeft = Math.ceil(activeBreakTimer / 60);

  return (
    <PageWrapContent>
      <Wrap imageUrl={boardBg}>
        <GreetingWrap>
          <Title1>Your break has</Title1>
          <Title2>Started</Title2>
          <Title3>at</Title3>
          <Time>{startDate.format("HH:mm")}</Time>
          <TimeLeft isRed={timeLeft === 0}>You have {timeLeft} minutes left</TimeLeft>
          <ClockInButton disabled={callType === CallType.Manual} onClick={onClick}>
            Clock Back In
          </ClockInButton>
        </GreetingWrap>
      </Wrap>
    </PageWrapContent>
  );
};

export default ClockInState;

const Wrap = styled.div<{ imageUrl: string }>`
  display: flex;
  align-items: center;
  justify-content: center;
  width: 100%;
  height: 100%;
  // background: url(${({ imageUrl }) => imageUrl});
`;

const GreetingWrap = styled.div`
  min-height: 273px;
  box-sizing: border-box;
  padding: 90px 90px 60px;
  display: flex;
  flex-direction: column;
  align-items: center;
  background: ${({ theme }) => theme.colors.bg.ternary};
  text-transform: uppercase;
  box-shadow: 0px 4px 4px ${({ theme }) => theme.colors.border.primary};
  border-radius: 4px;
`;

const ClockInButton = styled(Button)`
  min-width: 174px;
  margin-top: 1rem;
`;

const Title1 = styled.h1`
  line-height: 40px;
  font-weight: bold;
`;
const Title2 = styled.h1`
  font-size: 26px;
  font-weight: bold;
`;
const Title3 = styled.div`
  margin: 5px 0 2px;
  ${({ theme }) => theme.typography.body3};
`;
const Time = styled.h1`
  font-size: 20px;
  font-weight: bold;
`;
const TimeLeft = styled.div<{ isRed: boolean }>`
  ${({ theme }) => theme.typography.subtitle4}
  color: ${({ theme, isRed }) => (isRed ? theme.colors.error.baseError : theme.colors.fg.primary)};
`;

const BreakIsOut = styled.div`
  ${({ theme }) => theme.typography.subtitle4}
  color: ${({ theme }) => theme.colors.error.baseError};
`;
