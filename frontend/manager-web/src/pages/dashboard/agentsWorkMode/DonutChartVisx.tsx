import { useEffect, useState, Fragment } from "react";
import { Pie } from "@visx/shape";
import { Group } from "@visx/group";
import { Text } from "@visx/text";
import styled from "styled-components";
import dashboardApi from "services/api/dashboard";
import TooltipPopover from "components/tooltipPopover/TooltipPopover";
import useConnections from "services/websocket/useConnections";
import { LegendOrdinalComponent } from "./Legend";
import { ChartDataType } from "./types";
import { STATISTIC_WS } from "services/websocket/const";

const dictionary = {
  inTheCall: { symbol: "IN A CALL", amount: 0, color: "#8FD387" },
  waitingForTheCall: { symbol: "WAITING FOR A CALL", amount: 0, color: "#FC9F4A" },
  fillingFeedback: { symbol: "FILLING A FEEDBACK", amount: 0, color: "#D54A7C" },
  inBreak: { symbol: "ON A BREAK", amount: 0, color: "#CBDFED" },
};

const defaultChartData = Object.values(dictionary);

// is used for PieChart in case BE returned all zeroes
const defaultPieData = defaultChartData.map(({ amount, ...rest }) => ({
  ...rest,
  amount: 1,
}));

export default function DonutChartVisx() {
  // when false, substitute data for Pie chart and disable click event on pie chart
  const [isNonZeroValues, setIsNonZeroValues] = useState(false);
  const [chartData, setChartData] = useState<ChartDataType[]>(defaultChartData);
  const [active, setActive] = useState<ChartDataType>(null);
  const [isTooltipShown, setIsTooltipShown] = useState(false);

  const handleUpdate = wsData => {
    const value: ChartDataType[] = Object.entries(wsData).reduce((acc, [key, value]) => {
      if (dictionary[key]) {
        acc.push({ ...dictionary[key], amount: value });
      }
      return acc;
    }, []);
    // set to TRUE if at least one amount is greater zero
    setIsNonZeroValues(value.some(({ amount }) => amount >= 1));
    setChartData(value);
  };

  // replace all zeroes to ones to render a donut anyway when no data
  useConnections(
    STATISTIC_WS,
    [{ chanelName: "agents_work_mode", onMessage: handleUpdate }],
    [chartData]
  );

  useEffect(() => {
    dashboardApi.getAgentsWorkMode().then(response => {
      if (response && response.data) handleUpdate(response.data);
    });
  }, []);

  useEffect(() => {
    if (active && chartData) {
      const activeValue = chartData.find(el => el.symbol === active.symbol);
      setActive(activeValue);
    }
    // eslint-disable-next-line react-hooks/exhaustive-deps
  }, [chartData]);

  const width = 233;
  const half = width / 2;
  const totalAmount = chartData.reduce((acc, item) => acc + item.amount, 0);
  const percentile = active && (active.amount * 100) / totalAmount;
  const pieChartData = chartData.filter(data => data.amount > 0);

  return (
    <>
      <Header>
        <Container>
          <h3>Agent Work Mode</h3>
          <StyledTooltip
            width={150}
            position="left"
            onToggle={({ isShown }) => setIsTooltipShown(isShown)}
            tooltip={<TooltipContent>Here you can find some data about Agents work</TooltipContent>}
          >
            <InfoIcon isTooltipShown={isTooltipShown} className="icon-info icon-show-on-hover" />
          </StyledTooltip>
        </Container>
        <h4>{chartData?.reduce((acc, coin) => acc + coin.amount, 0)}</h4>
      </Header>
      <DonutContainer
      // FIXME: after rerender
      // onMouseOut={() => setActive(null)}
      >
        <svg width={width} height={width}>
          <Group top={half + 5} left={half + 5}>
            <Pie
              data={isNonZeroValues ? pieChartData : defaultPieData}
              pieValue={data => data.amount}
              outerRadius={half - 10}
              innerRadius={() => {
                const size = 36;
                return half - size;
              }}
              padAngle={0.04}
            >
              {pie => {
                return pie.arcs
                  .filter(arc => arc.data.amount > 0)
                  .map(arc => {
                    return (
                      <Fragment key={arc.data.symbol}>
                        <g
                          onClick={() => isNonZeroValues && setActive(arc.data)}
                          {...(active?.symbol === arc.data.symbol && {
                            // filter: "drop-shadow(0px 2px 2px gray)",
                            filter: "url(#innerShadowFilter)",
                          })}
                        >
                          <path d={pie.path(arc)} fill={arc.data.color}></path>
                        </g>
                        <defs>
                          <filter id="innerShadowFilter" width="200%" height="200%">
                            <feGaussianBlur
                              in="SourceAlpha"
                              stdDeviation="3"
                              result="blur"
                            ></feGaussianBlur>

                            <feComposite
                              in2="SourceAlpha"
                              operator="arithmetic"
                              k2="-1"
                              k3="1"
                              result="shadowDiff"
                            ></feComposite>

                            <feFlood floodColor="#444444" floodOpacity="0.55"></feFlood>
                            <feComposite in2="shadowDiff" operator="in"></feComposite>
                            <feComposite
                              in2="SourceGraphic"
                              operator="over"
                              result="firstfilter"
                            ></feComposite>

                            <feGaussianBlur
                              in="firstfilter"
                              stdDeviation="3"
                              result="blur2"
                            ></feGaussianBlur>
                            <feOffset dy="-2" dx="-3"></feOffset>
                            <feComposite
                              in2="firstfilter"
                              operator="arithmetic"
                              k2="-1"
                              k3="1"
                              result="shadowDiff"
                            ></feComposite>

                            <feFlood floodColor="#444444" floodOpacity="0.55"></feFlood>
                            <feComposite in2="shadowDiff" operator="in"></feComposite>
                            <feComposite in2="firstfilter" operator="over"></feComposite>
                          </filter>
                        </defs>
                      </Fragment>
                    );
                  });
              }}
            </Pie>

            {active ? (
              <>
                <StyledText
                  textAnchor="middle"
                  fill={"gray"}
                  fontSize={14}
                  lineHeight={17}
                  dy={-15}
                >
                  {active.symbol}
                </StyledText>
                <StyledText
                  textAnchor="middle"
                  fill={active.color}
                  fontSize={40}
                  lineHeight={48}
                  dy={25}
                >
                  {active.amount}
                </StyledText>
                <StyledText
                  textAnchor="middle"
                  fill={"#C80048"}
                  fontSize={12}
                  lineHeight={15}
                  dy={45}
                >
                  {isFinite(percentile) ? Math.floor(percentile) + "%" : ""}
                </StyledText>
              </>
            ) : (
              <>
                <StyledText textAnchor="middle" fill="#000" fontSize={40} dy={15}>
                  {totalAmount}
                </StyledText>
              </>
            )}
          </Group>
        </svg>
        <LegendOrdinalComponent setActive={setActive} data={chartData} />
      </DonutContainer>
    </>
  );
}

