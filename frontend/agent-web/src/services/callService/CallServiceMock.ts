import AgentModel from "data/user/AgentModel";

const tempDelay = (f, time) => {
  setTimeout(() => {
    f();
  }, time);
};

class CallServiceMock {
  private static _instance: CallServiceMock;
  private _lead;
  private _agent: AgentModel;

  constructor() {
    console.warn("Mock Service");
    if (CallServiceMock._instance) {
      throw new Error("Call Service already created use instance instead");
    }

    CallServiceMock._instance = this;
  }

  public static getInstance(): CallServiceMock {
    if (!CallServiceMock._instance) {
      new CallServiceMock();
    }

    return CallServiceMock._instance;
  }

  public setAgent(agent: AgentModel): void {
    this._agent = agent;
  }

  public connect(): void {
    if (!this._agent) {
      return;
    }

    const waitForCall = () => {
      this._lead = {
        id: 1,
        clientId: 338,
        dataSourceId: 1,
        duplicateOfId: null,
        phone: "123123123",
        lastUpdateTime: "2022-12-29T13:35:34",
        externalId: "externalIdTest1",
        firstName: "TEMP",
        lastName: "TEMP",
        languageCode: null,
        countryCode: null,
        status: 51,
        lastTimeOnline: null,
        registrationTime: "2022-12-29T13:35:34",
        assignedUserId: null,
        isFixedAssigned: false,
        firstDepositTime: null,
        remindOn: null,
        webRtcEndpointUrl:
          "wss://qa.sip.bridge.kollink.ai:443/ws?session=91896b5d-9202-46e3-89d0-922d7b8f035e&agent=1",
      };
      this._agent.setLead(this._lead);
      this.call();
    };

    tempDelay(waitForCall, 1000);
  }

  private async call(): Promise<void> {
    const disconnect = () => {
      this._agent.startFillFeedback();
    };

    tempDelay(disconnect, 5000000);
  }

  stop(): void {
    console.warn("--stop---");
  }
}

export default CallServiceMock;
