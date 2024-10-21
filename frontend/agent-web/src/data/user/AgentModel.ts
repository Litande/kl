import { BehaviorSubject } from "rxjs";

import { ROLES } from "router/enums";
import { Agent, AgentStatus, Break, CallType } from "./types";
import LeadModel from "data/user/lead/LeadModel";
import CallService from "services/callService/CallService";
import agentApi from "services/api/agentApi";
import { HangupReason } from "services/callService/type";
import { checkEnumType } from "utils/checkTypes";
import CallError from "data/errors/CallError";
import { timerService } from "services/timerService/TimerService";
import ManualModel from "data/user/manual/ManualModel";
import BaseModel from "data/user/BaseModel";
import BreakModel from "./BreakModel";
import { SettingsModel } from "./SettingsModel";
import { isConnectionOnline, NO_INTERNET_CONNECTION_ERROR } from "components/connection/utils";

class AgentModel {
  private _id: number;
  private _firstName: string;
  private _lastName: string;
  private _email: string;
  private _role: ROLES | null;
  private _iceServers: string[];
  private _status: BehaviorSubject<AgentStatus>;
  private _authToken?: string;
  private _curLead: LeadModel;
  private _manualLead: ManualModel;
  private _temporaryError: BehaviorSubject<CallError>;
  private _callType: BehaviorSubject<CallType>;
  private _updater: BehaviorSubject<number>;
  private _callActions: Map<CallType, (data) => void>;
  private _isCallMuted: BehaviorSubject<boolean>;
  private _breakModel: BreakModel;
  private _settings: BehaviorSubject<SettingsModel>;
  private _currentRedialsLimitCounter: BehaviorSubject<number>;
  private _isAuth: BehaviorSubject<boolean>;
  private _nextStatusAfterCall: BehaviorSubject<AgentStatus>;

  constructor(user?: Agent) {
    console.warn("AgentModel::constructor");
    this._id = -1;
    this._status = new BehaviorSubject<AgentStatus>(AgentStatus.Unknown);
    this._isAuth = new BehaviorSubject<boolean>(false);
    this._callType = new BehaviorSubject<CallType>(null);
    this._isCallMuted = new BehaviorSubject(false);
    this._temporaryError = new BehaviorSubject<CallError>(null);
    this._updater = new BehaviorSubject<number>(0);
    this._currentRedialsLimitCounter = new BehaviorSubject(0);
    this._nextStatusAfterCall = new BehaviorSubject<AgentStatus>(AgentStatus.WaitingForTheCall);
    this.generateActions();

    this.timeoutFeedback = this.timeoutFeedback.bind(this);
    this.timeoutBreak = this.timeoutFeedback.bind(this);

    this._breakModel = new BreakModel({ endBreakCallback: this.timeoutBreak });

    CallService.getInstance().setAgent(this);
  }

  public init(user: Agent): void {
    this._id = user.id;
    this._firstName = user.firstName;
    this._lastName = user.lastName;
    this._role = user.role;
    this._isCallMuted.next(false);
    this._callType.next(null);
    this._status.next(AgentStatus.Unknown);
    this._authToken = user.authToken;
    this._isAuth.next(true);
    this._iceServers = user.iceServers;
    CallService.getInstance().connect(this._iceServers);

    this.initSettings();
  }

  private generateActions(): void {
    this._callActions = new Map<CallType, (data) => void>();
    this.setLead = this.setLead.bind(this);
    this.setManual = this.setManual.bind(this);
    this._callActions.set(CallType.Predictive, this.setLead);
    this._callActions.set(CallType.Manual, this.setManual);
  }

  public setInfo(data): void {
    const callType = data.callType;
    if (checkEnumType(callType, CallType)) {
      this._callType.next(callType);
      const action = this._callActions.get(callType);
      if (action) {
        action(data);
      }
      this.resetRedialLimit(callType);
      this._updater.next(this._updater.getValue() + 1);
    }
  }

  private resetRedialLimit(callType): void {
    if (callType === CallType.Predictive) {
      this._currentRedialsLimitCounter.next(this._settings.getValue().redialsLimit);
    }
  }

  public break(): void {
    console.warn("BREAK");
    this._breakModel.activateBreak();
    this._status.next(AgentStatus.Break);
    CallService.getInstance().sendChangeStatus(AgentStatus.Break);
  }

  public updateTimeAnswerAt(): void {
    this.getActiveLead()?.updateTimeAnswerAt();
  }

  public scheduleBreak(item: Break): void {
    this._breakModel.scheduleBreak(item);
  }

  public unScheduleBreak(id: number): void {
    this._breakModel.unScheduleBreak(id);
  }

