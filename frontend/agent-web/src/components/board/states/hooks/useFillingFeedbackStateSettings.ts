import { useEffect, useRef, useState } from "react";
import { SettingsModel } from "data/user/SettingsModel";
import AgentModel from "data/user/AgentModel";

const useFillingFeedbackStateSettings = (agent: AgentModel) => {
  const [isCallAgainButtonEnable, setIsCallAgainButtonEnable] = useState(false);
  const settingsRef = useRef<SettingsModel>(null);

  useEffect(() => {
    const settingsSubscribe = agent?.settings.subscribe(settings => {
      settingsRef.current = settings;
    });
    const currentRedialsLimitCounterSubscribe = agent?.currentRedialsLimitCounter.subscribe(
      counter => {
        if (!settingsRef.current.redialsLimit) {
          setIsCallAgainButtonEnable(true);
        } else if (counter > 0) {
          setIsCallAgainButtonEnable(true);
        } else {
          setIsCallAgainButtonEnable(false);
        }
      }
    );
    return () => {
      settingsSubscribe?.unsubscribe();
      currentRedialsLimitCounterSubscribe?.unsubscribe();
    };
  }, []);

  return {
    isCallAgainButtonEnable,
  };
};

export default useFillingFeedbackStateSettings;
