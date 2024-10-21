import { ROLES } from "router/enums";
import BaseUserModel from "./BaseUserModel";

export type User = {
  id: number;
  firstName: string;
  lastName: string;
  email: string;
  role: ROLES | null;
  authToken?: string;
  iceServices?: string[];
};

export interface ILoginForm {
  email: string;
  password: string;
  rememberMe?: boolean;
}

export type BaseAuthContext = {
  user: BaseUserModel | null;
  isAuthProgress: boolean;
  checkAuth: () => void;
  login: (data: ILoginForm) => Promise<void>;
  logout: () => void;
};
