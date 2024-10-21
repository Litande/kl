import styled from "styled-components";
import Input, { StyledInput } from "components/input/Input";
import React from "react";
import { CountryType, ICallHourItem } from "./types";
import { TData, Divider } from "./Common";
import { getCallingHours } from "./helpers";
import moment from "moment-timezone";

type Props = {
  country: CountryType;
  timezone: {
    name: string;
    offset: number;
  };
  callHour: ICallHourItem;
};
const TimezoneItem = ({ timezone: { name, offset }, country, callHour }: Props) => {
  const regions = name.split("/");
  const region = regions[regions.length - 1];

  const { from, till } = getCallingHours({
    country,
    callHour,
    timezone: { name, offset },
  });

  const gmt = moment().tz(name).utcOffset() / 60;

  return (
    <TimezoneRow>
      <TimezoneTData colSpan={2}>
        <TimezoneContainer>
          <TimezoneLabel>Timezone</TimezoneLabel>
          <TimezoneInput
            value={`GMT ${gmt > 0 ? "+" : ""}${gmt} (${region}, ${country.name})`}
            disabled
          />
        </TimezoneContainer>
      </TimezoneTData>
      <TData colSpan={4}></TData>
      <TData>
        <TimeInput value={from} disabled />
      </TData>
      <TData>
        <Divider />
      </TData>
      <TData>
        <TimeInput value={till} disabled />
      </TData>
      <TData></TData>
      <TData></TData>
    </TimezoneRow>
  );
};

export default TimezoneItem;

const TimezoneTData = styled(TData)`
  padding-left: 2rem;
`;

const TimezoneContainer = styled.div`
  display: flex;
  align-items: center;
  justify-content: space-between;
  padding-right: 0.5rem;
`;

const TimezoneLabel = styled.span`
  ${({ theme }) => theme.typography.body3};
  color: ${({ theme }) => theme.colors.fg.secondary_light};
`;

const TimezoneInput = styled(Input)`
  width: calc(100% - 75px);
  background: ${({ theme }) => theme.colors.bg.ternary};

  ${StyledInput} {
    &[disabled] {
      background: ${({ theme }) => theme.colors.bg.ternary};
    }
  }
`;

const TimezoneRow = styled.tr`
  background-color: #f3f8ff;
`;

const TimeInput = styled(Input)`
  background: ${({ theme }) => theme.colors.bg.ternary};
`;
