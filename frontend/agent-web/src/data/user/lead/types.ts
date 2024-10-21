import { LeadStatus } from "data/user/types";

export const FEEDBACK_TIME_DEFAULT = 300; // seconds

export type DialerLead = {
  id: number;
  clientId: number;
  dataSourceId: number;
  externalId: string;
  sessionId: string;
  status: string;
  phone: string;
  lastUpdateTime: string;
  registrationTime: string;
  leadAnsweredAt: string;
  callFinishAt: string;
  agentAnsweredAt: string;
  firstName: string;
  lastName: string;
  callTime: number;
  feedbackTime?: number;
  agentRtcUrl: string; // former WebrtcEndpoint
  iframeUrl: null | string;
  availableStatuses: Array<LeadStatus | string> | null;
};
