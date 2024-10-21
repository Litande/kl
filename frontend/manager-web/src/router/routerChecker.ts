import { ROLES } from "router/enums";
import routes, { agentRoute } from "router/Configuration";

export const getRole = () => {
  return getRoleByLocation(window.location.hostname);
};

export const getRoleByLocation = hostname => {
  const arr = hostname.split(".");
  let role = ROLES.MANAGER;
  if (arr.length > 1) {
    const testSubDomain = value => {
      const appName = value.toLowerCase();
      if (appName === ROLES.AGENT.toLowerCase()) {
        return ROLES.AGENT;
      }
      if (appName === ROLES.MANAGER.toLowerCase()) {
        return ROLES.MANAGER;
      }
      return null;
    };

    role = testSubDomain(arr[0]) || testSubDomain(arr[1]);
  }
  return role;
};

export const getRoutes = hostname => {
  if (getRoleByLocation(hostname) === ROLES.AGENT) {
    return agentRoute;
  }
  return routes;
};
