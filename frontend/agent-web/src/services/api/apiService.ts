import axiosInstance, { getToken as getAuth } from "services/api";

const cacheData = {};

const apiService = () => {
  const fetchApi = async (url, cache = false, params = {}) => {
    const key = `${url}-get`;
    if (!cacheData[key]) {
      const result = await axiosInstance.get(url, params);
      if (cache) {
        cacheData[key] = result;
      }
      return result;
    }
    return cacheData[url];
  };

  const postApi = async (url, data) => {
    return axiosInstance.post(url, data, {});
  };

  const putApi = async (url, data) => {
    return axiosInstance.put(url, data, {});
  };

  const deleteApi = async (url, data?) => {
    return axiosInstance.delete(url, { data });
  };

  return { fetchApi, postApi, putApi, deleteApi, getAuth };
};

export default apiService;
