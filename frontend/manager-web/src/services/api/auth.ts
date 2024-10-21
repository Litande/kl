import apiService from "services/api/apiService";

const { fetchApi, postApi } = apiService();
export const AUTH_PATH = "/user/login";
export const ME_PATH = "/user/me";
const authApi = {
  auth: data => postApi(AUTH_PATH, data),
  me: () => fetchApi(ME_PATH),
};

export const getToken = () => window.localStorage.getItem("token");

export default authApi;
