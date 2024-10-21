import { IOption } from "components/multiSelect/MultiSelect";

export enum AgentStatusStr {
  WaitingForTheCall = "WaitingForTheCall",
  InTheCall = "InTheCall",
  FillingFeedback = "FillingFeedback",
  Offline = "Offline",
  Dialing = "Dialing",
  Busy = "Busy",
  Break = "InBreak",
}

export const agentStatusToLabel = {
  [AgentStatusStr.InTheCall]: "On a call",
  [AgentStatusStr.WaitingForTheCall]: "Waiting for the call",
  [AgentStatusStr.FillingFeedback]: "Feedback",
  [AgentStatusStr.Offline]: "Offline",
  [AgentStatusStr.Dialing]: "Dialing",
  [AgentStatusStr.Busy]: "Busy",
  [AgentStatusStr.Break]: "Break",
};

export interface ILead {
  leadId: number;
  name: string;
  leadStatus: string;
  leadAnsweredAt: string;
  agentAnsweredAt: string;
  registrationTime: string;
  groups: [];
  leadGroup: string;
  brandName: string;
  affiliateId: number;
  phoneNumber: string;
  country: string;
  assignedAgent: { agentId: number };
}

export enum TagStatus {
  Enable = "Enable",
  Disable = "Disable",
}

export enum CallStatus {
  Unknown = 0,
  CallFinishedByLead = 100,
  CallFinishedByAgent = 101,
  SessionTimeout = 201,
  NoAvailableAgents = 202,
  LeadLineBusy = 203,
  LeadInvalidPhone = 204,
  AgentNotAnswerLeadHangUp = 205,
  AgentReconnectTimeout = 206,
  SIPTransportError = 300,
  RTCTransportTimeout = 400,
}

export interface IAgent {
  email: string;
  name: string;
  lastName: string;
  firstName: string;
  managerRtcUrl: string;
  leadQueueIds: number[];
  agentId: number;
  id: number;
  score: number;
  status: AgentStatusStr | string;
  state: AgentStatusStr | string;
  tags: { id: number; name: string; value: number }[];
  teamIds: number[];
}

export type ILabelValue = IOption;

export type ITeam = {
  name: string;
  teamId: number;
};
