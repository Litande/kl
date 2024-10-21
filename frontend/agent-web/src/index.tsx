import React from "react";

import "localization/configuration";
import GlobalStyles from "./globalStyles";
import App from "App";

import { createRoot } from "react-dom/client";

const container = document.getElementById("dialerAgentApp");
const root = createRoot(container);
root.render(
  <React.StrictMode>
    <React.Suspense fallback={null}>
      <GlobalStyles />
      <App />
    </React.Suspense>
  </React.StrictMode>
);
