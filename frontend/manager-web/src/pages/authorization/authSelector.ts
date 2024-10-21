import { createSelector } from "@reduxjs/toolkit";
import { RootState } from "store";

export const authSelector = createSelector(
  (state: RootState) => state.auth,
  ({ role, token, loading }) => {
    const isAuth = Boolean(role && token && !loading);
    return isAuth
      ? { role, token, loading }
      : JSON.parse(window.localStorage.getItem("auth")) ?? null;
  }
);

export const userSelector = createSelector(
  (state: RootState) => state.auth,
  ({ user }) => user
);

export const authErrorSelector = createSelector(
  (state: RootState) => state.auth,
  ({ error }) => {
    return error;
  }
);
