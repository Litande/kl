import AgentModel from "data/user/AgentModel";
import { AgentStatus, CallType } from "data/user/types";
import { WS_STATUS } from "services/websocket/WebSocketTypes";
import ConnectionService from "services/websocket/ConnectionService";
import { BehaviorSubject } from "rxjs";
import { AUTHENTICATE_ERROR } from "services/websocket/errors";
import { Command, HangupReason } from "./type";

const AVAILABLE_WAIT_TIME_FOR_CANDIDATE = 5000;

class CallService {
  private static _instance = null;

  private _pc;
  private _ws;
  private _audio;
  private _localStream;
  private _isCandidate: boolean;
  private _isLoading: BehaviorSubject<boolean>;
  private _agent: AgentModel;
  private _connectionService: ConnectionService;
  private _stunUrls: string[];

  constructor() {
    if (CallService._instance !== null) {
      throw new Error("Call Service already created use instance instead");
    }
    console.warn("CallService Init");

    CallService._instance = this;
    this._isCandidate = false;
    this._isLoading = new BehaviorSubject<boolean>(false);
    this._connectionService = new ConnectionService(error => {
      if (error?.code === AUTHENTICATE_ERROR) {
        this._agent.destroy();
        this.closePeer(true);
        return;
      }
      this._isLoading.next(false);
      this._agent.addTemporaryError(1011, "Connection closed with an error.");
      this._agent.gotoOffline();
    });
    this.onCurrentStatusHandler = this.onCurrentStatusHandler.bind(this);
    this.onCallInfoHandler = this.onCallInfoHandler.bind(this);
    this.closePeer = this.closePeer.bind(this);
  }

  getConnectionStatus(): BehaviorSubject<WS_STATUS> {
    return this._connectionService.status;
  }

  static getInstance(): CallService {
    if (!CallService._instance) {
      new CallService();
    }

    return CallService._instance;
  }

  public setAgent(agent: AgentModel): void {
    this._agent = agent;
  }

  public onCurrentStatusHandler(message): void {
    this._agent.setCurrentStatus(message);
  }

  public onCallInfoHandler(message): void {
    console.warn("onReceive:", message);
    if (this.canStartCall(message)) {
      this._agent.setInfo(message);
      this._agent.tryCall();
    }
  }

  public connect(stuns: string[]): void {
    if (this._agent.id === -1) {
      return;
    }
    this._stunUrls = stuns;
    if (this._connectionService.status.getValue() !== WS_STATUS.CLOSED) {
      return;
    }
    this._connectionService.addChannelListener("CurrentStatus", this.onCurrentStatusHandler);
    this._connectionService.addChannelListener("CallInfo", this.onCallInfoHandler);

    this._connectionService.connect();
  }

  // TODO should be subscribed to this._agent.status
  public sendChangeStatus(newStatus: AgentStatus): void {
    this.invoke("ChangeStatus", newStatus);
  }

  // should use send before stop connection
  public logout(): void {
    this._connectionService.send({
      methodName: "ChangeStatus",
      data: AgentStatus.Offline,
    });
  }

  public startManualCall(phoneNumber): void {
    this.invoke("ManualCall", phoneNumber);
  }

  private canStartCall({ id }) {
    return this._agent.getActiveLead() === null;
  }

  public async callAgain(): Promise<void> {
    this.invoke("CallAgain");
  }

  private async invoke(method: string, args = null): Promise<void> {
    const res = await this._connectionService.invoke({
      methodName: method,
      data: args,
    });
    if (res.error) {
      this._agent && this._agent.addTemporaryError(res.error.code, res.error.message);
    }
  }

  private disconnect() {
    this._connectionService?.removeChannelListener("CurrentStatus", this.onCurrentStatusHandler);
    this._connectionService?.removeChannelListener("CallInfo", this.onCallInfoHandler);
    this._connectionService?.unsubscribeWS();
    this.closePeer();
  }

