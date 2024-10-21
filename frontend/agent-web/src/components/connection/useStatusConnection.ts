import { useEffect, useState } from "react";

import { CONNECTION_STATUS } from "components/connection/types";
import { WS_STATUS } from "services/websocket/WebSocketTypes";
import CallService from "services/callService/CallService";
import useOfflineStatus from "components/connection/useOfflineStatus";

const useStatusConnection = () => {
  const [status, setStatus] = useState<CONNECTION_STATUS>(CONNECTION_STATUS.GOOD_CONNECTION);
  const [wsStatus, setWsStatus] = useState(WS_STATUS.CLOSED);
  const isOffline = useOfflineStatus();

  useEffect(() => {
    if (wsStatus === WS_STATUS.CLOSED) {
      setStatus(CONNECTION_STATUS.NO_CONNECTION);
    }
    if (wsStatus === WS_STATUS.CONNECTED) {
      setStatus(CONNECTION_STATUS.STABLE_CONNECTION);
    }
  }, [wsStatus]);

  useEffect(() => {
    const subscribe = CallService.getInstance()
      .getConnectionStatus()
      .subscribe(value => {
        setWsStatus(value);
      });
    return () => {
      subscribe.unsubscribe();
    };
  }, []);

  useEffect(() => {
    setStatus(isOffline ? CONNECTION_STATUS.NO_CONNECTION : CONNECTION_STATUS.STABLE_CONNECTION);
  }, [isOffline]);

  return { status };
};

export default useStatusConnection;
