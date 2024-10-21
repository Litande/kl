import moment from "moment";
import MiniChart from "./MiniChart";

export const chartsOption = [
  {
    title: "Av. number of calls",
    type: "AvgCall",
    value: 0,
    trend: 0,
    chart: MiniChart,
    color: "#9DC7FC",
    accentColor: "#9DC7FC",
  },
  {
    title: "Av. new clients",
    type: "AvgNewClient",
    value: 0,
    trend: 0,
    chart: MiniChart,
    color: "#FF7A00",
    accentColor: "#FF7A00",
  },
  {
    type: "SuccessCall",
    title: "Successful calls",
    value: 0,
    trend: 0,
    chart: MiniChart,
    color: "#64C458",
    accentColor: "#64C458",
  },
];

export const periodOptions = {
  day: {
    label: "Day",
    from: moment().utc().subtract(1, "day").format(),
    to: moment().utc().format(),
    step: 10,
  },
  week: {
    label: "Week",
    from: moment().utc().subtract(1, "week").format(),
    to: moment().utc().format(),
    step: 30,
  },
  month: {
    label: "Month",
    from: moment().utc().subtract(1, "month").format(),
    to: moment().utc().format(),
    step: 60,
  },
};
