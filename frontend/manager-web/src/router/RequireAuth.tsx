import { useLocation } from "react-router-dom";
import { ROLES, ROUTES } from "./enums";
import { useContext, useEffect } from "react";
import LayoutSkeleton from "../components/loader/LayoutSkeleton";
import GlobalLoader from "components/loader/GlobalLoader";
import { AuthContext } from "data/user/AuthContext";

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
  const { user, isAuthProgress, checkAuth } = useContext(AuthContext);
  const location = useLocation();

  useEffect(() => {
    checkAuth();
    // eslint-disable-next-line react-hooks/exhaustive-deps
  }, []);

  if (isAuthProgress) {
    return (
      <LayoutSkeleton>
        <GlobalLoader />
      </LayoutSkeleton>
    );
  }
  // hide any protected pages if user is not logged in
  if (!user && isProtected) {
    return;
  }
  if (user && location.pathname === `/${ROUTES.AUTH}`) {
    return;
  }

  return children;
}
