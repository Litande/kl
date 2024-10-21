import { merge } from "webpack-merge";

import config from "./webpack.common";

const mainConfig = merge(config, {
  mode: "production",
  entry: {
    main: "index.tsx",
  },
});

export default mainConfig;
