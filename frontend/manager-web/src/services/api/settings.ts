import apiService from "./apiService";

const { fetchApi, putApi } = apiService();

const settingsApi = {
  getSettings: () => fetchApi("/settings"),
  getSetting: type => fetchApi(`/settings/${type}`),
  updateSetting: (type, data) => putApi(`/settings/${type}`, data),
};

export default settingsApi;
