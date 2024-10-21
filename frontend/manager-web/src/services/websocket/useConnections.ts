import { useEffect } from "react";
import { useConnectionService } from "services/websocket/useConnectionService";

export type IConnection = {
  chanelName: string;
  onMessage: (data) => void;
};

const useConnections = (host: string, chanels: Array<IConnection>, deps = []) => {
  const { addChannelListener, removeChannelListener } = useConnectionService(host);

  useEffect(() => {
    chanels.forEach(chanel => {
      addChannelListener(chanel.chanelName, chanel.onMessage);
    });
    return () => {
      chanels.forEach(chanel => {
        removeChannelListener(chanel.chanelName, chanel.onMessage);
      });
    };
    // eslint-disable-next-line react-hooks/exhaustive-deps
  }, [...deps, addChannelListener, removeChannelListener]);
};

export default useConnections;
