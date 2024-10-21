import React, { useState } from "react";
import styled from "styled-components";

import * as signalR from "@microsoft/signalr";

const STUN_URL = ["stun:stun.l.google.com:19302", "stun:stun1.l.google.com:19302"];
let pc, ws, audio, localStream;

const Dashboard = () => {
  const [value, setValue] = useState("");
  const [lead, setLead] = useState(null);

  const handleConnect = () => {
    componentConnect();
    console.log(value);
  };

  const connection = new signalR.HubConnectionBuilder()
    .withUrl("https://qa.agent.api.aorta.ai/agent", {
      accessTokenFactory: () => value,
    })
    .build();

  const handleDisconect = () => {
    connection.stop().catch(err => console.log(err));
  };

  const componentConnect = () => {
    connection.start().catch(err => console.log(err));
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
  };

  connection.on("ReceiveAgent", function (message) {
    setLead(message);
  });

  return (
    <Wrap>
      <Header>Dashboard</Header>
      <AgentWrap>
        <ButtonsWrap>
          <label>AgentId:</label>
          <input value={value} onChange={e => setValue(e.target.value)} />
        </ButtonsWrap>
        <ButtonsWrap>
          <TempButtonWrap onClick={handleConnect}>Connect</TempButtonWrap>
          <TempButtonWrap onClick={handleDisconect}>Disonnect</TempButtonWrap>
        </ButtonsWrap>
      </AgentWrap>

      <label>Phone:</label>
      <div>{lead?.phone}</div>
      <label>Name:</label>
      <div>
        {lead?.firstName} {lead?.lastName}
      </div>
      <label>WebRtcEndpointUrl:</label>
      <div>{lead?.webRtcEndpointUrl}</div>
      <div>
        <audio controls autoPlay id="audioCtl" />
      </div>
      <ButtonsWrap>
        <button onClick={call}>Answer</button>
        <button onClick={closePeer}>Close</button>
      </ButtonsWrap>
    </Wrap>
  );
};

export default Dashboard;

const Wrap = styled.div`
  display: flex;
  flex-direction: column;
  gap: 0.5rem;
`;

const Header = styled.h2`
  margin: 0 1.5rem 1.5rem 0;
  align-self: end;
  cursor: pointer;
`;

const AgentWrap = styled.div`
  display: flex;
  flex-direction: column;
  margin-bottom: 1rem;
`;

const ButtonsWrap = styled.div`
  display: flex;
  flex-direction: row;
  margin-bottom: 1rem;
  margin-left: 1rem;
  gap: 1rem;
`;

const TempButtonWrap = styled.button`
  width: 100px;
`;
