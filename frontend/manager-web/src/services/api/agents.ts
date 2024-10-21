import apiService from "./apiService";

const { fetchApi, putApi } = apiService();

const agentsApi = {
  getAllAgents: () => fetchApi("/agents"),
  updateAgent: agent => putApi(`/agents/${agent.agentId}`, agent),
};

export default agentsApi;
