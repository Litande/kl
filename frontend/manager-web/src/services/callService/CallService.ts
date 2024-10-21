import { CallInfo, CallStatus, Command, HangupReason } from "services/callService/types";
import { BehaviorSubject } from "rxjs";

class CallService {
  private static _instance = null;

  private _pc;
  private _ws: WebSocket;
  private _stunUrls: string[];
  private _audio;
  private _localStream;
  private _lead;
  private _callInfo: Partial<CallInfo>;
  private _status: BehaviorSubject<CallStatus>;
  private _isCallMuted: BehaviorSubject<boolean>;

  constructor() {
    if (CallService._instance !== null) {
      throw new Error("Call Service already created use instance instead");
    }

    console.warn("CallService");

    this._status = new BehaviorSubject<CallStatus>(CallStatus.OFFLINE);
    this._isCallMuted = new BehaviorSubject(false);

    CallService._instance = this;
  }

  static getInstance(): CallService {
    if (!CallService._instance) {
      new CallService();
    }

    return CallService._instance;
  }

  public setStuns(stuns: string[]): void {
    this._stunUrls = stuns;
  }

  public setCallInfo(info: Partial<CallInfo>): void {
    console.warn("setCallInfo:", info);
    this._callInfo = info;
  }

  public connect(): void {
    if (!this._callInfo || this._status.getValue() !== CallStatus.OFFLINE) {
      return;
    }

    this._status.next(CallStatus.PREPARING);
    this.call();
  }

  private sendMode() {
    if (this._callInfo) {
      const data = {
        command: Command.SetManagerMode,
        params: {
          mode: this._callInfo.mode,
        },
      };
      this._ws.send(JSON.stringify(data));
    }
  }

  public disconnect() {
    this.closePeer();
  }

  public endCall() {
    const data = {
      command: Command.HangupCall,
      params: {
        hangupReason: HangupReason.na,
        comment: HangupReason.na,
      },
    };
    this._ws.send(JSON.stringify(data));
    this.disconnect();
  }

  private async call(): Promise<void> {
    console.warn("CALL SERVICE:: start call");
    this._status.next(CallStatus.IN_CALL);
    this._isCallMuted.next(false);
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
    this._pc.onicegatheringstatechange = () => {
      console.warn("oniceGathering State Change:" + this._pc.iceGatheringState);
    };
    this._pc.onconnectionstatechange = async () => {
      console.warn("onConnectionChange::: " + this._pc.connectionState);
      switch (this._pc.connectionState) {
        case "connected": {
          this.sendMode();
          break;
        }
        case "disconnected":
        case "closed":
        case "failed": {
          await this.closePeer();
        }
      }
    };

    this._ws = new WebSocket(this._callInfo.rtcUrl, []);
    this._ws.onmessage = async evt => {
      if (evt.data.size === 0) return;
      if (/^[{"'\s]*candidate/.test(evt.data)) {
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
    this._ws.onopen = () => {
      console.warn("onOpen");
    };
    this._ws.onclose = async () => {
      this.closePeer();
    };
  }

  private async closePeer(): Promise<void> {
    console.warn("ClosePeer");
    await this._pc?.close();
    await this._ws?.close();
    if (this._audio) {
      let stream = this._audio.srcObject;

      const tracks = stream.getTracks();

      tracks.forEach(track => {
        track.stop();
      });
      this._audio.srcObject = null;
      stream = null;
    }
    if (this._localStream) {
      this._localStream.getTracks().forEach(track => track.stop());
    }

    this._pc = null;
    this._ws = null;
    this._audio = null;
    this._localStream = null;
    this._callInfo = null;

    this._status.next(CallStatus.OFFLINE);
  }

  public stop(): void {
    this.disconnect();
  }

  public get status(): BehaviorSubject<CallStatus> {
    return this._status;
  }

  public get callInfo(): Partial<CallInfo> {
    return this._callInfo;
  }

  public destroy(): void {
    this.stop();
    this.closePeer();
    this._callInfo = null;
  }

  get isCallMuted(): BehaviorSubject<boolean> {
    return this._isCallMuted;
  }

  private getAudioTracks() {
    return this._pc
      .getSenders()
      .map(sender => sender.track)
      .filter(track => track?.kind === "audio");
  }

  public mute(): void {
    this.getAudioTracks().forEach(track => {
      track.enabled = false;
    });
    this._isCallMuted.next(true);
  }

  public unmute(): void {
    this.getAudioTracks().forEach(track => {
      track.enabled = true;
    });
    this._isCallMuted.next(false);
  }
}

export default CallService;
