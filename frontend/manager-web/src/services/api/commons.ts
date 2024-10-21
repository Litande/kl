import apiService from "./apiService";
import { AxiosResponse } from "axios";

const { fetchApi } = apiService();

const commonsApi = {
  getLeadsStatuses: () => fetchApi("/commons/leads/statuses", true),
  getCountries: () => fetchApi("/commons/countries", true),
  getAgents: () => fetchApi(`/agents`) as Promise<AxiosResponse>,
};

export default commonsApi;
