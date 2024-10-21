import apiService from "services/api/apiService";

const { fetchApi } = apiService();

const leadsApi = {
  getLeadsQueue: () => fetchApi("/leads/queues"),
};

export default leadsApi;
