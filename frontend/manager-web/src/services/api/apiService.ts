import axiosInstance from "./";

const cacheData = {};
let controller = new AbortController();
const apiService = () => {
  const fetchApi = async (url, cache = false, params = {}) => {
    const key = `${url}-get`;
    params = { ...params, signal: controller.signal };
    if (!cacheData[key]) {
      const result = await axiosInstance.get(url, params);
      if (cache) {
        cacheData[key] = result;
      }
      return result;
    }
    return cacheData[url];
  };

  const postApi = async (url, data, config = {}) => {
    return axiosInstance.post(url, data, config);
  };

  const putApi = async (url, data, config = {}) => {
    return axiosInstance.put(url, data, config);
  };

  const deleteApi = async (url, data?) => {
    return axiosInstance.delete(url, { data });
  };
  const cancelRequests = () => {
    controller.abort();
    controller = new AbortController();
  };

  return { fetchApi, postApi, putApi, deleteApi, cancelRequests };
};

export default apiService;
