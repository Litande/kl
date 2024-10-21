import { useCallback, useEffect, useState } from "react";
import styled from "styled-components";
import debounce from "lodash.debounce";

import TooltipPopover from "components/tooltipPopover/TooltipPopover";
import AreaChart from "pages/dashboard/performanceAnalysis/AreaChart";
import ListArea from "pages/dashboard/performanceAnalysis/ListArea";
import { chartsOption, periodOptions } from "pages/dashboard/performanceAnalysis/Contants";
import { PlotChartResponse } from "pages/dashboard/performanceAnalysis/types";
import dashboardApi from "services/api/dashboard";
import { useConnectionService } from "services/websocket/useConnectionService";
import useConnections from "services/websocket/useConnections";
import useCarousel from "hooks/useCarousel";
import { useMounted } from "hooks/useMounted";
import { STATISTIC_WS } from "services/websocket/const";
import { typography } from "globalStyles/theme/fonts";
import { PeriodItem } from "pages/dashboard/styles";
import {
  PERFOMANCE_CHART_WIDTH,
  PERFOMANCE_LIST_WIDTH,
  PERFOMANCE_WIDTH,
} from "pages/dashboard/consts";

const periods = [periodOptions.day, periodOptions.week, periodOptions.month];
const CAROUSEL_SHOW_NEXT_CHART_DELAY = 30 * 1000; // 30 sec
const NON_ACTION_DELAY = 300 * 1000; // 5 min

