export interface ICountryItem {
  country: string;
  id?: string;
  amount: number;
  rate: string;
  avgTime: string;
  code: string; // country code ISO 3166-1 alpha-3 e.g. UKR
}

export const Period = {
  day: {
    label: "Day",
    value: "Today",
  },
  week: {
    label: "Week",
    value: "Week",
  },
  month: {
    label: "Month",
    value: "Month",
  },
  year: {
    label: "Year",
    value: "Year",
  },
};
