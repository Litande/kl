import { BehaviorSubject } from "rxjs";

export const MAX_ATTEMPTS = 10;
export const RECONNECT_DELAY = 1000;

export enum WS_STATUS {
  CONNECTING,
  CONNECTED,
  CLOSED,
}

export interface IConectionService {
  connect: () => void;
  send: (message: IMessageRequest) => void;
  invoke: (message: IMessageRequest) => Promise<any> | BaseError;
  status: BehaviorSubject<WS_STATUS>;
  forceReconnect: () => void;
  addChannelListener: (channelName: string, callback) => void;
  removeChannelListener: (channelName: string, callback) => void;
  unsubscribeWS: () => void;
  removeCurrentWS: () => void;
}

export interface IWebSocket {
  connect: () => void;
  send: (message: IMessageRequest) => void;
  invoke: (message: IMessageRequest) => Promise<any> | BaseError;
  close: () => void;
  unsubscribe: (chanelName) => void;
  subscribe: (chanelName) => void;
}

export interface IMessageRequest {
  methodName: string;
  data: string;
}

export interface IMessageResponse {
  id: number;
}

export type WebSocketProps = {
  changeStatus: (value: WS_STATUS) => void;
  onMessage: (res: IResponse) => void;
  onError: (error) => void;
};

export type IResponse = {
  eventName: string;
  data: IMessageResponse;
};

export type HandlingType = {
  [key: string]: (req: IMessageResponse) => void;
};

export type BaseError = {
  code: number;
  message: string;
};
