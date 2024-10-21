import { createAction, createAsyncThunk, createSlice } from "@reduxjs/toolkit";
import callsApi from "services/api/calls";
import { appliedFiltersSelector, paginationSelector } from "./selector";

type CallListPayload = {
  items: [];
  page: number;
  pageSize: number;
  totalCount: number | undefined;
};

type CallRecordingsState = {
  isLoading: boolean;
  callsList: CallListPayload;
  audioCall: Blob;
  // eslint-disable-next-line @typescript-eslint/no-explicit-any
  appliedFilters: any;
};

const initialState: CallRecordingsState = {
  isLoading: true,
  callsList: {
    items: null,
    page: 1,
    pageSize: 20,
    totalCount: null,
  },
  audioCall: null,
  appliedFilters: {},
};

export const getCallsAction = createAsyncThunk(
  "tags/getCalls",
  async (searchParams, { getState }) => {
    try {
      const appliedFilters = appliedFiltersSelector(getState());
      const pagination = paginationSelector(getState());

      const { data } = await callsApi.getCalls(appliedFilters, pagination);

      return data;
    } catch (e) {
      console.error(e);
    }
  }
);

export const applyFiltersAction = createAction("applyFilters");
export const applyPaginationAction = createAction("applyPagination");

const slice = createSlice({
  name: "callRecordings",
  initialState,
  reducers: {},
  extraReducers(builder) {
    builder
      .addCase(applyFiltersAction, (state, action) => {
        state.appliedFilters = action.payload;
      })
      .addCase(applyPaginationAction, (state, action) => {
        state.callsList.page = action.payload || 1;
      })
      .addCase(getCallsAction.pending, state => {
        state.isLoading = true;
      })
      .addCase(getCallsAction.fulfilled, (state, action) => {
        state.isLoading = false;
        state.callsList = action.payload;
      });
  },
});

export default slice.reducer;
