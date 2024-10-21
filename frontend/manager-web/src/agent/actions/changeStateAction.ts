import eventDomDispatcher from "services/events/EventDomDispatcher";

export const CHANGE_STATE_EVENT = "CHANGE_STATE_EVENT";

export enum STATES {
  MAIN_STATE = "MAIN_STATE",
  WAITING_STATE = "WAITING_STATE",
  CALLING_STATE = "CALLING_STATE",
}

export const changeStateAction = state => {
  const { dispatchEvent } = eventDomDispatcher();

  dispatchEvent(new CustomEvent(CHANGE_STATE_EVENT, { detail: { state: state } }));
};
