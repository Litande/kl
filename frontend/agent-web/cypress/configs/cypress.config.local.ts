import { defineConfig } from "cypress";
import { cypressBrowserPermissionsPlugin } from "cypress-browser-permissions";

export default defineConfig({
  video: false,
  screenshotOnRunFailure: false,
  e2e: {
    baseUrl: "http://localhost:3022/",
    experimentalStudio: true,

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
      // eslint-disable-next-line @typescript-eslint/no-var-requires
      //
      // Any existing plugins you are using
      //
      return config;
    },
  },

  component: {
    devServer: {
      framework: "react",
      bundler: "webpack",
    },
  },
});
