import { IOption } from "components/select/Select";

export interface ICountry {
  id: number;
  label: string;
}

export const mockCountries: Array<ICountry> = [
  {
    id: 1,
    label: "United Kingdom",
  },
  {
    id: 2,
    label: "Singapore",
  },
  {
    id: 3,
    label: "South Africa",
  },
  {
    id: 4,
    label: "Australia",
  },
];

export const optionsCountries: Array<IOption> = mockCountries.map(country => {
  return {
    value: country.label,
    ...country,
  };
});
