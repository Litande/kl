import apiService from "services/api/apiService";

const { fetchApi, postApi } = apiService();

const leadsApi = {
  getComments: (leadId, pagination) =>
    fetchApi(`leads/${leadId}/comments`, false, { params: pagination }),
  saveComment: (leadId, comment: string) =>
    postApi(`leads/${leadId}/comments`, { comment: comment }),
};

export default leadsApi;
