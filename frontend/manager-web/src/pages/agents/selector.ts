import { createSelector } from "@reduxjs/toolkit";
import { RootState } from "store";
import { IOption } from "./types";

export const isLoadingSelector = createSelector(
  (state: RootState) => state.agentsList,
  agentListState => agentListState.isLoading
);

export const filterOptionsSelector = createSelector(
  (state: RootState) => state.agentsList,
  agentListState => agentListState.filterOptions
);

export const appliedFiltersSelector = createSelector(
  (state: RootState) => state.agentsList,
  agentListState => agentListState.appliedFilters
);

export const groupsFilter = createSelector(appliedFiltersSelector, ({ groups }) => groups);

export const groupsSelector = createSelector(
  [groupsFilter, filterOptionsSelector],
  (filter, { groups }) => {
    if (!filter.length) return groups;

    return groups.filter(group => filter.some(i => i.value === group.value));
  }
);

export const agentsListFilters = createSelector(
  appliedFiltersSelector,
  ({ states, tags, users }) => ({ states, tags, users })
);

export const agentsSelector = createSelector(
  (state: RootState) => state.agentsList,
  agentListState => agentListState.list
);

const filterTypeToKey = {
  users: "id",
  states: "state",
  tags: "tags",
};

export const filteredAgentsSelector = createSelector(
  [agentsListFilters, agentsSelector],
  (filters, agents) => {
    const f: [string, IOption[]][] = Object.entries(filters).filter(([key, value]) => value.length);

    return f.reduce((res, [key, options]) => {
      return res.filter(agent => {
        if (Array.isArray(agent[filterTypeToKey[key]])) {
          return options.some(option => agent[filterTypeToKey[key]].some(i => i === option.value));
        }

        return options.some(option => option.value === agent[filterTypeToKey[key]]);
      });
    }, agents);
  }
);
