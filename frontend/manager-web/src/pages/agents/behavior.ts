import { applyFiltersAction, loadAgents, loadTags, loadTeams, updateAgentsAction } from "./slice";

const getGroups = appFeatures => () => {
  appFeatures.dispatch(loadAgents());
};

const getTags = appFeatures => () => {
  appFeatures.dispatch(loadTags());
};

const getTeams = appFeatures => () => {
  appFeatures.dispatch(loadTeams());
};

const applyFilters = appFeatures => filters => appFeatures.dispatch(applyFiltersAction(filters));

const updateAgents = appFeatures => data => appFeatures.dispatch(updateAgentsAction(data));

const behavior = {
  getTeams,
  getTags,
  getGroups,
  applyFilters,
  updateAgents,
};

export default behavior;
