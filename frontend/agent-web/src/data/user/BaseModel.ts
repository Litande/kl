import { AgentStatus } from "data/user/types";
import { TOKEN_KEY } from "./AuthContext";

export class BaseModel {
  protected _agentRtcUrl: string;
  protected _phoneNumber: string;
  protected _statusBeforeCall: AgentStatus;

  constructor(data) {
    this._agentRtcUrl = data.agentRtcUrl;
    this._phoneNumber = data.phone;
  }

  // eslint-disable-next-line @typescript-eslint/no-empty-function
  public updateTimeAnswerAt(): void {}

  get agentRtcUrl(): string {
    return this._agentRtcUrl
      ? `${this._agentRtcUrl}&token=${localStorage.getItem(TOKEN_KEY)}`
      : null;
  }
  get phoneNumber(): string {
    return this._phoneNumber;
  }
  set statusBeforeCall(value: AgentStatus) {
    this._statusBeforeCall = value;
  }
  get nextStatusAfterCall(): AgentStatus {
    return this._statusBeforeCall;
  }
  public destroy(): void {
    console.log("destroy");
  }
}

export default BaseModel;
