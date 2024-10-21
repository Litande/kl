import { changeStateAction, STATES } from "agent/actions/changeStateAction";

export const clockInAction = () => {
  changeStateAction(STATES.WAITING_STATE);
};
