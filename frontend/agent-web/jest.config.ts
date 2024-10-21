import type { Config } from "@jest/types";

const config: Config.InitialOptions = {
  rootDir: "./src/",
  verbose: true,
  preset: "ts-jest",
  testEnvironment: "node",
  transform: {
    "^.+\.tsx?$": "ts-jest"
  },
  moduleFileExtensions: ["js", "ts", "tsx"],
  moduleDirectories: ["node_modules", "src"],
};

export default config;
