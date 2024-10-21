import { useCallback, useEffect, useState } from "react";
import { useAgent } from "data/user/useAgent";

const useMute = () => {
  const { agent } = useAgent();
  const [isMute, setIsMute] = useState(false);

  const toggleMute = useCallback(() => {
    isMute ? agent.unmuteManualCall() : agent.muteManualCall();
  }, [agent, isMute]);

  useEffect(() => {
    if (!agent) {
      return;
    }
    const subscribe = agent.isCallMuted.subscribe(value => setIsMute(value));
    return () => {
      subscribe && subscribe.unsubscribe();
    };
  }, [agent]);

  return {
    isMute,
    toggleMute,
  };
};

export default useMute;
