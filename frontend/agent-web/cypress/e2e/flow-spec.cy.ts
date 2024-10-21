import {
  getClockInButton,
  getClockOutButton,
  getEndCallButton,
  getNavigationLink,
  getSubmitFeedbackButton,
} from "../utils";
import { ROUTES } from "../../src/router/enums";

describe("auth spec", () => {
  it("predictive call", () => {
    getClockInButton().click();
    cy.wait(15000);
    getEndCallButton().click();
    getNavigationLink(ROUTES.DASHBOARD).click();
    getClockOutButton().click();
    getNavigationLink(ROUTES.DIALING).click();
    getSubmitFeedbackButton().click();
    getNavigationLink(ROUTES.DASHBOARD).click();
    getClockInButton().should("exist");
  });
});
