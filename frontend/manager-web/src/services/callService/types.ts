export enum ConnectionMode {
  ListenOnly = "ListenOnly",
  AgentOnly = "AgentOnly",
  BothDirections = "BothDirections",
}

export enum CallStatus {
  OFFLINE = "OFFLINE",
  PREPARING = "PREPARING",
  IN_CALL = "IN_CALL",
}

export type CallInfo = {
  rtcUrl: string;
  mode: ConnectionMode;
  firstName?: string;
  lastName?: string;
  callTime: string;
  phone: string;
  agent: string;
};

export enum Command {
  HangupCall = "hangupCall",
  SetManagerMode = "setManagerMode",
  DropAgent = "dropAgent",
}

export enum HangupReason {
  na = "na",
}
