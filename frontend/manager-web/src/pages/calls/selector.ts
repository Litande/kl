import { createSelector } from "@reduxjs/toolkit";
import { RootState } from "store";

export const isLoadingSelector = createSelector(
  (state: RootState) => state.callRecordings,
  rulesState => rulesState.isLoading
);

export const appliedFiltersSelector = createSelector(
  (state: RootState) => state.callRecordings,
  ({ appliedFilters }) => {
    const { callType, country, leadStatusAfter, duration, tillDate, fromDate, agents } =
      appliedFilters;

    return {
      ...appliedFilters,
      callType: callType?.length ? callType[0].value : null,
      country: country?.length ? country.map(({ value }) => value) : null,
      leadStatusAfter: leadStatusAfter?.length ? leadStatusAfter.map(({ value }) => value) : null,
      duration: duration?.length
        ? {
            operation: "moreThan",
            value: duration[0].value,
          }
        : null,
      fromDate: fromDate ? fromDate : null,
      tillDate: tillDate ? tillDate : null,
      agents: agents?.length ? agents.map(({ value }) => value) : null,
    };
  }
);

export const paginationSelector = createSelector(
  (state: RootState) => state.callRecordings,
  ({ callsList }) => ({
    page: callsList.page,
    pageSize: callsList.pageSize,
  })
);
export const callListSelector = createSelector(
  (state: RootState) => state.callRecordings,
  rulesState => rulesState.callsList
);
