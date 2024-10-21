import { CaseReducer, createSlice, PayloadAction } from "@reduxjs/toolkit";
import { ROLES } from "router/enums";

type User = {
  firstName: string;
  lastName: string;
  id: number;
};

type AuthState = {
  token: string;
  loading: boolean;
  role: ROLES;
  error: null | string;
  user: User;
};

const initialState: () => AuthState = () => {
  const { token, role } = JSON.parse(window.localStorage.getItem("auth")) || {
    token: "",
    role: "",
  };

  return {
    token,
    loading: false,
    role,
    error: null,
    user: {
      firstName: "",
      lastName: "",
      id: null,
    },
  };
};

const loginStart: CaseReducer<
  AuthState,
  PayloadAction<{
    token: string;
  }>
> = (state, action) => ({
  ...state,
  loading: true,
  token: "",
  role: null,
});

const loginCompleted: CaseReducer<AuthState, PayloadAction<{ token: string; role: ROLES }>> = (
  state,
  action
) => ({
  error: null,
  loading: false,
  user: { firstName: "", lastName: "", id: null },
  token: action.payload.token,
  role: action.payload.role,
});

const loginError: CaseReducer<AuthState, PayloadAction<string>> = (state, action) => ({
  ...state,
  error: action.payload,
});

const getUserStart: CaseReducer<AuthState, PayloadAction> = (state, action) => ({
  ...state,
});
const getUserCompleted: CaseReducer<AuthState, PayloadAction<User>> = (state, action) => ({
  ...state,
  user: action.payload,
});

const authSlice = createSlice({
  name: "auth",
  initialState,
  reducers: {
    loginStart,
    loginCompleted,
    loginError,
    getUserStart,
    getUserCompleted,
  },
});

export const authActions = authSlice.actions;

export default authSlice.reducer;
