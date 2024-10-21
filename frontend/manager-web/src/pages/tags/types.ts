import { AgentStatusStr, TagStatus } from "types";

export interface IRow {
  id?: number;
  agentId?: number;
  firstName: string;
  lastName: string;
  teamName: string;
  score: number;
  tags: Array<ITag>;
  teamIds: Array<number>;
}

export interface ITag {
  id: number;
  name: string;
}

export type IFormValues = {
  name: string;
  action: SelectType;
  status: SelectType;
  value: number;
  lifetimeSeconds: SelectType;
};

export interface IScoringRule {
  id?: number;
  name: string;
  status: TagStatus;
  value: number;
  lifetimeSeconds?: number;
}

export type IAgentRequest = {
  id: number;
  agentId: number;
  email: string;
  firstName: string;
  lastName: string;
  leadQueueIds: Array<number>;
  score: number;
  status: AgentStatusStr;
  tagIds: Array<number>;
  tags: Array<ITag>;
  teamIds: Array<number>;
  teamName: string;
  userName: string;
};

export enum Action {
  GAINS = 0,
  REDUCES = 1,
}

export type SelectType = {
  label: string;
  value: string | number;
};

export const ACTION_OPTIONS = [
  { value: Action.GAINS, label: "Gains" },
  { value: Action.REDUCES, label: "Reduces" },
];

export const DURATION_OPTIONS = [
  { value: 3600, label: "1h" },
  { value: 7200, label: "2h" },
  { value: 10800, label: "3h" },
  { value: 14400, label: "4h" },
  { value: 21600, label: "6h" },
  { value: 28800, label: "8h" },
  { value: 43200, label: "12h" },
  { value: 86400, label: "1day" },
  { value: null, label: "always" },
];

export type ISelectedTag = {
  id: number;
  name: string;
};

export const STATUS_OPTIONS = [
  { value: TagStatus.Enable, label: "Enable" },
  { value: TagStatus.Disable, label: "Disable" },
];
