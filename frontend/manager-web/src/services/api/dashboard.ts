import apiService from "./apiService";
import { createBaseURL } from "services/api/index";
import { STATISTIC_WS } from "services/websocket/const";

const baseUrl = createBaseURL(STATISTIC_WS);

const { fetchApi } = apiService();
const params = {
  baseURL: baseUrl,
};
const dashboardApi = {
  getLeadsStats: ({ period }) => fetchApi(`/dashboard/call_analysis/${period}`, false, params),
  getAgentsWorkMode: () => fetchApi(`/dashboard/agents_work_mode`, false, params),
  getPerfomanceStatistics: ({ type, from, to }) =>
    fetchApi(`/dashboard/performance/${type}/statistics?from=${from}&to=${to}`, false, params),
  getPerformancePlot: ({ type, from, to, step }) =>
    fetchApi(
      `/dashboard/performance/${type}/plots?from=${from}&to=${to}&step=${step}`,
      false,
      params
    ),
};

export default dashboardApi;
