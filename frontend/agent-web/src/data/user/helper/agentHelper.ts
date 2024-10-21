import { AgentStatus } from "data/user/types";

export const clockOutIsAvailable = (status: AgentStatus) => {
  return status !== AgentStatus.Offline;
};

export const clockInIsAvailable = (status: AgentStatus) => {
  return status === AgentStatus.Offline;
};
