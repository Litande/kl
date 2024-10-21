import { clickOnUserMenuLogOut, getClockInButton, getLoginButton } from "../utils";

describe("auth spec", () => {
  it("login should works", function () {
    getClockInButton().should("exist");
  });

  it("logout should works", function () {
    getClockInButton().should("exist");
    clickOnUserMenuLogOut();
    getLoginButton().should("exist");
  });
});
