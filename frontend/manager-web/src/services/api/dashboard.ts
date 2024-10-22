import {statsApi} from "./apiService";

const { fetchApi } = statsApi();

const dashboardApi = {
  getNewLeadsStats: () => fetchApi("/lead/new_leads_statistics", false),
  getLeadsStats: ({ period }) => fetchApi(`/dashboard/call_analysis/${period}`, false),
  getAgentsWorkMode: () => fetchApi(`/dashboard/agents_work_mode`, false),
  getPerfomanceStatistics: ({ type, from, to }) =>
    fetchApi(`/dashboard/performance/${type}/statistics?from=${from}&to=${to}`, false),
  getPerformancePlot: ({ type, from, to, step }) =>
    fetchApi(
      `/dashboard/performance/${type}/plots?from=${from}&to=${to}&step=${step}`,
      false,
    ),
};

export default dashboardApi;
