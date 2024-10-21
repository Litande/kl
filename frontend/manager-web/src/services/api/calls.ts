import apiService from "./apiService";

const { fetchApi, postApi } = apiService();

const callsApi = {
  getCalls: (data = {}, pagination) => postApi(`/calls?`, data, { params: pagination }),
  getAudioCall: callId => fetchApi(`/calls/${callId}/audio`, false, { responseType: "blob" }),
};

export default callsApi;
