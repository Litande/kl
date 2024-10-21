import React, { Fragment, Suspense } from "react";
import { Route, Routes } from "react-router-dom";
import { IRoute } from "router/Configuration";
import RequireAuth from "router/RequireAuth";
import { getRole } from "router/routerChecker";
import { ROLES } from "router/enums";
import Layout from "components/layout/Layout";
import AgentLayout from "agent/components/layout/AgentLayout";

interface IProps {
  routes: IRoute[];
}

const Router: React.FC<IProps> = ({ routes }) => {
  const getLayout = route => {
    return route.layout ? (getRole() === ROLES.AGENT ? AgentLayout : Layout) : Fragment;
  };

  return (
    <Routes>
      {routes.map((route, index) => {
        const ContentLayout = getLayout(route);
        return (
          <Route
            path={route.path}
            key={index}
            element={
              <RequireAuth
                isProtected={route.isProtected}
                redirect={route.redirect}
                allowedRoles={route.allowedRoles}
              >
                <Suspense fallback={route.fallback}>
                  <ContentLayout>
                    <route.element />
                  </ContentLayout>
                </Suspense>
              </RequireAuth>
            }
          >
            {route.routes?.map(nestedRoute => (
              <Route
                key={nestedRoute.path}
                path={nestedRoute.path}
                element={<nestedRoute.element />}
              />
            ))}
          </Route>
        );
      })}
    </Routes>
  );
};

export default Router;
