export interface ICallHourItem {
  country: string;
  from: string;
  till: string;
}

export type CountryType = {
  name: string;
  offset: number;
};

export interface ICallHours {
  country: CountryType;
  callHours: Array<ICallHourItem>;
}
