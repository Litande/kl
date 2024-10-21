import { createAsyncThunk, createSlice } from "@reduxjs/toolkit";

import { RulesState, RuleTypes } from "components/ruleEngine/types";
import apiService from "services/api/apiService";
import { loadGroups } from "components/ruleEngine/groupSlice";

const initialState: RulesState = {
  conditions: null,
  actions: null,
  fetchedRuleData: null,
  ruleData: null,
  loading: false,
};

const { fetchApi, putApi, postApi, deleteApi } = apiService();

export const loadConditions = createAsyncThunk(
  "ruleEngine/rules/loadConditions",
  async (ruleType: RuleTypes, thunkAPI) => {
    try {
      const { data } = await fetchApi(`/rules/${ruleType}/conditions`);
      return data;
    } catch (e) {
      return thunkAPI.rejectWithValue(e);
    }
  }
);

export const loadActions = createAsyncThunk(
  "ruleEngine/rules/loadActions",
  async (ruleType: RuleTypes, thunkAPI) => {
    try {
      const { data } = await fetchApi(`/rules/${ruleType}/actions`);
      return data;
    } catch (e) {
      return thunkAPI.rejectWithValue(e);
    }
  }
);

export const createRule = createAsyncThunk(
  "ruleEngine/rules/createRule",
  async ({ ruleType, groupId, data }: { ruleType: RuleTypes; groupId: string; data }, thunkAPI) => {
    try {
      const result = await postApi(`/rules/${ruleType}/groups/${groupId}/rules`, data);
      thunkAPI.dispatch(loadGroups(ruleType));
      return { ...result.data, rules: JSON.parse(result.data.rules) };
    } catch (error) {
      return thunkAPI.rejectWithValue(error);
    }
  }
);

export const updateRule = createAsyncThunk(
  "ruleEngine/rules/updateRule",
  async (
    {
      ruleType,
      urlParams: { groupId, ruleId },
      data,
    }: {
      ruleType: RuleTypes;
      urlParams: { groupId; ruleId };
      data;
    },
    thunkAPI
  ) => {
    try {
      const result = await putApi(`/rules/${ruleType}/groups/${groupId}/rules/${ruleId}`, data);
      thunkAPI.dispatch(loadGroups(ruleType));
      return { ...result.data, rules: JSON.parse(result.data.rules) };
    } catch (error) {
      return thunkAPI.rejectWithValue(error);
    }
  }
);

export const removeRule = createAsyncThunk(
  "ruleEngine/rules/removeRule",
  async ({ ruleType, groupId, ruleId }: { ruleType: RuleTypes; groupId; ruleId }, thunkAPI) => {
    try {
      const result = await deleteApi(`/rules/${ruleType}/groups/${groupId}/rules/${ruleId}`, null);
      thunkAPI.dispatch(loadGroups(ruleType));
      return result.data;
    } catch (error) {
      return thunkAPI.rejectWithValue(error);
    }
  }
);

export const getRuleDetails = createAsyncThunk(
  "ruleEngine/rules/getRuleDetails",
  async ({ ruleType, groupId, ruleId }: { ruleType; groupId; ruleId }, thunkAPI) => {
    try {
      const [ruleDetails, conditionOptions, actionOptions] = await Promise.all([
        fetchApi(`/rules/${ruleType}/groups/${groupId}/rules/${ruleId}`),
        fetchApi(`/rules/${ruleType}/conditions`),
        fetchApi(`/rules/${ruleType}/actions`),
      ]);
      return [ruleDetails.data, conditionOptions.data, actionOptions.data];
    } catch (error) {
      return thunkAPI.rejectWithValue(error);
    }
  }
);

const ruleConditionsActions = createSlice({
  name: "ruleEngine/rules",
  initialState,
  reducers: {},
  extraReducers(builder) {
    builder
      .addCase(loadConditions.fulfilled, (state, action) => {
        state.conditions = action.payload;
      })
      .addCase(loadActions.fulfilled, (state, action) => {
        state.actions = action.payload;
      })
      .addCase(createRule.fulfilled, (state, action) => {
        state.ruleData = action.payload;
      })
      .addCase(updateRule.fulfilled, (state, action) => {
        state.ruleData = action.payload;
      })
      .addCase(getRuleDetails.pending, state => {
        state.loading = true;
        state.fetchedRuleData = null;
      })
      .addCase(getRuleDetails.fulfilled, (state, action) => {
        const [ruleDetails, conditions, actions] = action.payload;
        state.loading = false;
        state.fetchedRuleData = ruleDetails;
        state.actions = actions;
        state.conditions = conditions;
      });
  },
});

export const mainRulesGroupActions = ruleConditionsActions.actions;

export default ruleConditionsActions.reducer;
