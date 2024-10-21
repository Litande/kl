import React from "react";

import DefaultFlag from "components/flag/icons/DefaultFlag";
import { FlagIcon, CountryCode } from "components/flag/FlagIcon";

type Props = { iso3: string };

const CountryFlag: React.FC<Props> = props => {
  const countryCode = props.iso3 as CountryCode;

  const flag = FlagIcon(countryCode) ?? <DefaultFlag />;

  return flag;
};

export default CountryFlag;