export default function ChartBlock() {
  const { invoke } = useConnectionService(STATISTIC_WS);

  const [chartData, setChartData] = useState<PlotChartResponse["values"]>([]);

  const [isTooltipShown, setIsTooltipShown] = useState(false);
  const [chartType, setChartType] = useState(chartsOption[0].type);
  const [activePeriod, setActivePeriod] = useState(periodOptions.week);
  const { carouselIndex, setCarouselIndex, stopCarousel, toggleCarousel, startCarousel } =
    useCarousel();
  const isMounted = useMounted();
  const [isUserActive, setIsUserActive] = useState(false);
  const AreaChartProps = chartsOption.find(option => option.type === chartType);

  const handleChartClick = chartType => {
    setChartType(chartType);
    stopCarousel();
  };

  const userActivateCarousel = () => {
    setIsUserActive(false);
    debounceNonActiveStart();
  };

  const userPauseCarousel = () => {
    setIsUserActive(true);
    stopCarousel();
  };

  const debounceNonActiveStart = useCallback(
    debounce(() => {
      startCarousel();
    }, NON_ACTION_DELAY),
    []
  );

  const showNextRowInCarousel = () => {
    const chart = chartsOption[carouselIndex];
    if (!chart) {
      return setCarouselIndex(0);
    }
    setChartType(chart.type);
  };

  useEffect(() => {
    !isUserActive && debounceNonActiveStart();
    // eslint-disable-next-line react-hooks/exhaustive-deps
  }, [isUserActive]);

  useEffect(() => {
    if (!isUserActive) {
      if (carouselIndex > -1) {
        showNextRowInCarousel();
        setTimeout(() => {
          if (!isMounted) {
            return undefined;
          }
          setCarouselIndex(index => {
            if (index === -1) {
              return -1;
            } else {
              if (index + 1 >= chartsOption.length) {
                return 0;
              } else {
                return index + 1;
              }
            }
          });
        }, CAROUSEL_SHOW_NEXT_CHART_DELAY);
      }
    }
    // eslint-disable-next-line react-hooks/exhaustive-deps
  }, [carouselIndex, isUserActive]);

  useEffect(() => {
    dashboardApi
      .getPerformancePlot({
        type: chartType,
        from: activePeriod.from,
        to: activePeriod.to,
        step: activePeriod.step,
      })
      .then((response: { data: PlotChartResponse }) => {
        response && response.data && setChartData(response.data.values);
      })
      .finally(async () => {
        await invoke({
          methodName: "SubscribeAgentPerformancePlot",
          data: {
            from: activePeriod.from,
            step: activePeriod.step,
            type: chartType,
          },
          isRepeatAfterConnection: true,
        });

        await invoke({
          methodName: "SubscribeAgentPerformanceStatistic",
          data: {
            from: activePeriod.from,
            types: chartsOption.map(el => el.type),
          },
          isRepeatAfterConnection: true,
        });
      });
    // eslint-disable-next-line react-hooks/exhaustive-deps
  }, [chartType, activePeriod]);

  useConnections(
    STATISTIC_WS,
    [
      {
        chanelName: "performance_plot",
        onMessage: data => {
          if (data.value !== chartData[chartData.length - 1]?.value) {
            setChartData(prev => [...prev.slice(0, prev.length - 1), data]);
          }
        },
      },
    ],
    [chartData]
  );

  const selectPeriod = period => {
    setActivePeriod(period);
  };

  return (
    <ContentWrapper onMouseLeave={userActivateCarousel} onMouseEnter={userPauseCarousel}>
      <LeftContainer>
        <Header>
          <h3>Agents Performance Analysis</h3>
          <StyledTooltip
            width={150}
            position="left"
            onToggle={({ isShown }) => setIsTooltipShown(isShown)}
            tooltip={
              <TooltipContent>
                Here you can find some data about Agents Performance Analysis
              </TooltipContent>
            }
          >
            <Title>
              <InfoIcon
                isTooltipShown={isTooltipShown}
                onClick={toggleCarousel}
                className="icon-info icon-show-on-hover"
              />
              <span style={{ fontSize: "6px", color: "lightgray" }}>
                {carouselIndex === -1 ? " OFF" : " ON"}
              </span>
            </Title>
          </StyledTooltip>
        </Header>
        <ChartType>
          <AvgCountCalls>{AreaChartProps?.title || "Av. number of calls"}</AvgCountCalls>
        </ChartType>

        <Main>
          <AreaChart chartData={chartData} colorTo={AreaChartProps?.color} />
        </Main>
      </LeftContainer>

      <ChartsList>
        <PeriodsContainer>
          {periods.map(period => (
            <PeriodItem
              key={period.label}
              onClick={() => selectPeriod(period)}
              isActive={period.label === activePeriod.label}
            >
              {period.label}
            </PeriodItem>
          ))}
        </PeriodsContainer>
        {chartsOption.map(el => (
          <ListArea
            period={activePeriod}
            key={el.title}
            handleChartClick={handleChartClick}
            {...el}
            color={el.color}
            accentColor={el.accentColor}
          />
        ))}
      </ChartsList>
    </ContentWrapper>
  );
}

const ContentWrapper = styled.div`
  display: flex;
  margin: 1rem;
  width: ${PERFOMANCE_WIDTH}px;
  gap: 1rem;
`;
const LeftContainer = styled.div`
  display: flex;
  flex-direction: column;
  flex: 2 1 auto;
  justify-content: space-between;
`;

const ChartsList = styled.div`
  width: ${PERFOMANCE_LIST_WIDTH}px;
  flex: 1 1 auto;
  display: flex;
  flex-direction: column;
  gap: 1rem;
`;

const Header = styled.header`
  display: flex;

  & > h5 {
    color: rgba(0, 0, 0, 0.6);
    margin-top: 12px;
    font-size: 1rem;
    line-height: 19px;
    text-transform: uppercase;
  }
`;

const ChartType = styled.div`
  display: flex;
  align-items: flex-start;
  margin-top: 1rem;
  margin-left: 1.5rem;
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

const Main = styled.div`
  display: flex;
`;
const PeriodsContainer = styled.div`
  display: flex;
  gap: 1rem;
  justify-content: end;
`;

const Title = styled.h3`
  display: flex;
  align-items: center;
  gap: 1rem;
`;

const AvgCountCalls = styled.div`
  ${typography.subtitle3}
  text-transform: uppercase;
  color: ${({ theme }) => theme.colors.fg.secondary_light};
`;
