import { createAction, createAsyncThunk, createSlice, PayloadAction } from "@reduxjs/toolkit";
import trackingApi from "services/api/tracking";
import { AgentStatusStr, agentStatusToLabel, IAgent } from "types";
import { IOption, ITag, ITeamShort } from "./types";
import { AxiosResponse } from "axios";

export type AgentsState = {
  isLoading: boolean;
  list: IAgent[];
  appliedFilters: {
    users: IOption[];
    tags: IOption[];
    states: IOption[];
    groups: IOption[];
  };
  filterOptions: {
    users: IOption[];
    tags: IOption[];
    states: IOption[];
    groups: IOption[];
  };
};

const initialState: AgentsState = {
  isLoading: false,
  list: [],
  appliedFilters: {
    users: [],
    tags: [],
    states: [],
    groups: [],
  },
  filterOptions: {
    users: [],
    tags: [],
    states: [
      {
        label: agentStatusToLabel[AgentStatusStr.InTheCall],
        value: AgentStatusStr.InTheCall,
      },
      {
        label: agentStatusToLabel[AgentStatusStr.FillingFeedback],
        value: AgentStatusStr.FillingFeedback,
      },
      {
        label: agentStatusToLabel[AgentStatusStr.WaitingForTheCall],
        value: AgentStatusStr.WaitingForTheCall,
      },
      {
        label: agentStatusToLabel[AgentStatusStr.Break],
        value: AgentStatusStr.Break,
      },
      {
        label: agentStatusToLabel[AgentStatusStr.Offline],
        value: AgentStatusStr.Offline,
      },
    ],
    groups: [],
  },
};

export const loadAgents = createAsyncThunk("agentsList/loadGroups", async () => {
  try {
    const { data } = await trackingApi.getAgents();

    return data.items as IAgent[];
  } catch (e) {
    console.error(e);
  }
});

export const loadTags = createAsyncThunk("agentsList/loadTags", async () => {
  try {
    const { data }: AxiosResponse<{ items: ITag[] }> = await trackingApi.getTags();
    return data.items.map(({ id, name }) => ({
      label: name,
      value: id,
    }));
  } catch (e) {
    console.error(e);
  }
});

export const loadTeams = createAsyncThunk("agentsList/loadTeams", async () => {
  try {
    const { data }: AxiosResponse<{ items: ITeamShort[] }> = await trackingApi.getTeams();

    return data.items.map(({ teamId, name }) => ({
      label: name,
      value: teamId,
    }));
  } catch (e) {
    console.error(e);
  }
});

export const applyFiltersAction = createAction("agentsList/applyFilters");

export const updateAgentsAction = createAction("agentsList/updateAgents");

const slice = createSlice({
  name: "agents/list",
  initialState,
  reducers: {},
  extraReducers(builder) {
    builder
      .addCase(applyFiltersAction, (state, action) => {
        state.appliedFilters = action.payload;
      })
      .addCase(
        updateAgentsAction,
        (state, action: PayloadAction<{ data: IAgent[]; agents: IAgent[] }>) => {
          const { data, agents } = action.payload;

          data.forEach(item => {
            const index = agents.findIndex(i => i.id === item.id);

            state.list[index] = {
              ...state.list[index],
              state: item.state,
              tags: item.tags,
              teamIds: item.teamIds,
              managerRtcUrl: item.managerRtcUrl,
            };
          });
        }
      )
      .addCase(loadTeams.fulfilled, (state, action) => {
        state.filterOptions.groups = action.payload;
      })
      .addCase(loadTags.fulfilled, (state, action) => {
        state.filterOptions.tags = action.payload;
      })
      .addCase(loadAgents.pending, state => {
        state.isLoading = true;
      })
      .addCase(loadAgents.fulfilled, (state, action) => {
        state.isLoading = false;
        state.list = action.payload;
        state.filterOptions.users = action.payload.map(item => ({
          label: item.name,
          value: item.id,
        }));
      });
  },
});

export default slice.reducer;