  private getCurrentStatus(): AgentStatus {
    const curStatus = this._status.getValue();
    const isUnknownStatus = curStatus === AgentStatus.Unknown;
    const statusBeforeCall = isUnknownStatus
      ? this.hasBreaks()
        ? AgentStatus.Break
        : AgentStatus.Offline
      : curStatus;
    return statusBeforeCall;
  }

  private setManual(data): void {
    // if we reload page, should init by data
    this._isCallMuted.next(false);
    this._manualLead = new ManualModel(data);
    this._manualLead.statusBeforeCall = this.getCurrentStatus();
    this._callType.next(CallType.Manual);
    this._nextStatusAfterCall.next(this.getCurrentStatus());
    this._manualLead.updateData(data);
  }

  public setLead(data): void {
    this._curLead = new LeadModel(data);
  }

  public hold(): void {
    this._curLead?.hold();
  }

  public setCurrentStatus(status: AgentStatus): void {
    if (checkEnumType(status, AgentStatus)) {
      console.warn("Agent:next status:", status);
      const currentStatus = this._status.getValue();

      if (currentStatus !== AgentStatus.Break && status === AgentStatus.Break) {
        this._breakModel.activateBreak(true);
      }
      this._status.next(status);
      if (status === AgentStatus.Offline) {
        this._callType.next(null);
        this.clearLeadsInfo();
      }
    }
  }

  public getActiveLead(): BaseModel {
    if (this._curLead) return this._curLead;
    if (this._manualLead) return this._manualLead;
    return null;
  }

  public tryCall(): void {
    console.warn("try call");
    const lead: BaseModel = this.getActiveLead();
    if (lead && lead.agentRtcUrl) {
      this._isCallMuted.next(false);
      CallService.getInstance().call();
      this._status.next(AgentStatus.InTheCall);
    } else {
      console.warn("Call unavailable, state should be restored");
      this.restoreActionByStatus();
    }
  }

  public waitForCall(isApi = true): void {
    console.warn("WAIT FOR A CALL");
    // usually next status should be WaitingForTheCall
    this._status.next(AgentStatus.WaitingForTheCall);
    this._nextStatusAfterCall.next(AgentStatus.WaitingForTheCall);
    this.clearLeadsInfo();
    isApi && CallService.getInstance().sendChangeStatus(AgentStatus.WaitingForTheCall);
  }

  public finishBreak() {
    this._breakModel.finishBreak();
    this.waitForCall();
  }

  public endCallByAgent(): void {
    this._callType.next(null);
    this._curLead?.setFinishCallTime(new Date().toUTCString());
    this.startFillFeedback();
    CallService.getInstance().hangupCall(HangupReason.empty, "");
  }

  public endManualCall(): void {
    this.clearManual();
    CallService.getInstance().hangupCall(HangupReason.empty, "");
  }

  // end of call (close peer reason)
  public startFillFeedback(): void {
    console.warn("FILLING FEEDBACK");
    this._callType.next(null);
    this._curLead?.startFeedback(this.timeoutFeedback, this._settings.getValue().pageTimeout);
  }

  public async startManualCall(phoneNumber) {
    if (isConnectionOnline()) {
      const curStatus = this._status.getValue();
      const isUnknownStatus = curStatus === AgentStatus.Unknown;
      if (curStatus === AgentStatus.Offline || curStatus === AgentStatus.Break || isUnknownStatus) {
        this._isCallMuted.next(false);
        CallService.getInstance().startManualCall(phoneNumber);
      }
    } else {
      this.addTemporaryError(null, NO_INTERNET_CONNECTION_ERROR.message);
    }
  }

  public addTemporaryError(code, message) {
    if (this._temporaryError?.getValue() === null) {
      this._temporaryError.next(new CallError(code, message));
      timerService().addTimeout(5000, () => this._temporaryError.next(null));
    }
  }

  private restoreActionByStatus(): void {
    if (this._status.getValue() === AgentStatus.FillingFeedback) {
      if (this._callType.getValue() === CallType.Manual) {
        this.setCurrentStatus(this._nextStatusAfterCall.getValue());
        this._callType.next(null);
      }
      if (this._callType.getValue() === CallType.Predictive) {
        this.startFillFeedback();
      }
    }
  }

  public gotoNext = () => {
    this.clearLeadsInfo();
    if (this.hasBreaks()) {
      this.break();
    } else {
      this._status.next(this._nextStatusAfterCall.getValue());
      CallService.getInstance().sendChangeStatus(this._nextStatusAfterCall.getValue());
    }
  };

