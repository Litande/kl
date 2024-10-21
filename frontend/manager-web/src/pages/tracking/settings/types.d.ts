import { IOption } from "components/multiSelect/MultiSelect";

export interface IFormValues {
  endCallButtonDelay: string;
  maxRingDuration: string;
  callTimeout: string;
  busyStatus: IOption[];
  errorStatus: IOption[];
  faxStatus: IOption[];
}
