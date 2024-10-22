import apiService from "services/api/apiService";
import { AxiosResponse } from "axios";

const { fetchApi, postApi, putApi, deleteApi } = apiService();

const trackingApi = {
  getGroups: () => fetchApi("/tracking/groups"),
 
  getLiveTracking: () => fetchApi("/tracking/live_tracking"),
  getLeadInfo: ({ id }) => fetchApi(`/leads/${id}/shortinfo`),
  getAgent: ({ id }) => fetchApi(`/agents/${id}`),
  getBlacklist: () => fetchApi(`/blacklist`),
  addBlacklist: ({ id }) => postApi(`/blacklist/${id}`, {}),
  removeBlacklist: ({ ids }) => deleteApi(`/blacklist`, { leadIds: ids }),
  updateAgent: ({ id, ...data }) => putApi(`/agents/${id}`, data),
  changeAgentPassword: ({ id, data }) => postApi(`/agents/${id}/changepassword`, data),
  createAgent: data => postApi(`/agents`, data),
  getAgents: () => fetchApi(`/agents`) as Promise<AxiosResponse>,
  getTags: () => fetchApi(`/tags`) as Promise<AxiosResponse>,
  getTeams: () => fetchApi(`/teams`) as Promise<AxiosResponse>,
  getLeads: (data = {}, config = {}) => postApi(`/leads`, data, config) as Promise<AxiosResponse>,
  setLeadAgent: data =>
    putApi(`/leads/${data.leadId}/assignment`, {
      assignedAgentId: data.agentId,
    }) as Promise<AxiosResponse>,
  setLeadStatus: data =>
    putApi(`/leads/${data.leadId}/status`, {
      status: data.status,
    }) as Promise<AxiosResponse>,
  getLeadsQueues: () => fetchApi(`/leadQueues`) as Promise<AxiosResponse>,
  updateLeadQueueRatio: ({ id, ratio }) =>
    putApi(`/leadQueues/${id}/ratio/${ratio}`, {}) as Promise<AxiosResponse>,
  updateLeadQueue: ({ id, data }) => putApi(`/leadQueues/${id}`, data) as Promise<AxiosResponse>,
  getCountries: () => fetchApi(`/commons/countries`) as Promise<AxiosResponse>,
  getLeadStatuses: () => fetchApi(`/commons/leads/statuses`) as Promise<AxiosResponse>,
  getLeadStatInfo: ({ id }) => {
    const data = [
      { id: 1, country: "Canada" },
      { id: 2, country: "Canada" },
      { id: 3, country: "Canada" },
      { id: 4, country: "Canada" },
      { id: 5, country: "Canada" },
      { id: 6, country: "Canada" },
      { id: 7, country: "Canada" },
      { id: 8, country: "Canada" },
    ];
    return new Promise(res => res({ data }));
  },
};

export default trackingApi;
