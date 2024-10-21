import { ROLES, ROUTES } from "router/enums";
import { authActions } from "./authSlice";
import authApi from "services/api/auth";

const auth =
  appFeatures =>
  async ({ email, password, rememberMe }) => {
    try {
      const tokenValue = await authApi.auth({
        email,
        password,
        rememberMe,
      });
      const token = tokenValue.data;
      window.localStorage.setItem("token", token);

      const authValue = {
        token,
        role: ROLES.MANAGER,
      };
      window.localStorage.setItem("auth", JSON.stringify(authValue));
      appFeatures.dispatch(authActions.loginStart(authValue));
      const { data: user } = await authApi.me();

      appFeatures.dispatch(
        authActions.getUserCompleted({
          firstName: user.firstName,
          lastName: user.lastName,
          id: user.userId,
        })
      );

      setTimeout(() => {
        appFeatures.navigate(ROUTES.DASHBOARD);
      }, 500);
    } catch (error) {
      appFeatures.dispatch(authActions.loginError(error.response.data.error));
    }
  };

const fetchUser =
  appFeatures =>
  async ({ navigate }) => {
    try {
      const { data } = await authApi.me();

      appFeatures.dispatch(authActions.getUserCompleted(data));
    } catch (error) {
      if (error?.response?.status === 401) {
        window.localStorage.clear();
        navigate(`/${ROUTES.AUTH}`);
      }
    }
  };

const authBehaviors = {
  auth,
  fetchUser,
};

export type AuthBehaviors = typeof authBehaviors;
export default authBehaviors;
