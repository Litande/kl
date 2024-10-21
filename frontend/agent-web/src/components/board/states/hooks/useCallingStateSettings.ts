import { useEffect, useRef, useState } from "react";

import { SettingsModel } from "data/user/SettingsModel";
import { AgentStatus } from "data/user/types";
import AgentModel from "data/user/AgentModel";

const useCallingStateSettings = (agent: AgentModel, callTime: number) => {
  const [isEndCallButtonEnable, setIsEndCallButtonEnable] = useState(false);
  const [isVoicemailButtonEnable, setIsVoicemailButtonEnable] = useState(false);
  const settingsRef = useRef<SettingsModel>(null);

  const handleEndCallButton = () => {
    const { endCallButtonAfter } = settingsRef.current;
    if (endCallButtonAfter >= 0) {
      if (callTime >= endCallButtonAfter) {
        setIsEndCallButtonEnable(true);
      }
    } else {
      setIsEndCallButtonEnable(true);
    }
  };

  const handleShowVoicemailButton = () => {
    const { hideVoicemailButtonAfter, showVoicemailButtonAfter } = settingsRef.current;
    if (hideVoicemailButtonAfter !== showVoicemailButtonAfter && showVoicemailButtonAfter !== 0) {
      if (callTime >= showVoicemailButtonAfter) {
        setIsVoicemailButtonEnable(true);
      }
    } else {
      setIsVoicemailButtonEnable(true);
    }
  };

  const handleHideVoicemailButton = () => {
    const { hideVoicemailButtonAfter, showVoicemailButtonAfter } = settingsRef.current;
    if (hideVoicemailButtonAfter !== showVoicemailButtonAfter && hideVoicemailButtonAfter !== 0) {
      if (callTime >= hideVoicemailButtonAfter) {
        setIsVoicemailButtonEnable(true);
      }
    }
  };

  useEffect(() => {
    const settingsSubscribe = agent?.settings.subscribe(settings => {
      settingsRef.current = settings;
    });
    const statusSubscribe = agent?.status.subscribe(status => {
      if (status === AgentStatus.InTheCall) {
        handleEndCallButton();
        handleShowVoicemailButton();
        handleHideVoicemailButton();
      }
    });
    return () => {
      settingsSubscribe?.unsubscribe();
      statusSubscribe?.unsubscribe();
    };
  }, [callTime]);

  return {
    isEndCallButtonEnable,
    isVoicemailButtonEnable,
  };
};

export default useCallingStateSettings;
