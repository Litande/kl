import { createSelector } from "@reduxjs/toolkit";
import { RootState } from "store";
import { TagStatus } from "types";

export const isLoadingSelector = createSelector(
  (state: RootState) => state.tags,
  tagsState => tagsState.isLoading
);
export const selectedTagsSelector = createSelector(
  (state: RootState) => state.tags,
  tagsState => tagsState.selectedTags
);

export const scoringRulesSelector = createSelector(
  (state: RootState) => state.tags,
  rulesState => rulesState.scoringRules
);

export const enableScoringRulesSelector = createSelector(
  (state: RootState) => state.tags,
  rulesState => rulesState.scoringRules.filter(({ status }) => status === TagStatus.Enable)
);

export const filteredAgentsSelector = createSelector(
  (state: RootState) => state.tags,
  tagsState => {
    const { agents, selectedTags } = tagsState;

    if (selectedTags.length) {
      const selectedTagsIds = selectedTags.map(({ id }) => id);
      const findAgentTagsInSelectedTags = agent => {
        const agentTags = agent.tags.map(({ id }) => id);
        return selectedTagsIds.every(id => agentTags.includes(id));
      };
      return agents.filter(findAgentTagsInSelectedTags);
    }

    return agents;
  }
);
