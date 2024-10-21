import { ButtonSaveProps } from "pages/settings/SettingsHelper";

export enum SettingType {
  Telephony = "Telephony",
  Feedback = "Feedback",
  AgentPermanentLeadAssignment = "AgentPermanentLeadAssignment",
  CallHours = "CallHours",
  ProductiveDialer = "ProductiveDialer",
  DroppedCall = "DroppedCall",
  VoiceMail = "VoiceMail",
}

export type BaseAction = ButtonSaveProps;
