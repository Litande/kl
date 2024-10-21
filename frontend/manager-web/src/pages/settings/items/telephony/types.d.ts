export interface ITelephonyItem {
  phoneNumber: string;
  country: string;
}

export interface ITelephony {
  telephones: Array<ITelephonyItem>;
}
