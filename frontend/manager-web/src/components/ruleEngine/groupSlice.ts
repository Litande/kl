import { createAsyncThunk, createSlice } from "@reduxjs/toolkit";

import { ChangedRuleData, IRulesGroup, RuleData, RuleTypes } from "components/ruleEngine/types";
import { normalizeRules } from "components/ruleEngine/utils";
import apiService from "services/api/apiService";

const { fetchApi, putApi, postApi, deleteApi } = apiService();

type GroupsState = {
  loading: boolean;
  groups: Array<IRulesGroup>;
};

// eslint-disable-next-line @typescript-eslint/no-explicit-any
type ActionType = any;

const initialState: GroupsState = {
  loading: false,
  groups: [],
};

export const loadGroups = createAsyncThunk(
  "ruleEngine/Groups/loadGroups",
  async (ruleType: RuleTypes, thunkAPI) => {
    try {
      const { data } = await fetchApi(`/rules/${ruleType}/groups`);
      return normalizeRules(data);
    } catch (e) {
      return thunkAPI.rejectWithValue(e);
    }
  }
);

export const updateGroup = createAsyncThunk<string, ChangedRuleData, { state: GroupsState }>(
  "ruleEngine/Groups/updateGroup",
  async (ruleGroup, thunkAPI) => {
    try {
      const { data } = await putApi(
        `/rules/${ruleGroup.ruleType}/groups/${ruleGroup.groupId}`,
        ruleGroup.data
      );
      return data;
    } catch (e) {
      return thunkAPI.rejectWithValue(e);
    }
  }
);

export const removeGroup = createAsyncThunk<string, { ruleType; groupId }, { state: GroupsState }>(
  "ruleEngine/Groups/removeGroup",
  async ({ ruleType, groupId }, thunkAPI) => {
    try {
      const { data } = await deleteApi(`/rules/${ruleType}/groups/${groupId}`, null);
      return data;
    } catch (e) {
      return thunkAPI.rejectWithValue(e);
    }
  }
);

export const createGroup = createAsyncThunk<string, RuleData, { state: GroupsState }>(
  "ruleEngine/Groups/createGroup",
  async (ruleGroup, thunkAPI) => {
    try {
      const { data } = await postApi(`/rules/${ruleGroup.ruleType}/groups`, ruleGroup.data);
      return data;
    } catch (e) {
      return thunkAPI.rejectWithValue(e);
    }
  }
);

const ruleEngineGroupsSlice = createSlice({
  name: "ruleEngine/Groups",
  initialState,
  reducers: {},
  extraReducers(builder) {
    builder
      .addCase(loadGroups.pending, state => {
        state.loading = true;
      })
      .addCase(loadGroups.fulfilled, (state, action) => {
        state.loading = false;
        state.groups = action.payload;
      })
      .addCase(updateGroup.fulfilled, (state, action: ActionType) => {
        const oldGroup = state.groups.find(gr => gr.groupId === action.payload.id);
        oldGroup.groupName = action.payload.name;
        oldGroup.status = action.payload.status;
      })
      .addCase(removeGroup.fulfilled, (state, action: ActionType) => {
        state.groups = state.groups.filter(group => group.groupId !== action.meta.arg.groupId);
      })
      .addCase(createGroup.fulfilled, (state, action: ActionType) => {
        const newGroup = {
          groupName: action.payload.name,
          status: action.payload.status,
          groupId: action.payload.id,
          rules: action.payload.rules,
        };
        state.groups = [...state.groups, newGroup];
      });
  },
});

export const ruleEngineGroupsActions = ruleEngineGroupsSlice.actions;

export default ruleEngineGroupsSlice.reducer;
