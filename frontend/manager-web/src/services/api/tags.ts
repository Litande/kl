import apiService from "./apiService";

const { fetchApi, postApi, putApi, deleteApi } = apiService();

const tagsApi = {
  getAllTags: () => fetchApi("/tags"),
  createTag: tag => postApi(`/tags`, tag),
  deleteTag: tagId => deleteApi(`/tags/${tagId}`),
  updateTag: tag => putApi(`/tags/${tag.id}`, tag),
};

export default tagsApi;
