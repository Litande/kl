import { useContext, useEffect, useState } from "react";
import { useNavigate } from "react-router";
import { useLocalStorage } from "hooks/useLocalStorage";
import { AuthContext, TOKEN_KEY, USER_KEY } from "data/user/AuthContext";
import AgentModel from "data/user/AgentModel";

import { ROLES, ROUTES } from "router/enums";
import { Agent } from "data/user/types";
import { ILoginForm } from "pages/authorization/Auth";
import authApi from "services/api/auth";
import { timerService } from "services/timerService/TimerService";

type UseAgentType = {
  agent: AgentModel;
  login: (props: ILoginForm) => Promise<void>;
  logout: () => void;
  authError: string | null;
};

export const useAgent = (): UseAgentType => {
  const { agent, setAgent } = useContext(AuthContext);
  const [authError, setAuthError] = useState<string>(null);

  const { setItem, clearAll } = useLocalStorage();
  const navigate = useNavigate();

  // TODO https://plat4me.atlassian.net/browse/kl-269
  const showError = (text: string) => {
    setAuthError(text);
    timerService().addTimeout(5000, () => {
      setAuthError("");
    });
  };

  const login = async ({ email, password }) => {
    let errorObj = null;
    await authApi
      .auth({ email, password })
      .then(response => {
        setItem(TOKEN_KEY, response.data);
      })
      .catch(error => {
        errorObj = error.response?.data;
      });

    if (errorObj) {
      showError(errorObj?.error || "error");
      return;
    }

    const { data } = await authApi.me();

    const agent: Agent = {
      email: "",
      id: data.userId,
      role: ROLES.AGENT,
      firstName: data.firstName,
      lastName: data.lastName,
      authToken: "authData.data",
    };

    setItem(USER_KEY, JSON.stringify(agent));

    setAgent(agent);
    navigate(`${ROUTES.DASHBOARD}`, { replace: true });
  };

  const logout = () => {
    agent.id !== -1 && agent.destroy();
    clearAll();
  };

  return { agent, login, logout, authError };
};
