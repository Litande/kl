import { combineEpics } from "redux-observable";
import { authEpic } from "pages/authorization/authEpic";

const rootEpic = combineEpics(authEpic);

export default rootEpic;
