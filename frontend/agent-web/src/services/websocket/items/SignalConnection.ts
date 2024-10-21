import * as signalR from "@microsoft/signalr";
import apiService from "services/api/apiService";
import { createBaseURL } from "services/api";

import {
  IMessageRequest,
  IResponse,
  IWebSocket,
  WebSocketProps,
  WS_STATUS,
} from "services/websocket/WebSocketTypes";
import { AUTHENTICATE_ERROR, NO_INTERNET_CONNECTION_ERROR } from "services/websocket/errors";

let connection: signalR.HubConnection;
let chanels: Array<string>;

const SignalConnection = ({ onMessage, changeStatus, onError }: WebSocketProps): IWebSocket => {
  const logLevelMap = {
    0: "None",
    1: "Debug",
    2: "Trace",
    3: "Information",
    4: "Warning",
    5: "Error",
    6: "Critical",
  };

  const logLevel =
    process.env.NODE_ENV === "development"
      ? signalR.LogLevel[logLevelMap[3]]
      : signalR.LogLevel[logLevelMap[6]];

  const getHost = () => {
    const url = createBaseURL() + "/agent";
    return { url };
  };

  const getToken = () => {
    return apiService().getAuth();
  };

  const connect = () => {
    changeStatus(WS_STATUS.CONNECTING);

    try {
      connection = new signalR.HubConnectionBuilder()
        .withUrl(getHost().url, { accessTokenFactory: () => getToken() })
        .configureLogging(logLevel)
        .build();
    } catch (e) {
      console.warn(e);
    }

    connection.on("Leave", message => {
      console.warn("LeaveRooms::", message);
    });

    connection.onclose(error => {
      if (error === null) {
        changeStatus(WS_STATUS.CLOSED);
      } else {
        onError(error);
      }
    });

    connection
      .start()
      .then(() => {
        changeStatus(WS_STATUS.CONNECTED);
        joinToChanels();
      })
      .catch(e => {
        // huck to find 401 error
        if (e.message.search(AUTHENTICATE_ERROR) !== -1) {
          onError({
            code: AUTHENTICATE_ERROR,
            message: "Authenticate error, please login again",
          });
        }
        changeStatus(WS_STATUS.CLOSED);
      });
  };

  const isConnected = () => {
    return connection ? connection.state === signalR.HubConnectionState.Connected : false;
  };

  const send = (message: IMessageRequest) => {
    isConnected() && connection.send(message.methodName, message.data);
  };

  const invoke = async (message: IMessageRequest) => {
    if (isConnected()) {
      return await connection.invoke(message.methodName, message.data);
    } else {
      return {
        error: {
          code: NO_INTERNET_CONNECTION_ERROR,
          message: "No internet connections",
        },
      };
    }
  };

  const checkChanelSubscriptions = (chanelName: string) => {
    if (!chanels) {
      chanels = [];
    }

    return chanels.indexOf(chanelName);
  };

  const joinToChanels = () => {
    // connection.send("Join");
    chanels?.forEach(chanel => {
      connectToChanel(chanel);
    });
  };

  const subscribe = (chanelName: string) => {
    // add to list if should reconnect
    if (checkChanelSubscriptions(chanelName) === -1) {
      chanels.push(chanelName);
    }

    if (isConnected()) {
      connectToChanel(chanelName);
    }
  };

  const connectToChanel = (chanelName: string) => {
    connection.on(chanelName, message => {
      const res: IResponse = {
        eventName: chanelName,
        data: message,
      };
      onMessage(res);
    });
  };

  const unsubscribe = (chanelName: string) => {
    const index = checkChanelSubscriptions(chanelName);
    if (index !== -1) {
      connection.off(chanelName);
      chanels.splice(index, 1);
    }
  };

  const close = () => {
    if (connection && connection.state === signalR.HubConnectionState.Connected) {
      connection.stop();
      changeStatus(WS_STATUS.CLOSED);
    }
  };

  return {
    connect,
    invoke,
    subscribe,
    unsubscribe,
    send,
    close,
  };
};

export default SignalConnection;
