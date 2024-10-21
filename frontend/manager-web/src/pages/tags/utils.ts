import { Action } from "./types";

const SCORING_RULES_LOCAL_STORAGE_KEY = "scoringRules";
const SELECTED_TAGS_LOCAL_STORAGE_KEY = "selectedTags";

export const saveScoringRulesInLocalStorage = scoringRules =>
  window.localStorage.setItem(SCORING_RULES_LOCAL_STORAGE_KEY, JSON.stringify(scoringRules));

export const saveSelectedTagsInLocalStorage = scoringRules =>
  window.localStorage.setItem(SELECTED_TAGS_LOCAL_STORAGE_KEY, JSON.stringify(scoringRules));

export const getSelectedTagsFromLocalStorage = () =>
  JSON.parse(window.localStorage.getItem(SELECTED_TAGS_LOCAL_STORAGE_KEY));

export const getValueDependsOnAction = (value, action) =>
  value * (action.value === Action.GAINS ? 1 : -1);

export const getValueText = value =>
  value >= 0 ? `Gains ${value} weight` : `Reduces weight by ${value * -1}`;

const twelveHoursInSeconds = 43200;

export const getDurationText = lifetimeSeconds => {
  if (lifetimeSeconds === null) {
    return "Forever";
  }
  if (lifetimeSeconds > twelveHoursInSeconds) {
    return "For 1 day";
  }

  return `For ${lifetimeSeconds / 3600} hour(s)`;
};

export const getTeamName = (state, agent) =>
  state.teams.find(({ teamId }) => teamId === agent?.teamIds[0])?.name || "";

export const isOverflown = element => {
  if (!element) {
    return false;
  }
  return element.scrollHeight > element.clientHeight || element.scrollWidth > element.clientWidth;
};
