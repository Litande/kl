import { ROUTES } from "../src/router/enums";

const dataTestAttr = "data-testid";

export const getId = id => `[${dataTestAttr}="${id}"]`;

export const existOnPageById = id =>
  cy.window().then($window => $window.document.querySelector(`[${dataTestAttr}="${id}"]`));

export const login = () => {
  const { login, password } = Cypress.env();

  cy.visit("/");
  cy.get(getId("email")).clear().type(login);
  cy.get(getId("password")).clear().type(password);

  getLoginButton().click();
};

export const logOutIfDialing = () => {
  getNavigationLink(ROUTES.DIALING).click();

  return existOnPageById("clock-in-button").then(clickInButton => {
    if (!clickInButton) {
      cy.wait(15000);

      existOnPageById("end-call-button").then(endCallButton => {
        if (endCallButton) {
          getEndCallButton().click();

          existOnPageById("submit-feedback-button").then(submitFeedbackButton => {
            if (submitFeedbackButton) {
              getSubmitFeedbackButton().click();
              clickOnUserMenuLogOut();
            } else {
              clickOnUserMenuLogOut();
            }
          });
        } else {
          existOnPageById("submit-feedback-button").then(submitFeedbackButton => {
            if (submitFeedbackButton) {
              getSubmitFeedbackButton().click();
              clickOnUserMenuLogOut();
            } else {
              clickOnUserMenuLogOut();
            }
          });
        }
      });
    }
  });
};

export const clickOnUserMenuLogOut = () => {
  cy.get(getId("user-menu")).click();
  cy.get(getId("menu-item-log-out")).click();
};

export const getNavigationLink = route => cy.get(`a[href="/${route === "/" ? "" : route}"]`);

export const getClockInButton = () => cy.get(getId("clock-in-button"));

export const getDialButton = () => cy.get(getId("kl-button"));

export const getManualWidgetPhone = () => cy.get(getId("manual-widget-phone"));

export const getPhoneNumber = () => cy.get(getId("phone-number"));

export const getEndCallButton = () => cy.get(getId("end-call-button"));

export const getSubmitFeedbackButton = () => cy.get(getId("submit-feedback-button"));

export const getPageTitle = () => cy.get(getId("page-title"));

export const getLoginButton = () => cy.get(getId("login-button"));

export const getClockOutButton = () => cy.get(getId("clock-out-button"));
