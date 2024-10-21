import { LinePath } from "@visx/shape";
import { scaleTime, scaleLinear } from "@visx/scale";
import { curveNatural } from "@visx/curve";
import { LinearGradient } from "@visx/gradient";
import { max, extent } from "d3-array";
import moment from "moment";
import { PlotChartResponse, PlotChartValue } from "pages/dashboard/performanceAnalysis/types";

type AreaProps = {
  colorFrom?: string;
  color?: string;
  accentColor?: string;
  chartData: PlotChartResponse["values"];
  type: string;
};

const getDate = (d: PlotChartValue) => moment(d.date).utc();
const getStockValue = (d: PlotChartValue) => d?.value || 0;

export default function MiniChart({
  colorFrom = "#fff",
  color,
  accentColor,
  type,
  chartData = [{ date: moment().utc().format(), value: 0 }],
}: AreaProps) {
  const width = 120;
  const height = 45;
  // bounds

  const xScale = scaleTime({
    range: [0, width],
    domain: extent(chartData, getDate),
  });

  const yScale = scaleLinear({
    range: [height, 0],
    domain: [0, (max(chartData, getStockValue) || 0) + height / 3],
    nice: true,
  });

  return (
    <svg height={height} width={width}>
      <rect x={0} y={0} width={width} height={height} style={{ fill: colorFrom }} rx={14} />
      <LinearGradient id={`${type}-background-gradient`} from={accentColor} to={colorFrom} />
      <LinePath
        data={chartData}
        x={d => xScale(getDate(d))}
        y={d => yScale(getStockValue(d))}
        fill={`url("#${type}-background-gradient")`}
        curve={curveNatural}
      />

      <LinearGradient id={`${type}-line-gradient`} from={accentColor} to={color} />

      <LinePath
        data={chartData}
        x={d => xScale(getDate(d))}
        y={d => yScale(getStockValue(d))}
        stroke={`url("#${type}-line-gradient")`}
        strokeWidth={3}
        curve={curveNatural}
      />
    </svg>
  );
}
