import React, { useMemo, useCallback, useState, useEffect, useRef, useLayoutEffect } from "react";
import { AreaClosed, Line, Bar, LinePath } from "@visx/shape";
import { curveMonotoneX } from "@visx/curve";
import { GridRows, GridColumns } from "@visx/grid";
import { scaleTime, scaleLinear } from "@visx/scale";
import { withTooltip, TooltipWithBounds, defaultStyles } from "@visx/tooltip";
import { WithTooltipProvidedProps } from "@visx/tooltip/lib/enhancers/withTooltip";
import { localPoint } from "@visx/event";
import { LinearGradient } from "@visx/gradient";
import { max, extent, bisector } from "d3-array";
import { AxisLeft, AxisBottom } from "@visx/axis";
import { timeFormat } from "d3-time-format";
import styled from "styled-components";
import moment from "moment";
import { useSelector } from "react-redux";

import { PlotChartResponse, PlotChartValue } from "pages/dashboard/performanceAnalysis/types";
import { fullScreenStateSelector } from "components/fullScreen/fullScreenSlice";
import { navigationStateSelector } from "components/navigation/navigationSlice";
import { PERFOMANCE_CHART_WIDTH } from "pages/dashboard/consts";

type TooltipData = PlotChartValue;

type AreaProps = {
  margin?: { top: number; right: number; bottom: number; left: number };
  colorFrom?: string;
  colorTo?: string;
  chartData: PlotChartResponse["values"];
};

const accentColorDark = "#9DC7FC";
const tooltipStyles = (color = accentColorDark) => ({
  ...defaultStyles,
  background: "#FFFFFF",
  border: `1px solid ${color}`,
  boxShadow: "0px 4px 4px rgba(0, 0, 0, 0.25)",
  borderRadius: "8px",
  width: "89px",
  height: "35px",
  color: "#000",
  fontFamily: "Inter regular",
  fontSize: 10,
});

const axisBottomTickLabelProps = {
  textAnchor: "middle" as const,
  fontFamily: "Inter regular",
  fontSize: 10,
  fill: "grey",
};
const axisLeftTickLabelProps = {
  dx: "-0.25em",
  dy: "0.25em",
  fontFamily: "Inter regular",
  fontSize: 10,
  textAnchor: "end" as const,
  fill: "grey",
};

