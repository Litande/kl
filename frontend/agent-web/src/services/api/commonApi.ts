import apiService from "services/api/apiService";

const { fetchApi } = apiService();

const commonApi = {
  getStatuses: () => fetchApi("/commons/leads/statuses", true),
};

export default commonApi;
