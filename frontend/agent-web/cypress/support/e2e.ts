// ***********************************************************
// This example support/e2e.ts is processed and
// loaded automatically before your test files.
//
// This is a great place to put global configuration and
// behavior that modifies Cypress.
//
// You can change the location of this file or turn off
// automatically serving support files with the
// 'supportFile' configuration option.
//
// You can read more here:
// https://on.cypress.io/configuration
// ***********************************************************

// Import commands.js using ES2015 syntax:
import "./commands";
import { clickOnUserMenuLogOut, existOnPageById, login, logOutIfDialing } from "../utils";

// Alternatively you can use CommonJS syntax:
// require('./commands')

beforeEach(() => {
  cy.log("beforeEach is calling from support/e2e.ts");
  login();
  logOutIfDialing().then(() => {
    existOnPageById("login-button").then(loginButton => {
      if (loginButton) {
        login();
      }
    });
  });
});

afterEach(() => {
  cy.log("afterEach is calling  from support/e2e.ts");
  existOnPageById("user-menu").then(userMenu => {
    if (userMenu) {
      clickOnUserMenuLogOut();
    }
  });
});
