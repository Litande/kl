import { getUniqueId } from "./TimerServiceUtils";

export type ITimeout = {
  id: string;
  endTime: number;
  callback: () => void;
};

export type ITickTimeout = ITimeout & {
  countTick: number;
  isPaused: boolean;
  tickCallback: (timeout?: ITickTimeout) => void;
};

let mainInterval: NodeJS.Timer;
let timeouts: Array<ITimeout>;
let tickTimeouts: Array<ITickTimeout>;

const TICK_VALUE = 1000;

export const timerService = () => {
  if (!timeouts) {
    timeouts = new Array<ITimeout>();
    tickTimeouts = new Array<ITickTimeout>();
  }

  const clearMainInterval = () => {
    if (mainInterval) {
      clearInterval(mainInterval);
    }
  };

  const checkTimeout = () => {
    const currentTime = Date.now();
    const timeout = timeouts.at(-1);
    if (timeout && currentTime >= timeout.endTime && timeout.endTime !== 0) {
      timeout?.callback();
      removeTimeout(timeout);
      checkTimeout();
    }
  };

  const checkTickTimeout = () => {
    if (!tickTimeouts.length) {
      return;
    }

    tickTimeouts.forEach(timeout => {
      if (!timeout.isPaused) {
        timeout.countTick += 1;
        timeout.tickCallback(timeout);
      }
    });
  };

  const handleTimeout = () => {
    if (!timeouts.length) {
      return;
    }
    checkTimeout();
    checkTickTimeout();
  };

  const startTimerService = () => {
    clearMainInterval();

    mainInterval = setInterval(handleTimeout, TICK_VALUE);
  };

  const addTickTimeout = (
    time: number,
    tickCallback: (timeout?: ITickTimeout) => void,
    callback?: () => void
  ) => {
    // finish immediately
    if (time <= 0) {
      callback();
      return;
    }
    const tickTimeout: ITickTimeout = {
      ...addTimeout(time, callback),
      isPaused: false,
      tickCallback: tickCallback,
      countTick: 0,
    };

    tickTimeouts.push(tickTimeout);
    return tickTimeout;
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
    tickTimeouts = tickTimeouts.filter(item => item.id !== timeout.id);
  };

  const stopTimerService = () => {
    clearMainInterval();
  };

  return {
    startTimerService,
    stopTimerService,
    addTimeout,
    addTickTimeout,
    removeTimeout,
  };
};
