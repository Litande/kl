import { BehaviorSubject } from "rxjs";
import {
  HandlingType,
  IConectionService,
  IMessageRequest,
  IResponse,
  IWebSocket,
  WS_STATUS,
} from "services/websocket/WebSocketTypes";
import SignalConnection from "services/websocket/items/SignalConnection";
import ReconnectionWSService from "services/reconnectionService/ReconnectionWSService";
import { IReconnectionService, ReconnectionService } from "services/reconnectionService/types";

class ConnectionModel implements IConectionService {
  private _listeners: HandlingType = {};
  private _status: BehaviorSubject<WS_STATUS> = new BehaviorSubject<WS_STATUS>(WS_STATUS.CLOSED);
  private _ws: IWebSocket;
  private _reconnectionService: ReconnectionService;
  private _url: string;
  private _listUnsendMethods: Map<string, IMessageRequest>;

  constructor(url, onError) {
    this.onMessage = this.onMessage.bind(this);
    this.changeStatus = this.changeStatus.bind(this);
    this._listUnsendMethods = new Map();
    this._url = url;
    this._ws = new SignalConnection({
      onConnect: this.onConnect,
      onMessage: this.onMessage,
      changeStatus: this.changeStatus,
      onError: onError,
      url,
    });

    console.warn("CONNECTION MODEL::: url", url);

    this._reconnectionService = ReconnectionWSService();
    this._reconnectionService.init(this.getAvailableReconnectActions());
  }

  private onConnect = () => {
    this._listUnsendMethods.forEach((value, key) => {
      this.invoke(value);
      this._listUnsendMethods.delete(key);
    });
  };

  public onMessage = (res: IResponse) => {
    if (this._listeners && this._listeners[res.eventName]) {
      this._listeners[res.eventName](res.data);
    } else {
      console.warn("Unknown methodName:", res.eventName, this._url);
    }
  };

  public changeStatus(value: WS_STATUS) {
    if (this._status.getValue() !== value) {
      this._status.next(value);
    }
  }

  public getAvailableReconnectActions(): IReconnectionService {
    return {
      status: this._status,
      connect: this.connect,
    };
  }

  public connect(): void {
    this._ws.connect();
    this._reconnectionService.start();
  }

  public send = async (message: IMessageRequest) => {
    this._ws.send(message);
  };

  public invoke = async (message: IMessageRequest): Promise<any> => {
    if (!this.isConnected()) {
      message.isRepeatAfterConnection && this._listUnsendMethods.set(message.methodName, message);
      return;
    }
    return this._ws.invoke(message);
  };

  private isConnected() {
    return this._status.getValue() === WS_STATUS.CONNECTED;
  }

  public forceReconnect() {
    this._reconnectionService.forceReconnect();
  }

  public addChannelListener = (channelName: string, callback) => {
    if (!this._listeners[channelName]) {
      this._listeners[channelName] = callback;
      this._ws?.subscribe(channelName);
    }
  };

  get status(): BehaviorSubject<WS_STATUS> {
    return this._status;
  }

  public removeChannelListener = (channelName: string, _callback) => {
    if (this._listeners[channelName]) {
      this._ws?.unsubscribe(channelName);
      delete this._listeners[channelName];
    }
  };

  public unsubscribeWS() {
    console.warn("ConnectionService.Websocket:Unsubscribe");
    this._reconnectionService.stop();
    if (this._ws) {
      this._ws.close();
    }
  }

  removeCurrentWS = () => {
    console.warn("ConnectionService.Websocket:Remove");
    this._reconnectionService.stop();
    let canRemoved = true;
    while (canRemoved) {
      const key = Object.keys(this._listeners)[0];
      this.removeChannelListener(key, this._listeners[key]);
      canRemoved = !!Object.keys(this._listeners).length;
    }
    this._ws && this._ws.close();
  };
}

export default ConnectionModel;
