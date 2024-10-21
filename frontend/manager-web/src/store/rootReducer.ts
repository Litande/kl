import auth from "pages/authorization/authSlice";
import callRecordings from "pages/calls/slice";
import tags from "pages/tags/slice";
import agentsList from "pages/agents/slice";
import ruleEngineGroups from "components/ruleEngine/groupSlice";
import ruleEngineRules from "components/ruleEngine/rulesSlice";
import navigation from "components/navigation/navigationSlice";
import fullScreen from "components/fullScreen/fullScreenSlice";

const rootReducer = {
  auth,
  ruleEngineGroups,
  callRecordings,
  tags,
  ruleEngineRules,
  agentsList,
  navigation,
  fullScreen,
};

export default rootReducer;
