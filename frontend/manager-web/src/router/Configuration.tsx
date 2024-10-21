/* eslint-disable @typescript-eslint/no-explicit-any */

import { ComponentType, lazy, LazyExoticComponent, ReactNode } from "react";
import { Navigate, Outlet } from "react-router-dom";
import { ROLES, RULES_NESTED_ROUTES, ROUTES } from "./enums";

import ApiRules from "pages/rules/outlets/ApIRules";
import NewLeads from "pages/rules/outlets/NewLeadRules";
import MainRules from "pages/rules/outlets/MainRules";
import LeadScoringRules from "pages/rules/outlets/LeadScoringRules";
import AgentsScoringRules from "pages/rules/outlets/agentsScoringRules/AgentsScoringRules";
import StatusRules from "pages/rules/outlets/StatusRules";
import AuthPage from "pages/authorization/Auth";
import Board from "agent/pages/board/Board";
import AgentLayout from "agent/components/layout/AgentLayout";
import Dashboard from "pages/dashboard/Dashboard";
import Leads from "pages/leads/Leads";
import LeadsSearch from "pages/leadsSearch/LeadsSearch";
import Tracking from "pages/tracking/Tracking";
import Rules from "pages/rules/RulesPage";
import Agents from "pages/agents/Agents";
import CallsPage from "pages/calls/CallsPage";
import TagsPage from "pages/tags/TagsPage";

export interface IRoute {
  path: string;
  exact?: boolean;
  fallback?: NonNullable<ReactNode> | null;
  element: LazyExoticComponent<ComponentType<any>> | (() => JSX.Element);
  routes?: IRoute[];
  redirect?: string;
  isProtected?: boolean;
  layout?: boolean;
  index?: boolean;
  layoutElement?: LazyExoticComponent<ComponentType<any>> | (({ children }) => JSX.Element);
  allowedRoles?: ROLES[];
  exclude?: boolean;
}

export const rulesOutlets = [
  // navigate to default NEW_LEADS rule
  {
    path: "",
    index: true,
    element: () => <Navigate to={RULES_NESTED_ROUTES.NEW_LEADS} />,
    exclude: true,
  },
  {
    path: RULES_NESTED_ROUTES.NEW_LEADS,
    element: NewLeads,
    fallback: null,
  },
  {
    path: RULES_NESTED_ROUTES.MAIN,
    fallback: null,
    element: MainRules,
  },
  {
    path: RULES_NESTED_ROUTES.LEAD_SCORING,
    element: LeadScoringRules,
    fallback: null,
  },
  {
    path: RULES_NESTED_ROUTES.AGENT_SCORING,
    element: AgentsScoringRules,
    fallback: null,
  },
  {
    path: RULES_NESTED_ROUTES.STATUS,
    element: StatusRules,
    fallback: null,
  },
  {
    path: RULES_NESTED_ROUTES.API_RULES,
    element: ApiRules,
    fallback: null,
  },
];

export const agentRoute: IRoute[] = [
  {
    path: ROUTES.AUTH,
    element: AuthPage,
    isProtected: false,
    fallback: null,
  },
  {
    path: ROUTES.DASHBOARD,
    element: Board,
    redirect: ROUTES.AUTH,
    isProtected: true,
    allowedRoles: [ROLES.AGENT],
    fallback: null,
    layout: true,
    layoutElement: AgentLayout,
  },
  {
    path: ROUTES.NOT_FOUND,
    element: () => <h1>NOT FOUND</h1>,
    redirect: ROUTES.AUTH,
    isProtected: true,
    allowedRoles: [ROLES.MANAGER, ROLES.AGENT, ROLES.TEST],
    fallback: null,
    layout: true,
    layoutElement: AgentLayout,
  },
];

const routes: IRoute[] = [
  {
    path: ROUTES.AUTH,
    element: AuthPage,
    isProtected: false,
    fallback: null,
  },
  {
    path: ROUTES.DASHBOARD,
    element: Dashboard,
    redirect: ROUTES.AUTH,
    isProtected: true,
    allowedRoles: [ROLES.MANAGER],
    fallback: null,
    layout: true,
  },
  {
    path: ROUTES.SETTINGS,
    element: lazy(() => import("pages/settings/Settings")),
    redirect: ROUTES.AUTH,
    isProtected: true,
    allowedRoles: [ROLES.MANAGER],
    fallback: null,
    layout: true,
  },
  {
    path: ROUTES.LEADS,
    element: () => <Outlet />,
    redirect: ROUTES.AUTH,
    isProtected: true,
    allowedRoles: [ROLES.MANAGER],
    fallback: null,
    layout: true,
    routes: [
      {
        index: true,
        path: "",
        element: Leads,
        isProtected: true,
        fallback: null,
      },
      {
        path: ROUTES.LEADS_SEARCH,
        element: LeadsSearch,
        isProtected: true,
        fallback: null,
      },
    ],
  },
  {
    path: ROUTES.TRACKING,
    element: Tracking,
    redirect: ROUTES.AUTH,
    isProtected: true,
    allowedRoles: [ROLES.MANAGER],
    fallback: null,
    layout: true,
  },
  {
    path: ROUTES.RULES,
    element: Rules,
    redirect: ROUTES.AUTH,
    isProtected: true,
    allowedRoles: [ROLES.MANAGER],
    fallback: null,
    layout: true,
    routes: rulesOutlets,
  },

  {
    path: ROUTES.AGENTS,
    element: Agents,
    redirect: ROUTES.AUTH,
    isProtected: true,
    allowedRoles: [ROLES.MANAGER],
    fallback: null,
    layout: true,
  },
  {
    path: ROUTES.CALL_RECORDINGS,
    element: CallsPage,
    redirect: ROUTES.AUTH,
    isProtected: true,
    allowedRoles: [ROLES.MANAGER],
    fallback: null,
    layout: true,
  },
  {
    path: ROUTES.TAGS,
    element: TagsPage,
    redirect: ROUTES.AUTH,
    isProtected: true,
    allowedRoles: [ROLES.MANAGER],
    fallback: null,
    layout: true,
  },
  {
    path: ROUTES.NOT_FOUND,
    element: () => <h1>NOT FOUND</h1>,
    redirect: ROUTES.AUTH,
    isProtected: true,
    allowedRoles: [ROLES.MANAGER, ROLES.AGENT, ROLES.TEST],
    fallback: null,
    layout: true,
  },
];

export default routes;
