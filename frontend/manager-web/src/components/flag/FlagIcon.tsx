import React, { Suspense } from "react";
import { FLAG_MAP } from "components/flag/consts";

export type CountryCode = keyof typeof FLAG_MAP;

export function FlagIcon(countryCode?: CountryCode): React.ReactElement | null {
  if (!countryCode || !FLAG_MAP[countryCode]) {
    return null;
  }

  const Flag = FLAG_MAP[countryCode];

  if (!Flag) {
    return null;
  }

  if (typeof window === "undefined") {
    return <div style={{ height: 36, width: 36 }} />;
  }

  return (
    <Suspense fallback={<div style={{ height: 36, width: 36 }} />}>
      <Flag />
    </Suspense>
  );
}
