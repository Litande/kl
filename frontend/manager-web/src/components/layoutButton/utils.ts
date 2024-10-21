import { getLocalStorageItem, setLocalStorageItem } from "utils/localStorage";
import { Layout } from "./types";

const LOCAL_STORAGE_KEY = "tracking_page_layout";

export const getDefaultLayout = () =>
  parseInt(getLocalStorageItem(LOCAL_STORAGE_KEY) || Layout.Three);

export const saveLayout = index => setLocalStorageItem(LOCAL_STORAGE_KEY, index);