  // If candidate doesn't connect to the call
  private checkCall(): void {
    setTimeout(() => {
      if (!this._isCandidate) {
        console.warn("TimeoutError: Candidate not founded");
        this._agent.callType.getValue() === CallType.Predictive && this._agent.gotoNext();
        this.closePeer(true);

        this._agent.addTemporaryError("Timeout Request Error", "Can't connect to candidate");
      }
    }, AVAILABLE_WAIT_TIME_FOR_CANDIDATE);
  }

  public async call(): Promise<void> {
    console.warn("CALL SERVICE:: start call");
    this._isLoading.next(true);
    this.checkCall();
    this._isCandidate = false;
    this._pc = new RTCPeerConnection({
      iceServers: [{ urls: this._stunUrls }],
    });
    // Add an audio source to the peer connection.
    this._localStream = await navigator.mediaDevices.getUserMedia({
      video: false,
      audio: true,
    });
    this._localStream.getTracks().forEach(track => this._pc.addTrack(track, this._localStream));
    this._pc.ontrack = evt => (this._audio.srcObject = evt.streams[0]);
    this._pc.onicecandidate = evt => {
      evt.candidate && this._ws.send(JSON.stringify(evt.candidate));
    };
    this._audio = document.querySelector("#audioCtl");
    // Diagnostics.
    this._pc.onicegatheringstatechange = () =>
      console.warn("oniceGathering State Change:" + this._pc.iceGatheringState);
    this._pc.oniceconnectionstatechange = async () => {
      console.warn("onConnectionChange::: " + this._pc.iceGatheringState);
      switch (this._pc.connectionState) {
        case "disconnected":
        case "closed":
        case "failed": {
          await this.closePeer();
        }
      }
    };

    this._ws = new WebSocket(this._agent.getActiveLead().agentRtcUrl, []);

    this._ws.onmessage = async evt => {
      if (evt.data.size === 0) return;
      if (/^[{"'\s]*candidate/.test(evt.data)) {
        this._isLoading.next(false);
        this._isCandidate = true;
        this._agent.updateTimeAnswerAt();
        this._pc.addIceCandidate(JSON.parse(evt.data));
      } else {
        await this._pc.setRemoteDescription(new RTCSessionDescription(JSON.parse(evt.data)));
        this._pc
          .createAnswer()
          .then(answer => this._pc.setLocalDescription(answer))
          .then(() => this._ws.send(JSON.stringify(this._pc.localDescription)))
          .catch(() => console.warn("catch"));
      }
    };
    this._ws.onclose = async () => {
      this.closePeer();
    };
  }

  private async closePeer(isCloseLoading = false): Promise<void> {
    console.warn("ClosePeer");
    if (isCloseLoading) {
      this._isLoading.next(false);
    }
    await this._pc?.close();
    await this._ws?.close();
    if (this._audio) {
      let stream = this._audio.srcObject;

      const tracks = stream?.getTracks();

      tracks?.forEach(track => {
        track.stop();
      });
      this._audio.srcObject = null;
      stream = null;
    }
    if (this._localStream) {
      this._localStream.getTracks().forEach(track => track?.stop());
    }

    this._pc = null;
    this._ws = null;
    this._audio = null;
    this._localStream = null;
  }

  public hangupCall(hangupReason = HangupReason.na, comment = ""): void {
    const data = {
      command: Command.HangupCall,
      params: {
        hangupReason,
        comment,
      },
    };
    this._ws?.send(JSON.stringify(data));
  }

  public stop(): void {
    this.disconnect();
  }

  public destroy(): void {
    this.stop();
  }

  private getAudioTracks() {
    return this._pc
      ?.getSenders()
      .map(sender => sender.track)
      .filter(track => track?.kind === "audio");
  }

  public mute(): void {
    this.getAudioTracks()?.forEach(track => {
      track.enabled = false;
    });
  }

  public unmute(): void {
    this.getAudioTracks()?.forEach(track => {
      track.enabled = true;
    });
  }

  public get isLoading(): BehaviorSubject<boolean> {
    return this._isLoading;
  }
}

export default CallService;
