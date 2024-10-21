// read more about micromatch https://github.com/micromatch/micromatch
// read about build in rules you can read here https://github.com/DukeLuo/eslint-plugin-check-file/blob/main/lib/constants/naming-convention.js
const pascalCase = "*([A-Z]*([a-z0-9]))";
const camelCase = "+([a-z])*([A-Z]*([a-z0-9]))";
const allowCamelCaseReactHooks = "use*([a-z])*([A-Z]*([a-z0-9]))";
module.exports = {
  extends: [
    "react-app",
    "eslint:recommended",
    "plugin:react/recommended",
    "plugin:react-hooks/recommended",
    "plugin:@typescript-eslint/recommended",
    "plugin:jest/recommended",
    "plugin:prettier/recommended",
    "plugin:storybook/recommended",
  ],
  plugins: ["react", "@typescript-eslint", "jest", "check-file", "import-newlines"],
  env: {
    browser: true,
    es6: true,
    jest: true,
  },
  globals: {
    Atomics: "readonly",
    SharedArrayBuffer: "readonly",
    window: "readonly",
  },
  parser: "@typescript-eslint/parser",
  parserOptions: {
    ecmaFeatures: {
      jsx: true,
    },
    ecmaVersion: 2018,
    sourceType: "module",
    project: "./tsconfig.json",
    tsconfigRootDir: __dirname,
  },
  rules: {
    "max-len": [
      1,
      {
        code: 120,
        ignoreComments: true,
        ignoreUrls: true,
        ignoreStrings: true,
      },
    ],
    "linebreak-style": "off",
    "check-file/folder-match-with-fex": [
      "error",
      {
        "*.test.{js,jsx,ts,tsx}": "**/__test__/",
      },
    ],
    "check-file/filename-naming-convention": [
      "error",
      {
        "**/*.tsx": `(index|${pascalCase}|${allowCamelCaseReactHooks})`,
        "**/*.{ts}": `(index|${camelCase})`,
      },
    ],
    "check-file/folder-naming-convention": [
      "error",
      {
        "src/**/": `(__test__|${camelCase})`,
        "mocks/*/": "CAMEL_CASE",
      },
    ],
    "react/react-in-jsx-scope": "off",
    "react/jsx-pascal-case": [
      2,
      {
        allowAllCaps: false,
        allowLeadingUnderscore: false,
        allowNamespace: false,
      },
    ],
    "react/jsx-max-props-per-line": "off",
    "@typescript-eslint/naming-convention": [
      "error",
      {
        selector: "interface",
        format: ["PascalCase"],
        prefix: ["I"],
      },
      {
        selector: "variable",
        types: ["boolean"],
        format: ["PascalCase"],
        prefix: ["is", "should", "has", "can", "did", "will"],
      },
    ],
    "react/jsx-filename-extension": [
      2,
      {
        extensions: [".tsx"],
      },
    ],
    "prettier/prettier": [
      "error",
      {
        endOfLine: "auto",
      },
    ],
  },
};
