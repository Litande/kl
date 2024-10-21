import { Settings } from "./types";

export class SettingsModel {
  private _endCallButtonAfterThisAmountOfSeconds = 0;
  private _hideVoicemailButtonAfterThisAmountOfSecondsOfCall = 0;
  private _maxCallDuration = 0;
  private _pageTimeout = 0;
  private _redialsLimit = 0;
  private _showVoicemailButtonAfterThisAmountOfSecondsOfCall = 0;

  constructor(data?: Settings) {
    if (!data || (data && Object.keys(data).length === 0)) {
      return;
    }
    this._endCallButtonAfterThisAmountOfSeconds = Number.parseInt(
      data.endCallButtonAfterThisAmountOfSeconds
    );
    this._hideVoicemailButtonAfterThisAmountOfSecondsOfCall = Number.parseInt(
      data.hideVoicemailButtonAfterThisAmountOfSecondsOfCall
    );
    this._maxCallDuration = data.maxCallDuration;
    this._pageTimeout = data.pageTimeout;
    this._redialsLimit = data.redialsLimit;
    this._showVoicemailButtonAfterThisAmountOfSecondsOfCall = Number.parseInt(
      data.showVoicemailButtonAfterThisAmountOfSecondsOfCall
    );
  }

  get endCallButtonAfter() {
    return this._endCallButtonAfterThisAmountOfSeconds;
  }

  get hideVoicemailButtonAfter() {
    return this._hideVoicemailButtonAfterThisAmountOfSecondsOfCall;
  }

  get maxCallDuration() {
    return this._maxCallDuration;
  }

  get pageTimeout() {
    return this._pageTimeout;
  }

  get redialsLimit() {
    return this._redialsLimit;
  }

  get showVoicemailButtonAfter() {
    return this._showVoicemailButtonAfterThisAmountOfSecondsOfCall;
  }
}
