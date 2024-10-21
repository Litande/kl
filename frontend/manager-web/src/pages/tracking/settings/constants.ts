export const defaultValues = {
  endCallButtonDelay: "",
  maxRingDuration: "",
  callTimeout: "",
  busyStatus: [],
  errorStatus: [],
  faxStatus: [],
};

export const TEAMS_TAB = {
  name: "teams",
  label: "Teams",
};

export const SETTINGS_TAB = {
  name: "settings",
  label: "Settings",
};

export const MODAL_TABS = [TEAMS_TAB];

export const attributes = [
  {
    key: "online",
    label: "Online",
    value: 3,
  },
  {
    key: "offline",
    label: "Offline",
    value: 3,
  },
];

export const GENERAL_TAB = {
  name: "general",
  label: "Productivite Dialer Settings",
};

export const DROPPED_CALL_TAB = {
  name: "callDrop",
  label: "Dropped Call Settings",
};

export const VOICEMAIL_TAB = {
  name: "voicemail",
  label: "Voicemail Settings",
};

export const FORM_TABS = [GENERAL_TAB, DROPPED_CALL_TAB, VOICEMAIL_TAB];
