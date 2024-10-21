import { useEffect, useState } from "react";

import { CONNECTION_STATUS } from "components/connection/types";
import { WS_STATUS } from "services/websocket/WebSocketTypes";
import useOfflineStatus from "components/connection/useOfflineStatus";
import useUser from "data/user/useUser";

const useStatusConnection = () => {
  const user = useUser();
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
    let subscribe;
    try {
      subscribe = user?.trackingWS?.status.subscribe(value => {
        setWsStatus(value);
      });
    } catch (e) {
      console.warn(e);
    }

    return () => {
      subscribe?.unsubscribe();
    };
    // eslint-disable-next-line react-hooks/exhaustive-deps
  }, []);

  useEffect(() => {
    setStatus(isOffline ? CONNECTION_STATUS.NO_CONNECTION : CONNECTION_STATUS.STABLE_CONNECTION);
  }, [isOffline]);

  return { status };
};

export default useStatusConnection;
