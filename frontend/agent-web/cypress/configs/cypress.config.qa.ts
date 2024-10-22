import { defineConfig } from "cypress";
import { cypressBrowserPermissionsPlugin } from "cypress-browser-permissions";

export default defineConfig({
  video: false,
  screenshotOnRunFailure: false,
  e2e: {
    baseUrl: "https://qa.agent.kollink.ai/",

    env: {
      login: "autotest@mailsac.com",
      password: "1234567890",
      phoneNumber: "12345",

      browserPermissions: {
        microphone: "allow",
      },
    },

    setupNodeEvents(on, config) {
      // implement node event listeners here
      config = cypressBrowserPermissionsPlugin(on, config);
      //
      // Any existing plugins you are using
      //
      return config;
    },
  },
});
