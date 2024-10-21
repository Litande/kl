import i18next, { i18n as i18nInstance } from "i18next";
import { initReactI18next } from "react-i18next";
import HttpApi from "i18next-http-backend";

import { languages, LOCALE_STORAGE_KEY, namespaces } from "localization/constants";
import { getValueFromEnum } from "utils/checkTypes";

const localePath = process.env.REACT_APP_LOCALE_PATH;
const initLang = getValueFromEnum(
  localStorage.getItem(LOCALE_STORAGE_KEY),
  languages,
  languages.en_US
);

const createI18n = (language: string): i18nInstance => {
  const i18n = i18next.createInstance().use(initReactI18next);

  i18n.use(HttpApi).init({
    backend: {
      crossDomain: true,
      // TODO add specific domain (host)
      loadPath: `./${localePath}/{{lng}}/translation.json`, // Specify where backend will find translation files.
    },
    lng: language,
    fallbackLng: language,
    ns: namespaces.translation,
    interpolation: {
      escapeValue: false,
    },
    parseMissingKeyHandler(key: string, defaultValue?: string): string {
      return "[INVALID KEY]:" + key;
    },
  });

  return i18n;
};

export const i18n = createI18n(initLang);
