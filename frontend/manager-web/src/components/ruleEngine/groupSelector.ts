import { createSelector } from "@reduxjs/toolkit";

import { RootState } from "store";

export const isLoadingSelector = createSelector(
  (state: RootState) => state.ruleEngineGroups,
  rulesState => rulesState.loading
);

export const groupsSelector = createSelector(
  (state: RootState) => state.ruleEngineGroups,
  rulesState => rulesState.groups
);

export const getRulesByGroupId = groupId =>
  createSelector(
    (state: RootState) => state.ruleEngineGroups.groups,
    groups => groups.find(group => group.groupId === groupId)?.rules || []
  );

export const getRuleById = (groupId: string | number, ruleId: string | number) =>
  createSelector(
    (state: RootState) => state.ruleEngineGroups.groups,
    groups =>
      (groups.find(group => String(group.groupId) === String(groupId))?.rules || []).find(
        rule => String(rule?.ruleId) === String(ruleId)
      ) ?? null
  );
