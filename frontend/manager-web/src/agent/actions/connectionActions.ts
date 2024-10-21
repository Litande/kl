import eventDomDispatcher from "services/events/EventDomDispatcher";
import { changeStateAction, STATES } from "agent/actions/changeStateAction";

export const CONNECT_EVENT = "CONNECT_EVENT";
export const DISCONNECT_EVENT = "DISCONNECT_EVENT";
export const CLOSE_PEER_EVENT = "CLOSE_PEER_EVENT";
export const RECEIVE_LEAD_EVENT = "RECEIVE_LEAD_EVENT";
export const PEER_CLOSED_EVENT = "PEER_CLOSED_EVENT";

const { dispatchEvent } = eventDomDispatcher();
let nextState: STATES;

export const connectAction = agentID => {
  dispatchEvent(new CustomEvent(CONNECT_EVENT, { detail: { agentID: agentID } }));
};

export const disconnectAction = state => {
  nextState = state;
  dispatchEvent(new CustomEvent(DISCONNECT_EVENT));
};

export const closePeerAction = () => {
  dispatchEvent(new CustomEvent(CLOSE_PEER_EVENT));
};

// onclose peer connection
export const closedPeerAction = () => {
  setTimeout(() => {
    changeStateAction(nextState || STATES.WAITING_STATE);
  }, 1000);
};
