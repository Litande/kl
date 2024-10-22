import * as signalR from "@microsoft/signalr";
import eventDomDispatcher from "services/events/EventDomDispatcher";
import {
  CLOSE_PEER_EVENT,
  closedPeerAction,
  CONNECT_EVENT,
  DISCONNECT_EVENT,
} from "agent/actions/connectionActions";
import { changeStateAction, STATES } from "agent/actions/changeStateAction";

const STUN_URL = ["stun:stun.l.google.com:19302", "stun:stun1.l.google.com:19302"];

let pc, ws, audio, localStream;
let lead;
let connection;

const callService = () => {
  const { addEventListener, removeEventListener } = eventDomDispatcher();

  const connectHandle = ({ detail }: CustomEvent) => connect(detail.agentID);
  const disconnectHandle = () => disconnect();
  const closePeerHadle = () => closePeer();

  const init = () => {
    addEventListener(CONNECT_EVENT, connectHandle);
    addEventListener(DISCONNECT_EVENT, disconnectHandle);
    addEventListener(CLOSE_PEER_EVENT, closePeerHadle);
  };

  const destroy = () => {
    removeEventListener(CONNECT_EVENT, connectHandle);
    removeEventListener(DISCONNECT_EVENT, disconnectHandle);
    removeEventListener(CLOSE_PEER_EVENT, closePeerHadle);
  };

  const connect = (agentID: string) => {
    if (
      connection &&
      (connection.state === signalR.HubConnectionState.Connecting ||
        connection.state === signalR.HubConnectionState.Connected ||
        connection.state === signalR.HubConnectionState.Reconnecting)
    ) {
      return;
    }

    connection = new signalR.HubConnectionBuilder()
      .withUrl("https://qa.agent.api.kollink.ai/agent", {
        accessTokenFactory: () => agentID,
      })
      .build();

    connection.on("ReceiveAgent", function (message) {
      lead = message;
      changeStateAction(STATES.CALLING_STATE);
      call();
    });

    connection.start().catch(err => console.log(err));
  };

  const disconnect = () => {
    connection && connection.stop().catch(err => console.log(err));
    closePeer();
  };

  const call = async () => {
    pc = new RTCPeerConnection({ iceServers: [{ urls: STUN_URL }] });

    // Add an audio source to the peer connection.
    localStream = await navigator.mediaDevices.getUserMedia({
      video: false,
      audio: true,
    });
    localStream.getTracks().forEach(track => pc.addTrack(track, localStream));
    pc.ontrack = evt => (audio.srcObject = evt.streams[0]);
    pc.onicecandidate = evt => evt.candidate && ws.send(JSON.stringify(evt.candidate));
    audio = document.querySelector("#audioCtl");
    // Diagnostics.
    pc.onicegatheringstatechange = () =>
      console.log("onicegatheringstatechange: " + pc.iceGatheringState);
    pc.oniceconnectionstatechange = async () => {
      console.log("oniceconnectionstatechange: " + pc.iceGatheringState);
      switch (pc.connectionState) {
        case "disconnected":
        case "closed":
        case "failed":
          await closePeer();
      }
    };
    pc.onsignalingstatechange = () => console.log("onsignalingstatechange: " + pc.signalingState);
    pc.onconnectionstatechange = async ev => {
      console.log("onconnectionstatechange: " + pc.connectionState);
      switch (pc.connectionState) {
        case "disconnected":
        case "closed":
        case "failed":
          await closePeer();
      }
    };

    ws = new WebSocket(lead.webRtcEndpointUrl, []);

    ws.onmessage = async function (evt) {
      if (/^[{"'\s]*candidate/.test(evt.data)) {
        pc.addIceCandidate(JSON.parse(evt.data));
      } else {
        await pc.setRemoteDescription(new RTCSessionDescription(JSON.parse(evt.data)));
        pc.createAnswer()
          .then(answer => pc.setLocalDescription(answer))
          .then(() => ws.send(JSON.stringify(pc.localDescription)));
      }
    };
    ws.onclose = async function () {
      console.log("close peer.");
      await closePeer();
    };
  };

  const closePeer = async () => {
    console.log("ClosePeer");
    await pc?.close();
    await ws?.close();
    if (audio) {
      let stream = audio.srcObject;

      const tracks = stream.getTracks();

      tracks.forEach(track => {
        track.stop();
      });
      audio.srcObject = null;
      stream = null;
    }
    if (localStream) {
      localStream.getTracks().forEach(track => track.stop());
    }

    pc = null;
    ws = null;
    audio = null;
    localStream = null;

    onClose();
  };

  const onClose = () => {
    closedPeerAction();
  };

  return {
    init: init,
    destroy: destroy,
    closePeer: closePeer,
    getLead: () => lead,
  };
};

export default callService;
