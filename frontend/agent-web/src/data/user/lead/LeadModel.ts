import { DialerLead, FEEDBACK_TIME_DEFAULT } from "data/user/lead/types";
import FeedbackModel from "data/user/lead/FeedbackModel";
import { BehaviorSubject } from "rxjs";
import { ITickTimeout, timerService } from "services/timerService/TimerService";
import { AgentStatus, LeadStatus } from "data/user/types";
import commonApi from "services/api/commonApi";
import BaseModel from "data/user/BaseModel";
import leadsApi from "services/api/leads";

class LeadModel extends BaseModel {
  readonly id: number;
  private _clientId: number;
  private _dataSourceId: number;
  private _externalId: string;
  private _status: string;
  private _lastUpdateTime: string;
  private _registrationTime: string;
  private _callFinishAt: string;
  private _agentAnsweredAt: string;
  private _leadAnsweredAt: string;
  private _firstName: string;
  private _lastName: string;
  private _callTime: BehaviorSubject<number>;
  private _feedback: FeedbackModel;
  private _timeout: ITickTimeout;
  private _iframeUrl: string;
  private _sessionId: string;
  private _availableStatuses: Array<LeadStatus> | null;
  private _comment: BehaviorSubject<string>;

  // TODO add fields from https://plat4me.atlassian.net/browse/kl-257
  constructor(data: DialerLead) {
    super(data);
    this.id = data.id;
    this._clientId = data.clientId;
    this._dataSourceId = data.dataSourceId;
    this._externalId = data.externalId;
    this._status = data.status;
    this._lastUpdateTime = data.lastUpdateTime;
    this._registrationTime = data.registrationTime;
    this._agentAnsweredAt = data.agentAnsweredAt;
    this._callFinishAt = data.callFinishAt;
    this._leadAnsweredAt = data.leadAnsweredAt;
    this._firstName = data.firstName;
    this._lastName = data.lastName;
    this._callTime = new BehaviorSubject<number>(this.getCallTimeInSeconds());
    this._iframeUrl = data.iframeUrl;
    this._sessionId = data.sessionId;
    this._availableStatuses =
      data?.availableStatuses.map((value: string) => {
        return { label: value, value: value };
      }) || [];

    const { addTickTimeout } = timerService();
    this.tick = this.tick.bind(this);
    this._timeout = addTickTimeout(NaN, this.tick);
    this._comment = new BehaviorSubject<string>("");
  }

  public updateTimeAnswerAt(): void {
    this._callTime.next(this.getCallTimeInSeconds());
  }

  public getCallTimeInSeconds(): number {
    const lastDateUpdate = this._callFinishAt ? new Date(this._callFinishAt) : new Date();

    const answeredTime = this._leadAnsweredAt ? new Date(this._leadAnsweredAt) : new Date();

    let res = Math.round((lastDateUpdate.getTime() - new Date(answeredTime).getTime()) / 1000);

    if (res < 0) res = 0;
    return res > 0 ? res : 0;
  }

  public setFinishCallTime(value: string): void {
    this.clearTimeout();
    this._callFinishAt = value;
    this._callTime.next(this.getCallTimeInSeconds());
  }

  private tick(): void {
    this._callTime.next(this._callTime.getValue() + 1);
  }

  isHold(): boolean {
    return this._timeout.isPaused;
  }

  hold(): void {
    this._timeout.isPaused = !this._timeout.isPaused;
  }

  startFeedback(timeoutCallback: () => void, feedbackTime = FEEDBACK_TIME_DEFAULT): void {
    if (!this._feedback) {
      this._feedback = new FeedbackModel({
        timeoutCallback,
        callFinishAt: this._callFinishAt,
        feedbackTime,
      });
    }
    this._feedback.startFeedback(this._callTime.getValue());
  }

  public async fetchStatuses(): Promise<void> {
    const { data } = await commonApi.getStatuses();
    this._availableStatuses = data;
  }

  get name(): string {
    return this._lastName;
  }

  get clientId(): number {
    return this._clientId;
  }

  get dataSourceId(): number {
    return this._dataSourceId;
  }

  get externalId(): string {
    return this._externalId;
  }

  set status(status) {
    this._status = status;
  }

  get status(): string {
    return this._status;
  }

  get phone(): string {
    return this.phoneNumber;
  }

  get registrationTime(): string {
    return this._registrationTime;
  }

  get callTime(): BehaviorSubject<number> {
    return this._callTime;
  }
  get nextStatusAfterCall(): AgentStatus {
    return AgentStatus.FillingFeedback;
  }

  get feedback(): FeedbackModel {
    return this._feedback;
  }

  get iframeUrl(): string {
    return this._iframeUrl;
  }

  get sessionId(): string {
    return this._sessionId;
  }

  get availableStatuses(): Array<LeadStatus> | null {
    return this._availableStatuses;
  }

  private clearTimeout(): void {
    const { removeTimeout } = timerService();
    removeTimeout(this._timeout);
  }

  public async setComment(comment): Promise<void> {
    leadsApi.saveComment(this.id, comment);
    this._comment.next(comment);
  }

  public get comment(): BehaviorSubject<string> {
    return this._comment;
  }

  destroy(): void {
    super.destroy();
    this._feedback?.destroy();
    this._callTime.complete();
    this.clearTimeout();
    console.warn("LEAD_MODEL::DESTROY");
  }
}

export default LeadModel;
