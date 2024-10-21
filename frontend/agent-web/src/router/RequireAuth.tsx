import { useContext, useEffect, useState } from "react";
import { Navigate, useLocation } from "react-router-dom";
import { ROLES, ROUTES } from "./enums";
import { AuthContext, TOKEN_KEY } from "data/user/AuthContext";
import authApi from "services/api/auth";
import { useNavigate } from "react-router";
import { useLocalStorage } from "hooks/useLocalStorage";
import GlobalLoader from "components/loader/GlobalLoader";
import DashboardSkeleton from "components/loader/DashboardSkeleton";
import { AgentStatus } from "data/user/types";

export default function RequireAuth({
  children,
  isProtected,
  redirect,
  allowedRoles = [],
}: {
  children: JSX.Element;
  isProtected?: boolean;
  redirect?: string;
  allowedRoles: ROLES[];
}) {
  const navigate = useNavigate();
  const { getItem } = useLocalStorage();
  const [isReady, setIsReady] = useState(false);
  const { agent, setAgent } = useContext(AuthContext);

  const gotoAuthPage = () => {
    navigate(`/${ROUTES.AUTH}`, { replace: true });
  };

  useEffect(() => {
    const token = getItem(TOKEN_KEY);
    if (!token) {
      localStorage.clear();
      setIsReady(true);
      gotoAuthPage();
      return;
    }
    authApi
      .me()
      .then(response => {
        setAgent(response.data);
      })
      .catch(error => {
        if (error?.response?.status === 401) {
          gotoAuthPage();
        }
      })
      .finally(() => {
        setIsReady(true);
      });
  }, []);

  const [isAuth, setIsAuth] = useState(agent.isAuth.getValue());
  useEffect(() => {
    const subscription = agent.isAuth.subscribe(value => {
      if (isAuth === true && value === false) {
        gotoAuthPage();
      }
      if (isAuth !== value) {
        setIsAuth(value);
      }
    });
    return () => {
      subscription.unsubscribe();
    };
  }, [isAuth]);

  const [agentStatus, setAgentStatus] = useState(agent.status.getValue());
  useEffect(() => {
    const subscribe = agent.status.subscribe(value => {
      setAgentStatus(value);
    });
    return () => {
      subscribe && subscribe.unsubscribe();
    };
  }, [agent]);

  const location = useLocation();
  const isAuthenticated = Boolean(agent);
  const hasPermission = isAuthenticated;
  if (!isReady || (agentStatus === AgentStatus.Unknown && agent.id !== -1)) {
    return (
      <DashboardSkeleton>
        <GlobalLoader isShowRefresh={true} />
      </DashboardSkeleton>
    );
  }

  if (location.pathname === `/${ROUTES.AUTH}` && agent.id !== -1) {
    return <Navigate to={ROUTES.DASHBOARD} state={{ from: location }} replace />;
  }
  if (!hasPermission && redirect) {
    return <Navigate to={redirect} state={{ from: location }} replace />;
  }
  //
  if (!isAuthenticated && isProtected) {
    return <Navigate to={ROUTES.AUTH} state={{ from: location }} replace />;
  }
  return children;
}
