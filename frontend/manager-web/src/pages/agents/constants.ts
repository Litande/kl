import { AgentStatusStr, agentStatusToLabel } from "types";

const statusWeight = {
  [AgentStatusStr.Break]: 6,
  [AgentStatusStr.Offline]: 7,
};

const sort = (a, b) => statusWeight[a.state] - statusWeight[b.state];

export const columnDefs = [
  { key: AgentStatusStr.InTheCall, label: agentStatusToLabel[AgentStatusStr.InTheCall] },
  {
    key: [AgentStatusStr.WaitingForTheCall, AgentStatusStr.Dialing],
    label: agentStatusToLabel[AgentStatusStr.WaitingForTheCall],
  },
  {
    key: AgentStatusStr.FillingFeedback,
    label: agentStatusToLabel[AgentStatusStr.FillingFeedback],
  },
  {
    key: [AgentStatusStr.Offline, AgentStatusStr.Break],
    label: "Offline & Breaks",
    sort,
  },
];
