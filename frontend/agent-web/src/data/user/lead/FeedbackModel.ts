import { FEEDBACK_TIME_DEFAULT } from "data/user/lead/types";
import { ITickTimeout, timerService } from "services/timerService/TimerService";
import { BehaviorSubject } from "rxjs";

type ConstructorType = {
  feedbackTime?: number;
  timeoutCallback?: () => void;
  callFinishAt?: string;
};

class FeedbackModel {
  private _feedbackTime: number;
  private _secondsForFeedback: number;
  private _callTime: number;
  private _timeout: ITickTimeout;
  private _timeToEnd: BehaviorSubject<number>;
  private _timeoutCallback: () => void;

  constructor(data: ConstructorType) {
    const callFinishedTimeDate = data.callFinishAt
      ? new Date(data.callFinishAt).getTime()
      : new Date().getTime();

    this._feedbackTime =
      (data.feedbackTime || FEEDBACK_TIME_DEFAULT) -
      Math.round((new Date().getTime() - callFinishedTimeDate) / 1000);
    if (this._feedbackTime < 0) {
      this._feedbackTime = 0;
    }

    this._timeToEnd = new BehaviorSubject<number>(this._feedbackTime);
    this._timeoutCallback = data.timeoutCallback;

    this.tick = this.tick.bind(this);
    this.endFeedback = this.endFeedback.bind(this);
    this.clearCurrentTimeout = this.clearCurrentTimeout.bind(this);
  }

  startFeedback(callTime: number): void {
    this._callTime = callTime;
    if (this._secondsForFeedback) return;

    this._secondsForFeedback = FEEDBACK_TIME_DEFAULT;
    const { addTickTimeout } = timerService();
    this.clearCurrentTimeout();
    this._timeout = addTickTimeout(this._feedbackTime * 1000, this.tick, this.endFeedback);
  }

  private endFeedback(): void {
    this._timeToEnd.next(0);
    this._timeoutCallback();
  }

  private tick(timer?: ITickTimeout): void {
    this._timeToEnd.next(this._timeToEnd.getValue() - 1);
  }

  save(): void {
    console.warn("--save--");
  }

  get timeToEnd(): BehaviorSubject<number> {
    return this._timeToEnd;
  }

  get timeCall(): number {
    return this._callTime;
  }

  private clearCurrentTimeout(): void {
    const { removeTimeout } = timerService();
    removeTimeout(this._timeout);
    this._timeout = null;
  }

  get availableMaxTimeForFeedback(): number {
    return this._feedbackTime;
  }

  destroy(): void {
    console.warn("FeedbackModel::destroyed");
    this.clearCurrentTimeout();
  }
}

export default FeedbackModel;
