import { useCallback, useEffect, useState } from "react";
import CallService from "services/callService/CallService";

const useMute = () => {
  const [isMute, setIsMute] = useState(false);

  const toggleMute = useCallback(() => {
    isMute ? CallService.getInstance().unmute() : CallService.getInstance().mute();
  }, [isMute]);

  useEffect(() => {
    const subscribe = CallService.getInstance().isCallMuted.subscribe(value => setIsMute(value));
    return () => {
      subscribe.unsubscribe();
    };
  }, []);

  return {
    isMute,
    toggleMute,
  };
};

export default useMute;
