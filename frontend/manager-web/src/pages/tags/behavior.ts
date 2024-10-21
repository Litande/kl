import {
  getAllAgents as getAllAgentsAction,
  updateAgentAction,
  createScoringRule,
  deleteScoringRule,
  updateScoringRule,
  updateSelectedTags as updateSelectedTagsAction,
  deleteSelectedTag as deleteSelectedTagAction,
  deleteAllSelectedTags as deleteAllSelectedTagsAction,
  getAllTags as getAllTagsAction,
  getTeams as getTeamsAction,
} from "./slice";
import { getValueDependsOnAction } from "./utils";

const getAllAgents = appFeatures => () => appFeatures.dispatch(getAllAgentsAction());

const createRule =
  appFeatures =>
  ({ toggle, reset }) =>
  scoringRule => {
    const { name, value, action, lifetimeSeconds, status } = scoringRule;
    appFeatures.dispatch(
      createScoringRule({
        name,
        status: status.value,
        value: getValueDependsOnAction(value, action),
        lifetimeSeconds: lifetimeSeconds.value,
      })
    );
    toggle();
    reset();
  };

const updateRule = appFeatures => scoringRule => {
  const { value, action, lifetimeSeconds, status } = scoringRule;
  appFeatures.dispatch(
    updateScoringRule({
      ...scoringRule,
      status: status.value,
      value: getValueDependsOnAction(value, action),
      lifetimeSeconds: lifetimeSeconds.value,
    })
  );
};

const deleteRule = appFeatures => id => appFeatures.dispatch(deleteScoringRule(id));

const updateSelectedTags = appFeatures => tags =>
  appFeatures.dispatch(updateSelectedTagsAction(tags));

const deleteSelectedTag = appFeatures => id => appFeatures.dispatch(deleteSelectedTagAction(id));

const deleteAllSelectedTags = appFeatures => () =>
  appFeatures.dispatch(deleteAllSelectedTagsAction());

const updateAgent = appFeatures => agent => appFeatures.dispatch(updateAgentAction(agent));

const getAllTags = appFeatures => () => appFeatures.dispatch(getAllTagsAction());

const getTeams = appFeatures => () => appFeatures.dispatch(getTeamsAction());

const behavior = {
  createRule,
  updateRule,
  deleteRule,
  getAllAgents,
  updateAgent,
  updateSelectedTags,
  deleteSelectedTag,
  deleteAllSelectedTags,
  getAllTags,
  getTeams,
};

export default behavior;
