/* eslint-disable react-hooks/exhaustive-deps */

import { useEffect } from "react";
import { useTranslation } from "react-i18next";

import eventDomDispatcher from "services/events/EventDomDispatcher";
import { languages } from "i18n/i18n.const";
import { getValueFromEnum } from "utils/checkTypes";

export const TP_CHANGE_LOCALE_EVENT = "TP_CHANGE_LOCALE_EVENT";

const useLocaleHelper = () => {
  const { i18n } = useTranslation();
  const { addEventListener, removeEventListener } = eventDomDispatcher();

  const updateLocale = (event: CustomEvent) => {
    const newLang = getValueFromEnum(event.detail.locale.toLowerCase(), languages, i18n.language);
    if (newLang !== i18n.language) {
      i18n.changeLanguage(newLang);
    }
  };

  useEffect(() => {
    addEventListener(TP_CHANGE_LOCALE_EVENT, updateLocale);
    return () => {
      removeEventListener(TP_CHANGE_LOCALE_EVENT, updateLocale);
    };
  }, []);
};

export default useLocaleHelper;
