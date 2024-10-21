import { BehaviorSubject } from "rxjs";
import { WS_STATUS } from "services/websocket/WebSocketTypes";

export interface IReconnectionService {
  connect: () => void;
  status: BehaviorSubject<WS_STATUS>;
}

export type ReconnectionService = {
  init: (connectionService: IReconnectionService) => void;
  reconnect: () => void;
  forceReconnect: () => void;
  start: () => void;
  stop: () => void;
};