const DonutContainer = styled.main`
  display: flex;
  align-items: center;
  justify-content: center;
  padding-right: 14px;
`;

const Container = styled.div`
  display: flex;
  justify-content: space-between;
  align-items: center;
`;

const TooltipContent = styled.div`
  border-radius: 4px;
  padding: 0.5rem;
  ${({ theme }) => theme.typography.body1};
  background-color: ${({ theme }) => theme.colors.icons.primary};
  color: ${({ theme }) => theme.colors.icons.tertiary};
`;
const StyledTooltip = styled(TooltipPopover)`
  margin-left: 5px;

  &:after {
    background: ${({ theme }) => theme.colors.icons.primary};
  }
`;

const InfoIcon = styled.i<{ isTooltipShown: boolean }>`
  position: relative;
  right: -5px;
  font-size: 24px;
  color: ${({ theme }) => theme.colors.leadGroups.darkBlue};
  cursor: pointer;
  ${({ isTooltipShown }) => isTooltipShown && "opacity: 1 !important"};
`;

const StyledText = styled(Text)<{ fontSize: number }>`
  ${({ theme }) => theme.typography.body1};
  font-size: ${({ fontSize }) => fontSize + "px"};
`;
const Header = styled.header`
  display: flex;
  justify-content: space-between;
  margin: 1rem 1.5rem;
`;
