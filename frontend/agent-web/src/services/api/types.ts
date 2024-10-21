export type BaseApiError = {
  errorText: string;
  errorCode: number;
};

export type BaseApiResponse = {
  data: any;
  error: BaseApiError | null;
};
