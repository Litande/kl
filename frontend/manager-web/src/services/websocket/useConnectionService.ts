import useUser from "data/user/useUser";
import { useMemo } from "react";
import { STATISTIC_WS, TRACKING_WS } from "services/websocket/const";

export const useConnectionService = (host: string) => {
  const user = useUser();
  const { trackingWS, statisticWS } = user;
  const curWS = useMemo(() => {
    if (host !== TRACKING_WS && host !== STATISTIC_WS) throw new Error("Unknown host");
    return host === TRACKING_WS ? trackingWS : statisticWS;
  }, [host]);
  return {
    send: curWS.send,
    status: curWS.status,
    addChannelListener: curWS.addChannelListener,
    removeChannelListener: curWS.removeChannelListener,
    unsubscribeWS: curWS.unsubscribeWS,
    invoke: curWS.invoke,
  };
};
