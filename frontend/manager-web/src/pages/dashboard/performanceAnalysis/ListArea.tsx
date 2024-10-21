import { ReactElement, useEffect, useState } from "react";
import styled from "styled-components";

import { Period } from "pages/dashboard/performanceAnalysis/types";
import dashboardApi from "services/api/dashboard";
import useConnections from "services/websocket/useConnections";
import { STATISTIC_WS } from "services/websocket/const";
import { typography } from "globalStyles/theme/fonts";

type ListAreaProps = {
  type: string;
  title: string;
  value: number;
  trend: number;
  handleChartClick: (data: null | string) => void;
  // eslint-disable-next-line @typescript-eslint/no-explicit-any
  chart: (props: any) => ReactElement;
  period: Period;
  color: string;
  accentColor: string;
};

export default function ListArea({
  title,
  value,
  trend,
  type,
  period,
  handleChartClick,
  chart: Chart,
  ...rest
}: ListAreaProps) {
  const [areaData, setAreaData] = useState({
    value,
    trend,
    title,
    values: [],
    type,
  });

  const handleUpdate = wsData => {
    const data = wsData.find(el => type === el.type);
    setAreaData(prev => ({ ...prev, ...data }));
  };

  useConnections(STATISTIC_WS, [{ chanelName: "performance_statistic", onMessage: handleUpdate }]);

  useEffect(() => {
    Promise.all([
      dashboardApi.getPerfomanceStatistics({ type: type, from: period.from, to: period.to }),
      dashboardApi.getPerformancePlot({
        type,
        from: period.from,
        to: period.to,
        step: period.step,
      }),
    ]).then(([statistic, plot]) => {
      statistic && plot && setAreaData({ ...statistic.data, ...plot.data });
    });
  }, [period, type]);

  return (
    <Container onClick={() => handleChartClick(type)}>
      <ChartInfo>
        <TitleWrap>{title}</TitleWrap>
        <ChartValues>
          <H2Wrap>{areaData.value}</H2Wrap>
          <PreviousValueChartDiff isPositive={areaData.trend > 0}>
            {areaData.trend > 0 ? " +" + areaData.trend : areaData.trend}
          </PreviousValueChartDiff>
        </ChartValues>
      </ChartInfo>
      <ChartContainer>
        <Chart chartData={areaData.values} type={areaData.type} {...rest} />
      </ChartContainer>
    </Container>
  );
}

const Container = styled.div`
  position: relative;
  background: #ffffff;
  box-shadow: 0 2px 4px rgba(0, 0, 0, 0.16);
  border-radius: 8px;
  padding: 0.8rem;
  gap: 10px;
  display: flex;
  height: min-content;
  cursor: pointer;
`;

const ChartContainer = styled.div`
  position: absolute;
  display: flex;
  right: 10px;
  bottom: 10px;
  align-items: end;
`;

const ChartInfo = styled.div`
  z-index: 1;
  display: flex;
  flex-direction: column;
  & > h4 {
    ${({ theme }) => theme.typography.subtitle3}
  }
`;

const ChartValues = styled.div`
  display: flex;
  gap: 5px;
  & > h2 {
    font-size: 24px;
    line-height: 29px;
  }
`;

const PreviousValueChartDiff = styled.h5<{ isPositive?: boolean }>`
  color: ${({ isPositive, theme }) =>
    isPositive ? theme.colors.values.lightGreen : theme.colors.values.lightRed};
`;

const TitleWrap = styled.span`
  ${typography.subtitle3}
  text-transform: uppercase;
  color: ${({ theme }) => theme.colors.fg.secondary};
`;

const H2Wrap = styled.h2`
  min-width: 1.5rem;
`;
