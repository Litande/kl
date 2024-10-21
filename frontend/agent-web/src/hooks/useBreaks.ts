import { useAgent } from "data/user/useAgent";
import { useEffect, useState } from "react";
import { Break } from "data/user/types";

const useAgentStatus = () => {
  const { agent } = useAgent();
  const [activeBreakTimer, setActiveBreakTimer] = useState(null);
  const [breaks, setBreaks] = useState<Break[]>(
    Object.values(agent.breaksModel?.breaks?.getValue())
  );

  useEffect(() => {
    const subscribe = agent.breaksModel?.breaks?.subscribe(value => {
      setBreaks(Object.values(value));
    });

    return () => {
      subscribe?.unsubscribe();
    };
  }, []);

  useEffect(() => {
    const subscribe = agent.breaksModel?.breakTimeToEnd?.subscribe(value => {
      setActiveBreakTimer(value);
    });

    return () => {
      subscribe?.unsubscribe();
    };
  }, [breaks]);

  return {
    activeBreakTimer,
    breaks,
  };
};

export default useAgentStatus;
