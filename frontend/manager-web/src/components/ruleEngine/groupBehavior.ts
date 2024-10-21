import { RuleTypes } from "components/ruleEngine/types";
import {
  loadGroups,
  updateGroup,
  createGroup,
  removeGroup,
} from "components/ruleEngine/groupSlice";
import { removeRule } from "./rulesSlice";

const getRuleGroupsBehavior = appFeatures => (ruleType: RuleTypes) => {
  appFeatures.dispatch(loadGroups(ruleType));
};

const editGroupBehavior =
  appFeatures =>
  ({ toggle, groupId, ruleType }) =>
  groupData => {
    const data = {
      name: groupData.groupName,
      status: groupData.status.value,
    };
    appFeatures.dispatch(updateGroup({ data, groupId, ruleType }));
    toggle();
  };

const removeGroupBehavior =
  appFeatures =>
  ({ groupId, ruleType }) => {
    appFeatures.dispatch(removeGroup({ groupId, ruleType }));
  };

const removeRuleBehavior =
  appFeatures =>
  ({ groupId, ruleId, ruleType }) => {
    appFeatures.dispatch(removeRule({ ruleId, groupId, ruleType }));
  };

const addGroupBehavior =
  appFeatures =>
  ({ ruleType }) =>
  groupData => {
    const data = {
      name: groupData.groupName,
      status: groupData.status.value,
    };
    appFeatures.dispatch(createGroup({ data, ruleType }));
  };

const groupsBehavior = {
  getRuleGroups: getRuleGroupsBehavior,
  editGroup: editGroupBehavior,
  addGroup: addGroupBehavior,
  removeGroup: removeGroupBehavior,
  removeRule: removeRuleBehavior,
};

export type AuthBehaviors = typeof groupsBehavior;
export default groupsBehavior;
