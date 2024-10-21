import React from "react";
import { AgentStatusStr } from "types";
import { ConnectionMode } from "services/callService/types";

export interface IRow {
  id: number;
  name: string;
  leadStatus?: string;
  callDuration: number;
  state: AgentStatusStr;
  leadId?: string;
  leadName?: string;
  brandName: string;
  affiliateId?: string;
  registrationTime?: string;
  phoneNumber: string;
  country?: string;
  callId: string;

  fullWidth?: boolean;
  managerRtcUrl?: string;
  callFinishedAt?: null | string;
  // can't find in response
  group: string;
  details?: string;
  score: number;
  // get from response
  groups: null | string[];
  leadAnsweredAt?: string;
  callOriginatedAt?: string;
  // eslint-disable-next-line @typescript-eslint/no-explicit-any
  source?: any;
  teamIds: number[];
  // eslint-disable-next-line @typescript-eslint/no-explicit-any
  weight?: any;
}

export interface IAction {
  name: string;
  component: React.FC;
  mode: ConnectionMode;
  tooltip?: string;
}
