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

class ConnectionService implements IConectionService {
  listeners: HandlingType = {};
  status: BehaviorSubject<WS_STATUS> = new BehaviorSubject<WS_STATUS>(WS_STATUS.CLOSED);
  ws: IWebSocket;
  reconnectionService: ReconnectionService;

  constructor(onError) {
    this.onMessage = this.onMessage.bind(this);
    this.changeStatus = this.changeStatus.bind(this);

    this.ws = SignalConnection({
      onMessage: this.onMessage,
      changeStatus: this.changeStatus,
      onError: onError,
    });

    this.reconnectionService = ReconnectionWSService();
    this.reconnectionService.init(this.getAvailableReconnectActions());
  }

  public onMessage(res: IResponse) {
    if (this.listeners && this.listeners[res.eventName]) {
      this.listeners[res.eventName](res.data);
    } else {
      console.warn("Unknown methodName:", res.eventName);
    }
  }

  changeStatus(value: WS_STATUS) {
    if (this.status.getValue() !== value) {
      this.status.next(value);
    }
  }

  getAvailableReconnectActions(): IReconnectionService {
    return {
      status: this.status,
      connect: this.connect,
    };
  }

  connect = (): void => {
    this.ws.connect();
    this.reconnectionService.start();
  };

  send = (message: IMessageRequest) => {
    this.ws.send(message);
  };

  invoke = async (message: IMessageRequest): Promise<any> => {
    return this.ws.invoke(message);
  };

  forceReconnect() {
    this.reconnectionService.forceReconnect();
  }

  addChannelListener = (channelName: string, callback) => {
    if (!this.listeners[channelName]) {
      this.listeners[channelName] = callback;
      this.ws?.subscribe(channelName);
    }
  };

  removeChannelListener = (channelName: string, _callback) => {
    if (this.listeners[channelName]) {
      this.ws?.unsubscribe(channelName);
      delete this.listeners[channelName];
    }
  };

  unsubscribeWS() {
    console.warn("ConnectionService.Websocket:Unsubscribe");
    this.reconnectionService.stop();
    if (this.ws) {
      this.ws.close();
    }
  }
  removeCurrentWS = () => {
    this.reconnectionService.stop();
    this.ws && this.ws.close();
  };
}

export default ConnectionService;
