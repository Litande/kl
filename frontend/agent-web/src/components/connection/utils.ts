export const isConnectionOnline = () =>
  typeof navigator !== "undefined" && typeof navigator.onLine === "boolean"
    ? navigator.onLine
    : false;

export const NO_INTERNET_CONNECTION_ERROR = new Error(
  "You disconnected, please check you network connections."
);
