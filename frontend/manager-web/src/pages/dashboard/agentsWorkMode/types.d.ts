import { Dispatch, SetStateAction } from "react";

export type ChartDataType = {
  symbol: string;
  amount: number;
  color: string;
};

export type LegendProps = {
  data: { symbol: string; amount: number; color: string }[];
  setActive: Dispatch<SetStateAction<ChartDataType>>;
};
