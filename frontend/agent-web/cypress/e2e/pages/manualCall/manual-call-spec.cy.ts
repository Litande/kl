import {
  getId,
  getNavigationLink,
  getClockInButton,
  getDialButton,
  getManualWidgetPhone,
  getPhoneNumber,
  logOutIfDialing,
} from "../../../utils";
import { ROUTES } from "../../../../src/router/enums";

describe("manual spec", () => {
  it("should do manual call if agent offline", function () {
    const { phoneNumber } = Cypress.env();
    getNavigationLink(ROUTES.MANUAL_CALL).click();
    getDialButton().should("exist");
    getPhoneNumber().clear().type(phoneNumber);
    getDialButton().click();
    getManualWidgetPhone().should("have.text", phoneNumber);
    cy.get(getId("manual-widget-end-call-button")).click();
    getManualWidgetPhone().should("not.exist");
  });

  it("shouldn't do manual call agent online", function () {
    const { phoneNumber } = Cypress.env();
    getNavigationLink(ROUTES.DIALING).click();
    getClockInButton().click();
    getNavigationLink(ROUTES.MANUAL_CALL).click();
    getPhoneNumber().clear().type(phoneNumber);
    getDialButton().should("be.disabled");
    getManualWidgetPhone().should("not.exist");
    logOutIfDialing();
  });
});
