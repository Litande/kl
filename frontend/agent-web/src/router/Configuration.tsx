/* eslint-disable @typescript-eslint/no-explicit-any */

import React, { ReactNode } from "react";
import { ROLES, ROUTES } from "./enums";
import AuthPage from "pages/authorization/Auth";
import Board from "pages/board/Board";
import Dialing from "pages/dialing/Dialing";
import FutureCallbacks from "pages/futureCallback/FutureCallbacks";
import History from "pages/history/History";
import ManualCallPage from "pages/manualCall/ManualCallPage";
import PageTitle from "components/layout/PageTitle";
import { PageWrapContent } from "components/layout/AgentLayout";

export interface IRoute {
  path: string;
  exact?: boolean;
  fallback: NonNullable<ReactNode> | null;
  element: JSX.Element;
  routes?: IRoute[];
  redirect?: string;
  isProtected?: boolean;
  layout?: boolean;
  index?: boolean;
  allowedRoles?: ROLES[];
}

export const agentRoute: IRoute[] = [
  {
    path: ROUTES.AUTH,
    element: <AuthPage />,
    isProtected: false,
    fallback: null,
  },
  {
    path: ROUTES.DASHBOARD,
    element: <Board />,
    redirect: ROUTES.AUTH,
    isProtected: true,
    allowedRoles: [ROLES.AGENT],
    fallback: null,
    layout: true,
  },
  {
    path: ROUTES.DIALING,
    element: <Dialing />,
    redirect: ROUTES.AUTH,
    isProtected: true,
    allowedRoles: [ROLES.AGENT],
    fallback: null,
    layout: true,
  },
  {
    path: ROUTES.FUTURE_CALLBACK,
    element: <FutureCallbacks />,
    redirect: ROUTES.AUTH,
    isProtected: true,
    allowedRoles: [ROLES.AGENT],
    fallback: null,
    layout: true,
  },
  {
    path: ROUTES.MANUAL_CALL,
    element: <ManualCallPage />,
    redirect: ROUTES.AUTH,
    isProtected: true,
    allowedRoles: [ROLES.AGENT],
    fallback: null,
    layout: true,
  },
  {
    path: ROUTES.HISTORY,
    element: <History />,
    redirect: ROUTES.AUTH,
    isProtected: true,
    allowedRoles: [ROLES.AGENT],
    fallback: null,
    layout: true,
  },
  {
    path: ROUTES.NOT_FOUND,
    element: (
      <PageWrapContent>
        <PageTitle label={"Not Found"} />
      </PageWrapContent>
    ),
    redirect: ROUTES.AUTH,
    isProtected: true,
    allowedRoles: [ROLES.AGENT],
    fallback: null,
    layout: true,
  },
];

export default agentRoute;
