import { ILabelValue } from "types";

export interface IFormValues {
  groupName: string;
  leadId: string;
  leadPhone: string;
  userId: string;
  userName: string;
  country: Array<ILabelValue>;
  agents: Array<ILabelValue>;
  callType: Array<ILabelValue>;
  leadStatusAfter: Array<ILabelValue>;
  fromDate: string;
  tillDate: string;
  duration: Array<ILabelValue>;
}

export type ICallType = "manual" | "predictive";

export type IFormKey = keyof IFormValues;

export interface IForm {
  onSubmit: (data: IFormValues) => void;
}

type IDurationOperation = "moreThan" | "lessThan" | "equal";

export type IDuration = {
  operation: IDurationOperation;
  value: number;
};
