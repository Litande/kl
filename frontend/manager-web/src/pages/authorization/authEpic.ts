import { Action } from "@reduxjs/toolkit";
import { StateObservable } from "redux-observable";
import { map, filter } from "rxjs/operators";

import { authActions } from "./authSlice";
import { RootState } from "store";
import { Observable } from "rxjs";
import { getRoleByLocation } from "router/routerChecker";

export const authEpic = (actions$: Observable<Action>, state$: StateObservable<RootState | void>) =>
  actions$.pipe(
    filter(authActions.loginStart.match),
    map(action =>
      authActions.loginCompleted({
        token: action.payload.token,
        role: getRoleByLocation(window.location.hostname),
      })
    )
  );
