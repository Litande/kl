import * as signalR from "@microsoft/signalr";
import { createBaseURL, getToken } from "services/api";

import {
  IMessageRequest,
  IResponse,
  IWebSocket,
  WebSocketProps,
  WS_STATUS,
} from "services/websocket/WebSocketTypes";
import { AUTHENTICATE_ERROR, NO_INTERNET_CONNECTION_ERROR } from "services/websocket/errors";

class SignalConnection implements IWebSocket {
  private _url: string;
  private _connection: signalR.HubConnection;
  private _onError: (error: any) => void;
  private _onConnect: () => void;
  private _onMessage: (res: IResponse) => void;
  private _changeStatus: (status: WS_STATUS) => void;
  private _channels: Array<string>;
  private _isConnected: boolean;
  constructor({ onConnect, onMessage, changeStatus, onError, url }: WebSocketProps) {
    this._changeStatus = changeStatus;
    this._url = url;
    this._onConnect = onConnect;
    this._onMessage = onMessage;
    this._changeStatus = changeStatus;
    this._channels = [];
    this._isConnected = false;
  }

  logLevelMap = {
    0: "None",
    1: "Debug",
    2: "Trace",
    3: "Information",
    4: "Warning",
    5: "Error",
    6: "Critical",
  };

  logLevel =
    process.env.NODE_ENV === "development"
      ? signalR.LogLevel[this.logLevelMap[3]]
      : signalR.LogLevel[this.logLevelMap[6]];

  public connect = () => {
    this._changeStatus(WS_STATUS.CONNECTING);

    try {
      this._connection = new signalR.HubConnectionBuilder()
        .withUrl(this._url, { accessTokenFactory: () => getToken() })
        .configureLogging(this.logLevel)
        .build();
    } catch (e) {
      console.warn(e);
    }

    this._connection.on("Leave", message => {
      console.warn("LeaveRooms::", message);
    });

    this._connection.onclose(error => {
      if (error === null) {
        this._changeStatus(WS_STATUS.CLOSED);
      } else {
        this._onError(error);
      }
    });

    this._connection
      .start()
      .then(() => {
        this._changeStatus(WS_STATUS.CONNECTED);
        this._onConnect();
        this.joinToChanels();
      })
      .catch(e => {
        // huck to find 401 error
        if (e.message.search(AUTHENTICATE_ERROR) !== -1) {
          this._onError({
            code: AUTHENTICATE_ERROR,
            message: "Authenticate error, please login again",
          });
        }
        this._changeStatus(WS_STATUS.CLOSED);
      });
  };

  public isConnected = () => {
    return this._connection
      ? this._connection.state === signalR.HubConnectionState.Connected
      : false;
  };

  public send = (message: IMessageRequest) => {
    this.isConnected() && this._connection.send(message.methodName, message.data);
  };

  public invoke = async (message: IMessageRequest) => {
    if (this.isConnected()) {
      return await this._connection.invoke(message.methodName, message.data);
    } else {
      return {
        error: {
          code: NO_INTERNET_CONNECTION_ERROR,
          message: "No internet connections",
        },
      };
    }
  };

  public checkChanelSubscriptions = (chanelName: string) => {
    if (!this._channels) {
      this._channels = [];
    }

    return this._channels.indexOf(chanelName);
  };

  public joinToChanels = () => {
    // this._connection.send("Join");
    this._channels?.forEach(chanel => {
      this.connectToChanel(chanel);
    });
  };

  public subscribe = (chanelName: string) => {
    // add to list if should reconnect
    if (this.checkChanelSubscriptions(chanelName) === -1) {
      this._channels.push(chanelName);
    }

    if (this.isConnected()) {
      this.connectToChanel(chanelName);
    }
  };

  public connectToChanel = (chanelName: string) => {
    this._connection.on(chanelName, message => {
      const res: IResponse = {
        eventName: chanelName,
        data: message,
      };
      this._onMessage(res);
    });
  };

  public unsubscribe = (chanelName: string) => {
    const index = this.checkChanelSubscriptions(chanelName);
    if (index !== -1) {
      this._connection.off(chanelName);
      this._channels.splice(index, 1);
    }
  };

  public close = () => {
    if (
      this._connection &&
      (this._connection.state === signalR.HubConnectionState.Connected ||
        this._connection.state === signalR.HubConnectionState.Connecting)
    ) {
      this._connection.stop();
      this._changeStatus(WS_STATUS.CLOSED);
    }
  };
}

export default SignalConnection;
