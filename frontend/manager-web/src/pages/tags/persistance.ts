import tagsApi from "services/api/tags";
import agentsApi from "services/api/agents";
import trackingApi from "services/api/tracking";

export const fetchAllTags = async () =>
  await tagsApi
    .getAllTags()
    .then(({ data }) => data)
    .then(({ items }) => items);

export const createTag = async tag => await tagsApi.createTag(tag).then(({ data }) => data);

export const deleteTag = async tagId => await tagsApi.deleteTag(tagId);

export const updateTag = async tag => await tagsApi.updateTag(tag).then(({ data }) => data);

export const fetchAllAgents = async () =>
  await agentsApi.getAllAgents().then(({ data }) => data.items);

export const updateAgent = async agent =>
  await agentsApi.updateAgent(agent).then(({ data }) => data);

export const fetchTeams = async () => await trackingApi.getTeams().then(({ data }) => data.items);
