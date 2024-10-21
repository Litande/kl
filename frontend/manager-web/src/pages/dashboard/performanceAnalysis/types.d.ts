export type Period = {
  label: string;
  from: string;
  to: string;
  step: number;
};
export type PeriodOptions = {
  day: Period;
  week: Period;
  month: Period;
};

export type PlotChartResponse = {
  from: string;
  to: string;
  step: number;
  type: string;
  values: Array<PlotChartValue>;
};

export type PlotChartValue = { date: string; value: number };

export type StatisticsChartResponse = {
  from: string;
  to: string;
  type: string;
  trend: number;
  value: number;
};
