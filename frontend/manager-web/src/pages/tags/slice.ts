import { createAction, createAsyncThunk, createSlice } from "@reduxjs/toolkit";

import {
  createTag,
  deleteTag,
  fetchAllAgents,
  fetchAllTags,
  updateAgent,
  updateTag,
  fetchTeams,
} from "./persistance";
import { IAgentRequest, IRow, IScoringRule, ISelectedTag } from "./types";
import {
  getSelectedTagsFromLocalStorage,
  getTeamName,
  saveScoringRulesInLocalStorage,
  saveSelectedTagsInLocalStorage,
} from "./utils";
import { ITeam } from "types";

type TagsState = {
  isLoading: boolean;
  agents: Array<IRow>;
  scoringRules: Array<IScoringRule>;
  selectedTags: Array<ISelectedTag>;
  teams: Array<ITeam>;
};

const initialState: TagsState = {
  isLoading: true,
  agents: [],
  scoringRules: [],
  selectedTags: getSelectedTagsFromLocalStorage() ?? [],
  teams: [],
};

export const createScoringRule = createAsyncThunk(
  "tags/createScoringRule",
  async (scoringRule: IScoringRule, thunkAPI) => {
    try {
      return await createTag(scoringRule);
    } catch (e) {
      return thunkAPI.rejectWithValue(e);
    }
  }
);

export const updateScoringRule = createAsyncThunk(
  "tags/updateScoringRule",
  async (scoringRule: IScoringRule, thunkAPI) => {
    try {
      return await updateTag(scoringRule);
    } catch (e) {
      return thunkAPI.rejectWithValue(e);
    }
  }
);

export const deleteScoringRule = createAsyncThunk(
  "tags/deleteScoringRule",
  async (tagId: number, thunkAPI) => {
    try {
      await deleteTag(tagId);
      return tagId;
    } catch (e) {
      return thunkAPI.rejectWithValue(e);
    }
  }
);

export const updateSelectedTags = createAction("tags/updateSelectedTags");
export const deleteSelectedTag = createAction("tags/deleteSelectedTag");
export const deleteAllSelectedTags = createAction("tags/deleteAllSelectedTags");

export const getAllTags = createAsyncThunk("tags/getAllTags", () => fetchAllTags());
export const getAllAgents = createAsyncThunk("tags/getAllAgents", () => fetchAllAgents());
export const getTeams = createAsyncThunk("tags/getTeams", () => fetchTeams());

export const updateAgentAction = createAsyncThunk(
  "tags/updateAgent",
  async (agent: IAgentRequest, thunkAPI) => {
    try {
      return await updateAgent({
        ...agent,
        agentId: agent.id,
      });
    } catch (e) {
      console.log(e);
      return thunkAPI.rejectWithValue(e);
    }
  }
);

const slice = createSlice({
  name: "tags",
  initialState,
  reducers: {},
  extraReducers(builder) {
    builder
      .addCase(getAllAgents.pending, state => {
        state.isLoading = true;
      })
      .addCase(getAllAgents.fulfilled, (state, action) => {
        state.isLoading = false;
        const { payload } = action;

        if (state.teams.length) {
          state.agents = payload.map(agent => ({
            ...agent,
            teamName: getTeamName(state, agent),
          }));
        } else {
          state.agents = payload;
        }
      })
      .addCase(updateAgentAction.fulfilled, (state, action) => {
        const { payload } = action;
        const updatedAgent: IRow = {
          ...payload,
          id: payload.agentId,
          teamName: getTeamName(state, payload),
        };
        state.agents = state.agents.map((agent: IRow) =>
          agent.id === updatedAgent.id && agent.teamIds[0] === updatedAgent.teamIds[0]
            ? updatedAgent
            : agent
        );
      })
      .addCase(createScoringRule.fulfilled, (state, action) => {
        state.scoringRules.push(action.payload);

        saveScoringRulesInLocalStorage(state.scoringRules);
      })
      .addCase(deleteScoringRule.fulfilled, (state, action) => {
        state.scoringRules = state.scoringRules.filter(({ id }) => id !== action.payload);

        saveScoringRulesInLocalStorage(state.scoringRules);
      })
      .addCase(updateScoringRule.fulfilled, (state, action) => {
        const updatedScoringRule = action.payload;
        state.scoringRules = state.scoringRules.map(scoringRule => {
          if (scoringRule.id === updatedScoringRule.id) {
            return updatedScoringRule;
          }
          return scoringRule;
        });

        saveScoringRulesInLocalStorage(state.scoringRules);
      })
      .addCase(updateSelectedTags, (state, action) => {
        state.selectedTags = action.payload;
        saveSelectedTagsInLocalStorage(state.selectedTags);
      })
      .addCase(deleteSelectedTag, (state, action) => {
        state.selectedTags = state.selectedTags.filter(({ id }) => id !== action.payload);
        saveSelectedTagsInLocalStorage(state.selectedTags);
      })
      .addCase(deleteAllSelectedTags, state => {
        state.selectedTags = [];
        saveSelectedTagsInLocalStorage(state.selectedTags);
      })
      .addCase(getAllTags.pending, state => {
        state.isLoading = true;
      })
      .addCase(getAllTags.fulfilled, (state, action) => {
        state.isLoading = false;
        state.scoringRules = action.payload;
      })
      .addCase(getTeams.fulfilled, (state, action) => {
        state.teams = action.payload;
      });
  },
});

export default slice.reducer;
