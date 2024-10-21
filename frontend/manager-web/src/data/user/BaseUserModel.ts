import { ROLES } from "router/enums";
import { User } from "./types";
import ConnectionModel from "services/websocket/ConnectionModel";
import { STATISTIC_WS, STATISTICS_ENDPOINT, TRACKING_WS } from "services/websocket/const";
import { createBaseURL } from "services/api";
import CallService from "services/callService/CallService";

class BaseUserModel {
  private _firstName: string;
  private _lastName: string;
  private _role: ROLES | null;
  private _trackingWS: ConnectionModel;
  private _statisticWS: ConnectionModel;
  private _iceServices: string[];
  constructor(data: User) {
    this._firstName = data.firstName;
    this._lastName = data.lastName;
    this._role = data.role;
    this._iceServices = data.iceServices;
    console.warn("BaseUserModel initialized");
    CallService.getInstance().setStuns(this._iceServices);

    this._trackingWS = new ConnectionModel(createBaseURL() + "/" + TRACKING_WS, this.onError);
    this._trackingWS.connect();
    this._statisticWS = new ConnectionModel(
      createBaseURL(STATISTIC_WS) + "/" + STATISTICS_ENDPOINT,
      this.onError
    );
    this._statisticWS.connect();
  }

  private onError(error: any): void {
    console.error(error);
  }

  get firstName(): string {
    return this._firstName;
  }

  get lastName(): string {
    return this._lastName;
  }

  get role(): ROLES | null {
    return this._role;
  }

  get fullName(): string {
    return `${this._firstName} ${this._lastName}`;
  }

  get trackingWS(): ConnectionModel {
    return this._trackingWS;
  }

  get statisticWS(): ConnectionModel {
    return this._statisticWS;
  }

  logout(): void {
    this.destroy();
  }

  destroy(): void {
    this._trackingWS.removeCurrentWS();
    this._statisticWS.removeCurrentWS();
    this._trackingWS = null;
    this._statisticWS = null;

    this._firstName = null;
    this._lastName = null;
    this._role = null;
  }
}

export default BaseUserModel;
