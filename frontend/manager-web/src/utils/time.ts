import moment from "moment";

export default class Time {
  private static formatTimeToDoubles(time) {
    return time.toString().padStart(2, "0");
  }
  static getTimeFromSeconds(secs) {
    const totalSeconds = Math.ceil(secs);
    const days = Math.floor(totalSeconds / (60 * 60 * 24));
    const hours = Math.floor((totalSeconds % (60 * 60 * 24)) / (60 * 60));
    const minutes = Math.floor((totalSeconds % (60 * 60)) / 60);
    const seconds = Math.floor(totalSeconds % 60);
    const total = `${this.formatTimeToDoubles(hours)}:${this.formatTimeToDoubles(
      minutes
    )}:${this.formatTimeToDoubles(seconds)}`;

    return {
      seconds,
      minutes,
      hours,
      days,
      total,
    };
  }

  static getSecondsFromExpiry(expiry, shouldRound) {
    const now = moment().valueOf();
    const milliSecondsDistance = expiry - now;
    if (milliSecondsDistance > 0) {
      const val = milliSecondsDistance / 1000;
      return shouldRound ? Math.round(val) : val;
    }
    return 0;
  }

  static getSecondsFromPrevTime(prevTime, shouldRound, timeStop = moment().valueOf()) {
    const milliSecondsDistance = timeStop - prevTime;
    if (milliSecondsDistance > 0) {
      const val = milliSecondsDistance / 1000;
      return shouldRound ? Math.round(val) : val;
    }
    return 0;
  }

  static getSecondsFromTimeNow() {
    const now = moment();
    const currentTimestamp = now.valueOf();
    const offset = now.utcOffset() * 60;
    return currentTimestamp / 1000 - offset;
  }
}
