import { useAgent } from "data/user/useAgent";
import { useEffect, useState } from "react";
import { AgentStatus, CallType } from "data/user/types";
import { Subscription } from "rxjs";

const useAgentStatus = () => {
  const { agent } = useAgent();
  const [curStatus, setCurStatus] = useState(
    agent && agent.status ? agent.status.getValue() : null
  );
  const [callType, setCallType] = useState<CallType>(
    agent && agent.callType ? agent.callType.getValue() : null
  );
  const [updater, setUpdater] = useState(-1);
  const [nextAgentStatus, setNextAgentStatus] = useState(agent?.nextStatusAfterCall.getValue());

  useEffect(() => {
    let subscribe: Subscription;
    let updaterSubscribe: Subscription;
    let callTypeSubscribe: Subscription;
    let nextStatus: Subscription;
    if (agent) {
      callTypeSubscribe = agent.callType.subscribe(value => {
        setCallType(value);
      });
      subscribe = agent.status.subscribe(value => {
        setCurStatus(value);
      });
      updaterSubscribe = agent.updater.subscribe(value => {
        setUpdater(value);
      });
      nextStatus = agent.nextStatusAfterCall.subscribe(value => {
        setNextAgentStatus(value);
      });
    }
    return () => {
      subscribe && subscribe.unsubscribe();
      updaterSubscribe && updaterSubscribe.unsubscribe();
      callTypeSubscribe && callTypeSubscribe.unsubscribe();
      nextStatus && nextStatus.unsubscribe();
    };
  }, [agent]);

  return {
    agentStatus: curStatus,
    isAgentOffline: curStatus === AgentStatus.Offline,
    isManualCallAvailable: curStatus === AgentStatus.Offline || curStatus === AgentStatus.Break,
    updater,
    callType,
    nextAgentStatus,
  };
};

export default useAgentStatus;
