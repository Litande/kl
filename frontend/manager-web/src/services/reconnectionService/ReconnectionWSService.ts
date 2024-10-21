import { IReconnectionService, ReconnectionService } from "services/reconnectionService/types";
import { MAX_ATTEMPTS, RECONNECT_DELAY, WS_STATUS } from "services/websocket/WebSocketTypes";
import { Subscription } from "rxjs";
let connection: IReconnectionService;
let attempts = MAX_ATTEMPTS;
let timeoutValue;
let statusSubscription: Subscription = null;

const ReconnectionWSService = (): ReconnectionService => {
  const onChange = value => {
    console.warn("ReconnectionService::onChange", connection.status.getValue());
    if (connection.status?.getValue() === WS_STATUS.CONNECTED) {
      attempts = MAX_ATTEMPTS;
    }
    if (connection.status.getValue() === WS_STATUS.CLOSED) {
      reconnect();
    }
  };

  const init = (connectionService: IReconnectionService) => {
    console.warn("ReconnectionService::init");
    connection = connectionService;
  };

  function reconnect() {
    console.warn("ReconnectionService::reconnect", attempts);
    if (connection.status.getValue() === WS_STATUS.CLOSED) {
      if (timeoutValue) {
        clearInterval(timeoutValue);
      }
      timeoutValue = setTimeout(() => {
        attempts -= 1;
        connection.connect();
      }, RECONNECT_DELAY);
    }
  }

  function forceReconnect() {
    console.warn("ReconnectionService::forceReconnect");
    if (connection.status.getValue() !== WS_STATUS.CONNECTED) {
      attempts = MAX_ATTEMPTS;
      connection.connect();
    }
  }

  const start = () => {
    console.warn("ReconnectionService::start", statusSubscription);
    if (statusSubscription === null) {
      statusSubscription = connection.status.subscribe(value => {
        onChange(value);
      });
    }
  };

  const stop = () => {
    console.warn("ReconnectionService::stop");
    statusSubscription?.unsubscribe();
    statusSubscription = null;
    if (timeoutValue) {
      clearInterval(timeoutValue);
    }
    timeoutValue = null;
  };

  return {
    init,
    stop,
    start,
    reconnect,
    forceReconnect,
  };
};

export default ReconnectionWSService;
