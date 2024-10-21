import apiService from "services/api/apiService";

const { fetchApi, postApi } = apiService();

type FilledData = {
  leadId: number;
  sessionId: number;
  leadStatus: string | null;
  remindOn: Date;
  comment: string;
};

type HistoryData = {
  page?: number;
};

const agentApi = {
  filledcall: (data: FilledData) => postApi("/agents/filledcall", data),
  getCallbacks: () => fetchApi("/leads/callbacks", false),
  getHistory: ({ page = 1 }: HistoryData) => fetchApi(`/history?page=${page}`, false),
  getRecord: ({ id }) => fetchApi(`/history/${id}/play`, false, { responseType: "blob" }),
  getSettings: () => fetchApi("/settings", false),
};

export default agentApi;