  private timeoutFeedback = () => {
    this.gotoNext();
  };

  private hasBreaks(): boolean {
    return Boolean(this.breaksModel.getNextActiveBreak());
  }

  private timeoutBreak = () => {
    // this.waitForCall();
  };

  public fillFeedback(): void {
    this._curLead.destroy();
  }

  private clearLeadsInfo(): void {
    this.getActiveLead()?.destroy();
    this._curLead = null;
    this._manualLead = null;
  }

  public gotoOffline(): void {
    this._callType.next(null);
    this._nextStatusAfterCall.next(AgentStatus.Offline);
  }

  get id(): number {
    return this._id;
  }

  public voiceMail(): void {
    CallService.getInstance().hangupCall(HangupReason.voicemail, "");
    this._callType.next(null);
    this.clearLeadsInfo();
    this.gotoNext();
  }

  public noAnswer(): void {
    CallService.getInstance().hangupCall(HangupReason.na, "");
    this._callType.next(null);
    this.clearLeadsInfo();
    this.gotoNext();
  }

  public async saveFeedback({ leadId, sessionId, leadStatus, remindOn, comment }): Promise<void> {
    await agentApi.filledcall({
      leadId,
      sessionId,
      leadStatus,
      remindOn,
      comment,
    });

    this.clearLeadsInfo();
    this.gotoNext();
  }

  get breaksModel(): BreakModel {
    return this._breakModel;
  }

  get lead(): LeadModel | null {
    return this._curLead;
  }

  get role(): ROLES | null {
    return this._role;
  }

  get callType(): BehaviorSubject<CallType> {
    return this._callType;
  }

  get status(): BehaviorSubject<AgentStatus> {
    return this._status;
  }

  get isCallMuted(): BehaviorSubject<boolean> {
    return this._isCallMuted;
  }

  get authToken(): string {
    return this._authToken;
  }

  get email(): string {
    return this._email;
  }

  get firstName(): string {
    return this._firstName;
  }

  get lastName(): string {
    return this._lastName;
  }

  get isAuth(): BehaviorSubject<boolean> {
    return this._isAuth;
  }

  get temporaryError(): BehaviorSubject<CallError> {
    return this._temporaryError;
  }

  get stringify(): string {
    return "";
  }

  get updater(): BehaviorSubject<number> {
    return this._updater;
  }

  private clearAll(): void {
    this.clearLead();
    this.clearManual();
  }

  private clearLead(): void {
    if (this._curLead) {
      this._curLead.destroy();
    }
    this._curLead = null;
  }

  private clearManual(): void {
    this._manualLead && this._manualLead.destroy();
    this._manualLead = null;
  }

  private clearAllData = () => {
    this._id = -1;

    this._firstName = "";
    this._lastName = "";
    this._role = null;
    this._isCallMuted.next(false);
    this._isCallMuted.complete();
    this._temporaryError.next(null);
    this._temporaryError.complete();
    this._authToken = "user.authToken";
    this._callType.next(null);
    this._callType.complete();
    this._breakModel.destroy();
    this._isAuth.next(false);
  };

  public destroy() {
    const preStatus = this._status.getValue();
    this._status.next(AgentStatus.Unknown);
    preStatus !== AgentStatus.Offline && CallService.getInstance().logout();
    this.clearLead();
    this.clearManual();
    this.clearAllData();
    CallService.getInstance().destroy();
  }

  public muteManualCall(): void {
    CallService.getInstance().mute();
    this._isCallMuted.next(true);
  }

  public unmuteManualCall(): void {
    CallService.getInstance().unmute();
    this._isCallMuted.next(false);
  }

  private async initSettings() {
    this._settings = new BehaviorSubject(new SettingsModel());

    const data = await agentApi.getSettings().then(({ data }) => data);
    this._settings.next(new SettingsModel(data));
  }

  public get settings(): BehaviorSubject<SettingsModel> {
    return this._settings;
  }

  private isCanCallAgain(): boolean {
    return this._settings.getValue().redialsLimit === 0;
  }

  public callAgain(): void {
    if (this.isCanCallAgain() || this._currentRedialsLimitCounter.getValue() > 0) {
      this._currentRedialsLimitCounter.next(this._currentRedialsLimitCounter.getValue() - 1);
      CallService.getInstance().callAgain();
      this.waitForCall(false);
    }
  }

  public get nextStatusAfterCall(): BehaviorSubject<AgentStatus> {
    return this._nextStatusAfterCall;
  }

  public get currentRedialsLimitCounter(): BehaviorSubject<number> {
    return this._currentRedialsLimitCounter;
  }
}

export default AgentModel;
