import { createSelector } from "@reduxjs/toolkit";

import { RootState } from "store";
import { IForm } from "components/ruleEngine/types";
import { STATUS_OPTIONS } from "components/ruleEngine/constants";
import { transformFieldsFromApiToForm } from "components/ruleEngine/utils";

export const conditionsActionsSelector = createSelector(
  (state: RootState) => state.ruleEngineRules,
  ({ conditions, actions }) => ({ conditions, actions })
);

export const ruleLoadingStateSelector = (state: RootState) => state.ruleEngineRules.loading;

export const ruleDetailsSelector = createSelector(
  (state: RootState) => state.ruleEngineRules,
  ({ fetchedRuleData: ruleData, conditions: conditionOptions, actions: actionOptions }): IForm => {
    if (!ruleData || !conditionOptions || !actionOptions) return null;

    const { combination, actions } = ruleData.rules;

    const conditionsData = combination.groups
      ? combination.groups.map(condition => {
          const rule = conditionOptions.find(
            opt => opt.name?.toLowerCase() === condition.name?.toLowerCase()
          );
          if (!rule) return null;

          const comparisonOperation = (
            conditionOptions.find(el => el.name === rule.name)?.comparisonOperation || []
          ).find(op => op.value?.toLowerCase() === condition?.comparisonOperation?.toLowerCase());
          const optionFields = conditionOptions.find(el => el.name === rule.name)?.fields;
          const fields = transformFieldsFromApiToForm(condition.fields, optionFields);
          return {
            name: { label: rule.displayName, value: rule.name },
            comparisonOperation,
            fields,
          };
        })
      : [];

    const actionsData = actions
      ? actions.map(action => {
          const rule = actionOptions.find(
            opt => opt.name?.toLowerCase() === action.name?.toLowerCase()
          );
          if (!rule) return null;

          const actionOperation = (
            actionOptions.find(el => el.name === rule.name)?.actionOperation || []
          ).find(op => op.value?.toLowerCase() === action?.actionOperation?.toLowerCase());
          const optionFields = actionOptions.find(el => el.name === rule.name)?.fields;
          const fields = transformFieldsFromApiToForm(action.fields, optionFields);

          return {
            name: { label: rule.displayName, value: rule.name },
            actionOperation,
            fields,
          };
        })
      : [];

    const status = STATUS_OPTIONS.find(s => s.value === ruleData.status);

    const data: IForm = {
      name: ruleData.name,
      status,
      conditions: conditionsData,
      actions: actionsData,
    };

    return data;
  }
);