const formatDate = timeFormat("%d %b %y");
const getDate = (d: PlotChartValue) => moment(d.date).utc();
const getStockValue = (d: PlotChartValue) => d?.value || 0;
const bisectDate = bisector<PlotChartValue, Date>(d => moment(d.date).utc()).left;
const SHIFT_ZOOM_X_DEFAULT = 8;
const SHIFT_DRAG_X_DEFAULT = 2;
export default withTooltip<AreaProps, TooltipData>(
  ({
    margin = { top: 25, right: 10, bottom: 0, left: 50 },
    showTooltip,
    hideTooltip,
    tooltipData,
    tooltipTop = 0,
    tooltipLeft = 0,
    colorFrom = "#fff",
    colorTo = accentColorDark,
    chartData = [{ date: moment().utc().format(), value: 0 }],
  }: AreaProps & WithTooltipProvidedProps<TooltipData>) => {
    const wrapperRef = useRef<HTMLDivElement>(null);
    const [width, setWidth] = useState(PERFOMANCE_CHART_WIDTH);
    const height = 215;
    // bounds
    const innerWidth = width - margin.left - margin.right;
    const innerHeight = height - margin.top - margin.bottom;

    const [left, setLeft] = useState(0);
    const [right, setRight] = useState(1);
    const chartRef = useRef(null);
    const [isDragging, setIsDragging] = useState(false);
    const [startX, setStartX] = useState(-1);
    const { isFullScreen } = useSelector(fullScreenStateSelector);
    const { isCollapsed } = useSelector(navigationStateSelector);

    useLayoutEffect(() => {
      if (wrapperRef.current) {
        setWidth(wrapperRef.current.parentElement.parentElement.clientWidth);
      }
    }, [wrapperRef.current, isFullScreen, isCollapsed]);

    // TODO huck to stop browser page zoom
    useEffect(() => {
      const stopWheel = e => e.preventDefault();
      chartRef.current?.addEventListener("wheel", stopWheel, { passive: false });
      return () => {
        // eslint-disable-next-line react-hooks/exhaustive-deps
        chartRef.current?.removeEventListener("wheel", stopWheel, { passive: false });
      };
    }, []);

    useEffect(() => {
      setLeft(0);
      setRight(1);
      setStartX(-1);
    }, [chartData.length]);

    const scaledData = useMemo(() => {
      return chartData.slice(left, chartData.length + right);
    }, [chartData, left, right]);

    // scales
    const dateScale = useMemo(() => {
      return scaleTime({
        range: [margin.left, innerWidth + margin.left],
        domain: extent(scaledData, getDate) as [Date, Date],
      });
    }, [scaledData, innerWidth, margin.left]);

    const stockValueScale = useMemo(
      () =>
        scaleLinear({
          range: [innerHeight + margin.top, margin.top],
          domain: [0, (max(scaledData, getStockValue) || 0) + innerHeight / 3],
          nice: true,
        }),
      [scaledData, innerHeight, margin.top]
    );

    // tooltip handler
    const handleTooltip = useCallback(
      (event: React.TouchEvent<SVGRectElement> | React.MouseEvent<SVGRectElement>) => {
        const { x } = localPoint(event) || { x: 0 };
        const x0 = dateScale.invert(x);
        const index = bisectDate(chartData, x0, 1);
        const d0 = chartData[index - 1];
        const d1 = chartData[index];
        let d = d0;
        if (d1 && getDate(d1)) {
          d = x0.valueOf() - getDate(d0).valueOf() > getDate(d1).valueOf() - x0.valueOf() ? d1 : d0;
        }
        showTooltip({
          tooltipData: d,
          tooltipLeft: x,
          tooltipTop: stockValueScale(getStockValue(d)),
        });
      },
      [showTooltip, stockValueScale, dateScale, chartData]
    );

    const onWheel = useCallback(
      e => {
        const { layerX, wheelDelta } = e.nativeEvent;
        const diff = Math.round(((layerX - 50) / innerWidth) * 100);
        const diffLeft = diff / 100;
        const diffRight = (100 - diff) / 100;
        const tempLeft =
          wheelDelta < 0
            ? left - SHIFT_ZOOM_X_DEFAULT * diffLeft
            : left + SHIFT_ZOOM_X_DEFAULT * diffLeft;
        const tempRight =
          wheelDelta < 0
            ? right + SHIFT_ZOOM_X_DEFAULT * diffRight
            : right - SHIFT_ZOOM_X_DEFAULT * diffRight;

        const sum = Math.abs(tempLeft) + Math.abs(tempRight);

        // width point
        if (sum > chartData.length * 0.85) return;
        if (tempLeft >= 0) {
          setLeft(tempLeft);
        }
        if (tempRight > -chartData.length / 2 && tempRight < 0) {
          setRight(tempRight);
        }
      },
      [left, right, chartData, innerWidth]
    );

    const dragStart = e => {
      setIsDragging(true);
      setStartX(e.nativeEvent.layerX);
    };
    const dragMove = e => {
      if (!isDragging) return;

      const delta = startX - e.nativeEvent.layerX;
      if (Math.abs(delta) < 10) return;
      setStartX(e.nativeEvent.layerX);
      const tempLeft = delta < 0 ? left - SHIFT_DRAG_X_DEFAULT : left + SHIFT_DRAG_X_DEFAULT;
      const tempRight = delta < 0 ? right - SHIFT_DRAG_X_DEFAULT : right + SHIFT_DRAG_X_DEFAULT;
      if (tempLeft < 0 || tempRight > 0) {
        return;
      }
      setLeft(tempLeft);
      setRight(tempRight);
    };

    const dragEnd = () => {
      setIsDragging(false);
    };

    return (
      <Wrap ref={wrapperRef}>
        <SvgWrap
          width={width}
          height={height + 20}
          ref={chartRef}
          onWheelCapture={onWheel}
          onTouchStart={dragStart}
          onTouchMove={dragMove}
          onTouchEnd={dragEnd}
          onMouseDown={dragStart}
          onMouseMove={isDragging ? dragMove : null}
          onMouseUp={dragEnd}
          onMouseLeave={() => {
            if (isDragging) dragEnd();
          }}
        >
          <rect
            x={0}
            y={0}
            width={width}
            height={height}
            fill="url(#area-background-gradient)"
            rx={14}
          />
          <LinearGradient id="area-background-gradient" from={"#fff"} to={"#fff"} />
          <LinearGradient
            id="area-gradient"
            from={colorTo}
            to={colorFrom}
            toOpacity={0.1}
            fromOpacity={0.6}
          />

          <GridRows
            left={margin.left}
            scale={stockValueScale}
            width={innerWidth}
            strokeDasharray="3"
            stroke={"grey"}
            strokeOpacity={0.3}
            pointerEvents="none"
          />
          <GridColumns
            top={margin.top}
            scale={dateScale}
            height={innerHeight}
            numTicks={4}
            stroke={"grey"}
            strokeOpacity={0.15}
            pointerEvents="none"
          />
          <LinePath
            data={scaledData}
            curve={curveMonotoneX}
            x={d => dateScale(getDate(d)) ?? 0}
            y={d => stockValueScale(getStockValue(d)) ?? 0}
            stroke={colorTo}
            strokeWidth={2}
          />
          <AreaClosed<PlotChartValue>
            data={scaledData}
            x={d => dateScale(getDate(d)) ?? 0}
            y={d => stockValueScale(getStockValue(d)) ?? 0}
            yScale={stockValueScale}
            strokeWidth={2}
            stroke="url(#area-gradient)"
            opacity={1}
            fill="url(#area-gradient)"
            curve={curveMonotoneX}
          />
          <AxisLeft
            scale={stockValueScale}
            numTicks={6}
            left={50}
            stroke={"white"}
            tickStroke={"white"}
            tickLabelProps={axisLeftTickLabelProps}
          />
          <AxisBottom
            top={innerHeight + 20}
            scale={dateScale}
            numTicks={6}
            stroke={"transparent"}
            tickLabelProps={axisBottomTickLabelProps}
            hideTicks
            tickStroke={"grey"}
          />
          <Bar
            x={margin.left}
            y={margin.top}
            width={innerWidth}
            height={innerHeight}
            fill="transparent"
            rx={14}
            onTouchStart={handleTooltip}
            onTouchMove={handleTooltip}
            onMouseMove={handleTooltip}
            onMouseLeave={() => hideTooltip()}
          />
          {tooltipData && !isDragging && (
            <g>
              <Line
                from={{ x: tooltipLeft, y: margin.top }}
                to={{ x: tooltipLeft, y: innerHeight + margin.top }}
                stroke={colorTo}
                strokeWidth={1}
                pointerEvents="none"
              />
              <circle
                cx={tooltipLeft}
                cy={tooltipTop + 1}
                r={4}
                fill="black"
                fillOpacity={0.1}
                stroke="black"
                strokeOpacity={0.1}
                strokeWidth={2}
                pointerEvents="none"
              />
              <circle
                cx={tooltipLeft}
                cy={tooltipTop}
                r={4}
                fill={colorTo}
                stroke="white"
                strokeWidth={2}
                pointerEvents="none"
              />
            </g>
          )}
        </SvgWrap>
        {tooltipData && !isDragging && (
          <div>
            <TooltipWithBounds
              key={Math.random()}
              top={tooltipTop - 75}
              left={tooltipLeft - 65}
              style={tooltipStyles(colorTo)}
            >
              <TooltipDataContainer>
                <span>{`Time: ${formatDate(getDate(tooltipData))}`}</span>
                <b>{`${getStockValue(tooltipData)}`}</b>
              </TooltipDataContainer>
            </TooltipWithBounds>
          </div>
        )}
      </Wrap>
    );
  }
);

const TooltipDataContainer = styled.div`
  display: flex;
  flex-direction: column;
  justify-content: center;
  align-items: center;
  gap: 5px;
  & > b {
    ${({ theme }) => theme.typography.body2};
  }
`;

const Wrap = styled.div`
  width: 100%;
  height: 100%;
  touch-action: none;
`;

const SvgWrap = styled.svg`
  touch-action: none;
`;
