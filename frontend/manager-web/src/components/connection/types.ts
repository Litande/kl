export enum CONNECTION_STATUS {
  STABLE_CONNECTION = 4,
  GOOD_CONNECTION = 3,
  POOR_CONNECTION = 2,
  UNSTABLE_CONNECTION = 1,
  NO_CONNECTION = 0,
}

export enum ColorsEnum {
  SUCCESS = "#64C458",
  UNSTABLE = "#FFB800",
  FAIL = "#000000",
  RED = "#C80048",
}

export enum OpacityEnum {
  SUCCESS = 1,
  FAIL = 0.3,
}

export interface IFill {
  fill: string;
  opacity: number;
}

export interface IFillStatus {
  line1: IFill;
  line2: IFill;
  line3: IFill;
  line4: IFill;
  dotLine?: IFill;
  noConnect?: IFill;
  description: string;
}
