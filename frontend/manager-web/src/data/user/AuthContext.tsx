import React, { createContext, useEffect } from "react";
import { useLocalStorage } from "hooks/useLocalStorage";
import { BaseAuthContext, ILoginForm } from "data/user/types";
import { useNavigate } from "react-router";
import { ROUTES } from "router/enums";
import { useLocation } from "react-router-dom";
import BaseUserModel from "data/user/BaseUserModel";
import authApi from "services/api/auth";
import apiService from "services/api/apiService";
import { removeLocalStorageItem } from "utils/localStorage";
import EventDomDispatcher from "services/events/EventDomDispatcher";
import { LOGOUT_EVENT } from "services/events/consts";

export const USER_KEY = "user";
export const TOKEN_KEY = "token";

type ComponentProps = {
  children: JSX.Element;
};
const AuthProvider = ({ children }: ComponentProps) => {
  const [user, setUser] = React.useState<BaseUserModel>(null);
  const [isAuthProgress, setIsAuthProgress] = React.useState(true);
  const { getItem } = useLocalStorage();
  const navigate = useNavigate();
  const location = useLocation();

  const checkAuth = async () => {
    const token = getItem(TOKEN_KEY);
    if (!token) {
      gotoLoginPage();
      setIsAuthProgress(false);
    } else {
      getMe();
    }
  };
  const getMe = async () => {
    setIsAuthProgress(true);
    apiService().cancelRequests();

    try {
      const response = await authApi.me();
      if (response !== undefined) {
        setUser(new BaseUserModel(response.data));
        setIsAuthProgress(false);
      }
    } catch (error) {
      user?.destroy();
      setUser(null);
      setIsAuthProgress(false);
      removeLocalStorageItem(TOKEN_KEY);
      return;
    }
  };

  const login = async (data: ILoginForm) => {
    setIsAuthProgress(true);
    apiService().cancelRequests();
    try {
      const response = await authApi.auth(data);
      const token = response.data;
      window.localStorage.setItem(TOKEN_KEY, token);
      getMe();
    } catch (error) {
      setIsAuthProgress(false);
      return;
    }
  };

  const logout = () => {
    user?.destroy();
    removeLocalStorageItem(TOKEN_KEY);
    setUser(null);
    gotoLoginPage();
  };

  const gotoDashboard = () => {
    if (location.pathname === `/${ROUTES.AUTH}`) {
      navigate(`${ROUTES.DASHBOARD}`, { replace: true });
    }
  };

  const gotoLoginPage = () => {
    if (location.pathname !== `/${ROUTES.AUTH}`) {
      navigate(`/${ROUTES.AUTH}`, { replace: true });
    }
  };

  useEffect(() => {
    const { addEventListener, removeEventListener } = EventDomDispatcher();
    addEventListener(LOGOUT_EVENT, logout);
    return () => {
      removeEventListener(LOGOUT_EVENT, logout);
    };
    // eslint-disable-next-line react-hooks/exhaustive-deps
  }, []);

  useEffect(() => {
    if (isAuthProgress) return;
    if (user) {
      gotoDashboard();
    } else {
      gotoLoginPage();
    }
  }, [user, isAuthProgress]);

  return (
    <AuthContext.Provider value={{ user, isAuthProgress, checkAuth, login, logout }}>
      {children}
    </AuthContext.Provider>
  );
};

export const AuthContext = createContext<BaseAuthContext | null>(null);

export default AuthProvider;
