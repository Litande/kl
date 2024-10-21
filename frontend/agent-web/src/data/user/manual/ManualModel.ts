import BaseModel from "data/user/BaseModel";
import { AgentStatus } from "data/user/types";

class ManualModel extends BaseModel {
  constructor(data) {
    super(data);
  }

  public updateData(data): void {
    this._agentRtcUrl = data.agentRtcUrl;
  }

  get nextStatusAfterCall(): AgentStatus {
    return this._statusBeforeCall || AgentStatus.Offline;
  }

  public destroy(): void {
    super.destroy();
  }
}

export default ManualModel;
