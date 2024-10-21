import { useState } from "react";
import moment from "moment";

import useInterval from "hooks/useInterval";
import Time from "utils/time";

export default function useTimer({
  timestampStart,
  autoStart = false,
  timestampFinish = moment().valueOf(),
}) {
  const [passedSeconds, setPassedSeconds] = useState(
    Time.getSecondsFromPrevTime(timestampStart, true, timestampFinish) || 0
  );

  const [prevTime, setPrevTime] = useState(moment().valueOf());
  const [seconds, setSeconds] = useState(
    passedSeconds + Time.getSecondsFromPrevTime(prevTime || 0, true)
  );
  const [isRunning, setIsRunning] = useState(autoStart);

  useInterval(
    () => {
      setSeconds(passedSeconds + Time.getSecondsFromPrevTime(prevTime, true));
    },
    isRunning ? 1000 : null
  );

  function start() {
    const newPrevTime = moment().valueOf();
    setPrevTime(newPrevTime);
    setIsRunning(true);
    setSeconds(passedSeconds + Time.getSecondsFromPrevTime(newPrevTime, true));
  }

  function pause() {
    setPassedSeconds(seconds);
    setIsRunning(false);
  }

  function reset(offset = 0, newAutoStart = true) {
    const newPassedSeconds = Time.getSecondsFromExpiry(offset, true) || 0;
    const newPrevTime = moment().valueOf();
    setPrevTime(newPrevTime);
    setPassedSeconds(newPassedSeconds);
    setIsRunning(newAutoStart);
    setSeconds(newPassedSeconds + Time.getSecondsFromPrevTime(newPrevTime, true));
  }

  return {
    ...Time.getTimeFromSeconds(seconds),
    start,
    pause,
    reset,
    isRunning,
  };
}
