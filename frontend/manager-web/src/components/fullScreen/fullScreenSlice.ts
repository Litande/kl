import { createSelector, createSlice } from "@reduxjs/toolkit";

import { RootState } from "store";

const isFullScreen = JSON.parse(localStorage.getItem("fullScreen")) || false;

const fullScreenSlice = createSlice({
  name: "fullScreen",
  initialState: {
    isFullScreen: isFullScreen,
  },
  reducers: {
    toggleScreenState(state) {
      localStorage.setItem("fullScreen", JSON.stringify(!state.isFullScreen));
      state.isFullScreen = !state.isFullScreen;
    },
  },
});

export const fullScreenStateSelector = createSelector(
  (state: RootState) => state.fullScreen,
  screenState => screenState
);

export const fullScreenActions = fullScreenSlice.actions;

export default fullScreenSlice.reducer;
