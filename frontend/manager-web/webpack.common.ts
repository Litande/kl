import path from "path";

import { Configuration as WebpackConfiguration } from "webpack";
import { Configuration as WebpackDevServerConfiguration } from "webpack-dev-server";
import Dotenv from "dotenv-webpack";
import TerserPlugin from "terser-webpack-plugin";
import HtmlWebpackPlugin from "html-webpack-plugin";
import CopyPlugin from "copy-webpack-plugin";

interface IConfiguration extends WebpackConfiguration {
  devServer?: WebpackDevServerConfiguration;
}

export const ENV_FILE = process.env.NODE_ENV_FILE || "./.env";
const DEV_PORT = process.env.DEV_PORT || "3021";

const DIST_PATH = "build";

const config: IConfiguration = {
  mode: "development",
  output: {
    path: path.resolve(__dirname, DIST_PATH),
    clean: true,
  },
  entry: {
    main: "index.tsx",
  },
  optimization: {
    minimize: true,
    minimizer: [
      new TerserPlugin({
        extractComments: false,
        terserOptions: {
          format: {
            comments: false,
          },
        },
      }),
    ],
  },
  resolve: {
    extensions: [".tsx", ".ts", ".js"],
    modules: [path.resolve(__dirname, "src"), "node_modules"],
  },
  plugins: [
    new Dotenv({
      path: ENV_FILE,
    }),
    new HtmlWebpackPlugin({
      template: "public/index.html",
      favicon: "public/favicon.ico",
    }),
    new CopyPlugin({
      patterns: [
        {
          from: "public/locales",
          to: "locales",
        },
      ],
    }),
  ],
  module: {
    rules: [
      {
        test: /\.(ts|js)x?$/i,
        exclude: /node_modules/,
        use: {
          loader: "babel-loader",
          options: {
            presets: [
              "@babel/preset-env",
              ["@babel/preset-react", { runtime: "automatic" }],
              "@babel/preset-typescript",
            ],
          },
        },
      },
      {
        test: /\.(png|jpe?g|gif|ico)$/i,
        use: [
          {
            loader: "file-loader",
            options: {
              name: "images/[name].[ext]",
            },
          },
        ],
      },
      {
        test: /\.(svg)$/i,
        type: "asset/resource",
        generator: {
          filename: "assets/images/[name][ext][query]",
        },
      },
      {
        test: /\.(ttf)$/i,
        type: "asset/resource",
        generator: {
          filename: "assets/fonts/[name][ext][query]",
        },
      },
      {
        test: /\.css$/,
        use: ["style-loader", "css-loader"],
      },
    ],
  },
};

export default config;
