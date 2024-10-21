import { createContext, ReactElement } from "react";
import useOptions from "hooks/useOptions";

export const SettingItemContext = createContext({
  countries: [],
  statuses: [],
});

export const SettingsProvider = ({ children }: { children: ReactElement }) => {
  const options = useOptions({ withStatuses: true, withCountries: true });

  return <SettingItemContext.Provider value={options}>{children}</SettingItemContext.Provider>;
};
