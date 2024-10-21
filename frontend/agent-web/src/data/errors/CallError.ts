class CallError extends Error {
  readonly code: string | number;
  readonly message: string;
  constructor(code, message) {
    super();
    this.code = code;
    this.message = message;
  }
}

export default CallError;
