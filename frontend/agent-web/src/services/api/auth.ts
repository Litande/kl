import apiService from "services/api/apiService";

const { fetchApi, postApi } = apiService();

const authApi = {
  auth: data => postApi("/user/login", data),
  me: () => fetchApi("/user/me"),
};

export default authApi;
