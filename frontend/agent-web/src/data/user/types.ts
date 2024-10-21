import { ROLES } from "router/enums";
import AgentModel from "data/user/AgentModel";

export enum AgentStatus {
  WaitingForTheCall = "WaitingForTheCall",
  InTheCall = "InTheCall",
  FillingFeedback = "FillingFeedback",
  Offline = "Offline", // clock out
  Online = "Online", // clock in
  Break = "InBreak",
  InManualCall = "ManualCall", // temporary, just for FE, may be should change to BE statuses
  Unknown = "Unknown", // system status to show loader
}

export enum CallType {
  Predictive = "Predictive",
  Manual = "Manual",
}

export type LeadStatus = {
  label: string;
  value: string;
};

export type Agent = {
  id: number;
  firstName: string;
  lastName: string;
  email: string;
  role: ROLES | null;
  status?: AgentStatus;
  authToken?: string;
  iceServers?: string[];
};

export type BaseAuthContext = {
  agent: AgentModel | null;
  setAgent: (user: Agent | null) => void;
};

export type Break = {
  id: number;
  duration: number;
  label: string;
  isScheduled: boolean;
  isActive: boolean;
  isAvailable: boolean;
  isUsed: boolean;
  startedAt: Date;
};

export const phoneSymbols = ["1", "2", "3", "4", "5", "6", "7", "8", "9", "*", "0", "#", "+"];

export type Settings = {
  endCallButtonAfterThisAmountOfSeconds: string;
  hideVoicemailButtonAfterThisAmountOfSecondsOfCall: string;
  maxCallDuration: number;
  pageTimeout: number;
  redialsLimit: number;
  showVoicemailButtonAfterThisAmountOfSecondsOfCall: string;
};
