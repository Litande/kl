import callsApi from "services/api/calls";

export const fetchAudioCall = async callId =>
  callsApi.getAudioCall(callId).then(({ data }) => data);
