import { RuleTypes, IRuleData } from "components/ruleEngine/types";
import {
  createRule,
  getRuleDetails,
  loadActions,
  loadConditions,
  updateRule,
} from "components/ruleEngine/rulesSlice";

const getConditionsAndActions = appFeatures => (ruleType: RuleTypes) => {
  appFeatures.dispatch(loadActions(ruleType));
  appFeatures.dispatch(loadConditions(ruleType));
};

const getRuleData = appFeatures => data => {
  appFeatures.dispatch(getRuleDetails(data));
};

const submitRule = appFeatures => (ruleType, urlParams) => {
  return payload => {
    const groups = payload.conditions.map(el => ({
      name: el.name?.value,
      comparisonOperation: el.comparisonOperation?.value,
      fields: el.fields?.map(field => ({
        ...field,
        value: Array.isArray(field.value)
          ? field.value.map(el => el.value).join()
          : typeof field.value === "string"
          ? field.value
          : field.value?.value,
      })),
    }));

    const acitons: IRuleData["rules"]["actions"] = payload.actions.map(el => ({
      name: el.name?.value,
      actionOperation: el.actionOperation?.value,
      fields: el.fields?.map(field => ({
        type: field.type,
        value: Array.isArray(field.value)
          ? field.value.map(el => el.value).join()
          : typeof field.value === "string"
          ? field.value
          : field.value?.value,
      })),
    }));
    const rules = {
      combination: {
        operator: "and",
        groups: groups,
        combination: [],
      },
      actions: acitons,
    };
    const ruleData = {
      name: payload.name,
      status: payload.status.value,
      rules,
    };
    if (urlParams.groupId && urlParams.ruleId) {
      appFeatures.dispatch(updateRule({ ruleType, urlParams, data: ruleData }));
    } else {
      appFeatures.dispatch(createRule({ ruleType, groupId: urlParams.groupId, data: ruleData }));
    }
  };
};

const ruleConditionsActionsBehavior = {
  getConditionsAndActions,
  submitRule,
  getRuleData,
};

export default ruleConditionsActionsBehavior;
