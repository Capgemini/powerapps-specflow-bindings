module.exports = {
  parser: "@typescript-eslint/parser",
  parserOptions: {
    ecmaVersion: 2020,
    sourceType: "module",
    project: ['./tsconfig.json'],
  },
  plugins: [
    'jasmine'
  ],
  extends: [
    'plugin:jasmine/recommended',
    'airbnb-typescript/base',
  ],
  env: {
    jasmine: true,
  },
  globals: {
    "Xrm": "readonly",
    "window": "readonly",
    "fetch": "readonly",
  },
  rules: {
  },
};