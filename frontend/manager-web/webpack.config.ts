import { merge } from "webpack-merge";
import config from "./webpack.common";

const DEV_PORT = process.env.DEV_PORT || "3021";

const mainConfig = merge(config, {
  mode: "development",
  devServer: {
    historyApiFallback: true,
    port: DEV_PORT,
    open: true,
    hot: true,
  },
  devtool: "eval",
});

export default mainConfig;
