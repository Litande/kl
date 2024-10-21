import { getCallsAction, applyFiltersAction, applyPaginationAction } from "./slice";

const getCalls = appFeatures => () => appFeatures.dispatch(getCallsAction());

const defaultSearch = {
  callType: null,
  country: null,
  agents: [],
  leadStatusAfter: null,
  duration: null,
  tillDate: null,
  fromDate: null,
};

const applySearch =
  appFeatures =>
  (searchData = defaultSearch) => {
    appFeatures.dispatch(applyFiltersAction(searchData));
  };

const applyPagination =
  appFeatures =>
  ({ page }) => {
    appFeatures.dispatch(applyPaginationAction(page));
  };

const behavior = {
  getCalls,
  applySearch,
  applyPagination,
};

export default behavior;
