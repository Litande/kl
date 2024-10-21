import { createSelector, createSlice } from "@reduxjs/toolkit";

import { RootState } from "store";

const isMenuCollapsed = JSON.parse(localStorage.getItem("menuState")) || false;

const navigationSlice = createSlice({
  name: "navigation",
  initialState: {
    isCollapsed: isMenuCollapsed,
  },
  reducers: {
    toggleMenu(state) {
      localStorage.setItem("menuState", JSON.stringify(!state.isCollapsed));
      state.isCollapsed = !state.isCollapsed;
    },
  },
});

export const navigationStateSelector = createSelector(
  (state: RootState) => state.navigation,
  navState => navState
);

export const navActions = navigationSlice.actions;

export default navigationSlice.reducer;
