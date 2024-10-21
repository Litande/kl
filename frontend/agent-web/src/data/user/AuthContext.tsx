import React, { createContext, useEffect, useMemo, useState } from "react";
import { useLocalStorage } from "hooks/useLocalStorage";
import { BaseAuthContext } from "data/user/types";
import AgentModel from "data/user/AgentModel";
import { useNavigate } from "react-router";
import { ROUTES } from "router/enums";
import { useLocation } from "react-router-dom";

export const USER_KEY = "user";
export const TOKEN_KEY = "token";

type ComponentProps = {
  children: JSX.Element;
};
const mainAgent: AgentModel = new AgentModel();
const AuthProvider = ({ children }: ComponentProps) => {
  const { getItem } = useLocalStorage();
  const navigate = useNavigate();
  const location = useLocation();

  const setAgent = value => {
    if (mainAgent.id !== value?.id) {
      mainAgent.init(value);
      if (location.pathname === `/${ROUTES.AUTH}`) {
        navigate(`${ROUTES.DASHBOARD}`, { replace: true });
      }
    }
  };

  const agent = useMemo(() => {
    return mainAgent;
  }, []);

  return <AuthContext.Provider value={{ agent, setAgent }}>{children}</AuthContext.Provider>;
};

export const AuthContext = createContext<BaseAuthContext | null>(null);

export default AuthProvider;
