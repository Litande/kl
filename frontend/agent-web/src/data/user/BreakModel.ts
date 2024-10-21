import { ITickTimeout, timerService } from "services/timerService/TimerService";
import { BehaviorSubject } from "rxjs";
import moment from "moment";
import { Break } from "./types";

type Breaks = {
  [id: string]: Break;
};

const initialBreaks: Breaks = {
  "1": {
    id: 1,
    duration: 10,
    label: "Break 10 minutes",
    isAvailable: true,
    isScheduled: false,
    isActive: false,
    isUsed: false,
    startedAt: null,
  },
  "2": {
    id: 2,
    duration: 10,
    label: "Break 10 minutes",
    isAvailable: true,
    isScheduled: false,
    isActive: false,
    isUsed: false,
    startedAt: null,
  },
  "3": {
    id: 3,
    duration: 60,
    label: "Break 60 minutes",
    isAvailable: true,
    isScheduled: false,
    isActive: false,
    isUsed: false,
    startedAt: null,
  },
};

class BreakModel {
  private _breaks: BehaviorSubject<Breaks>;

  private _breakTimeout: ITickTimeout;
  private _breakTimeToEnd: BehaviorSubject<number>;
  private _endBreakCallback: () => void;

  constructor({ endBreakCallback }) {
    const savedBreaks = JSON.parse(localStorage.getItem("breaks")) || {};
    const now = moment();
    const breaks = initialBreaks;
    Object.values(initialBreaks).forEach(item => {
      const savedItem = savedBreaks[item?.id];

      if (savedItem) {
        const startDate = moment(savedItem.startedAt);
        const isStartedToday = savedItem.startedAt && now.isSame(startDate, "day");

        const isActive = startDate.add(item.duration, "minutes").diff(now) > 0;

        breaks[item.id] = {
          ...breaks[item.id],
          startedAt: savedItem.startedAt,
          isActive: startDate ? isActive : false,
          isScheduled: savedItem.isScheduled || (startDate ? isActive : false),
          isAvailable: isStartedToday ? Boolean(savedBreaks[item?.id]?.isAvailable) : true,
        };
      }
    });

    this._breaks = new BehaviorSubject(breaks);
    this._breakTimeToEnd = new BehaviorSubject(0);

    this.tick = this.tick.bind(this);
    this.clearBreakTimer = this.clearBreakTimer.bind(this);
    this._endBreakCallback = endBreakCallback;
  }

  getNextActiveBreak = () => {
    const breaks: Break[] = Object.values(this._breaks.getValue());
    breaks.sort((a, b) => a.duration - b.duration);
    return breaks.find(item => item.isScheduled && !item.isUsed);
  };

  cleanNextActiveBreak = ({ id }) => {
    this.updateBreak(id, {
      isScheduled: false,
      isAvailable: false,
      isActive: false,
    });
  };

  markAsUsedBreak = ({ id }) => {
    this.updateBreak(id, {
      isScheduled: false,
      isAvailable: false,
      isActive: false,
      isUsed: true,
    });
  };

  finishBreak = () => {
    const breaks: Break[] = Object.values(this._breaks.getValue());

    const activeBreak = breaks.find(({ isActive }) => isActive);

    if (activeBreak) {
      this.cleanNextActiveBreak(activeBreak);
      this.markAsUsedBreak(activeBreak);
    }
  };

  private updateBreak = (id, newItem) => {
    const breaks: Breaks = this._breaks.getValue();
    breaks[id] = {
      ...breaks[id],
      ...newItem,
    };
    this.updateBreaks(breaks);
  };

  private updateBreaks(breaks) {
    localStorage.setItem("breaks", JSON.stringify(breaks));
    this._breaks.next(breaks);
  }

  scheduleBreak = (breakItem: Break) => {
    this.updateBreak(breakItem.id, { isScheduled: true });
  };

  unScheduleBreak = id => {
    this.updateBreak(id, { isScheduled: false });
  };

  get breaks(): BehaviorSubject<Breaks> {
    return this._breaks;
  }

  private clearBreakTimer(breakItem): void {
    this._breakTimeToEnd.next(0);
    this.cleanNextActiveBreak(breakItem);
    this.activateBreak();
  }

  getDuration = ({ id, startedAt, duration }, isPast) => {
    if (!isPast) {
      return duration * 60;
    }

    const now = moment();
    const startDate = moment(startedAt);
    const diff = startDate.add(duration, "minutes").diff(now, "minutes", true);

    return diff > 0 ? Math.ceil(diff) * 60 : 0;
  };

  activateBreak = (isPast = false) => {
    const breakItem = this.getNextActiveBreak();

    if (!breakItem) return;

    const { addTickTimeout } = timerService();
    const duration = this.getDuration(breakItem, isPast);

    this.updateBreak(breakItem.id, {
      isActive: true,
      isScheduled: true,
      isAvailable: false,
      startedAt: isPast ? breakItem.startedAt : moment().toDate(),
    });

    this._breakTimeToEnd.next(duration);
    this._breakTimeout = addTickTimeout(duration * 1000, this.tick, () => {
      this.clearBreakTimer(breakItem);
      this.markAsUsedBreak(breakItem);
    });
  };

  private tick() {
    this._breakTimeToEnd.next(this._breakTimeToEnd.getValue() - 1);
  }

  get breakTimeToEnd(): BehaviorSubject<number> {
    return this._breakTimeToEnd;
  }

  destroy(): void {
    const { removeTimeout } = timerService();
    removeTimeout(this._breakTimeout);
    console.warn("BreakModel::Destroy");
  }
}

export default BreakModel;
