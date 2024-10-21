import { getUniqueId } from "./TimerServiceUtils";

export type ITimeout = {
  id: string;
  endTime: number;
  callback: () => void;
};

let mainInterval: NodeJS.Timer;
let timeouts: Array<ITimeout>;

const TICK_VALUE = 1000;

export const timerService = () => {
  if (!timeouts) {
    timeouts = new Array<ITimeout>();
  }

  const clearMainInterval = () => {
    if (mainInterval) {
      clearInterval(mainInterval);
    }
  };

  const handleTimeout = () => {
    if (!timeouts.length) {
      return;
    }

    const currentTime = Date.now();

    if (currentTime >= timeouts.at(-1).endTime) {
      const timeout = timeouts.pop();
      timeout?.callback();
      handleTimeout();
    }
  };

  const startTimerService = () => {
    clearMainInterval();

    mainInterval = setInterval(handleTimeout, TICK_VALUE);
  };

  const addTimeout = (time: number, callback: () => void) => {
    const timeout: ITimeout = {
      id: getUniqueId(),
      endTime: Date.now() + time,
      callback,
    };
    timeouts.push(timeout);

    if (timeouts.length > 1) {
      timeouts.sort((a, b) => b.endTime - a.endTime);
    }

    return timeout;
  };

  const removeTimeout = (timeout: ITimeout) => {
    if (!timeout) {
      return;
    }
    timeouts = timeouts.filter(item => item.id !== timeout.id);
  };

  const stopTimerService = () => {
    clearMainInterval();
    timeouts = [];
  };

  return {
    startTimerService,
    stopTimerService,
    addTimeout,
    removeTimeout,
  };
};
