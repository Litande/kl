import { CountryType, ICallHourItem } from "./types";
import moment from "moment-timezone";
import { IOption } from "components/multiSelect/MultiSelect";

type CallingHoursProps = {
  country: CountryType;
  callHour: ICallHourItem;
  timezone: {
    name: string;
    offset: number;
  };
};
type GetCallingHours = (props: CallingHoursProps) => { from: string; till: string };

export const getCallingHours: GetCallingHours = ({ country, callHour, timezone }) => {
  if (!callHour.country || !timezone) {
    return { till: callHour.till, from: callHour.from };
  }

  const offset = country.offset
    ? country.offset * 60
    : moment.tz.zonesForCountry(country.name, true)[0].offset;

  const [fromHours, fromMinutes] = callHour.from.split(":");
  const [tillHours, tillMinutes] = callHour.till.split(":");

  const from = moment()
    .set("minutes", Number(fromMinutes))
    .set("hours", Number(fromHours))
    .add(timezone.offset - offset, "minutes");

  const till = moment()
    .set("minutes", Number(tillMinutes))
    .set("hours", Number(tillHours))
    .add(timezone.offset - offset, "minutes");

  return {
    from: moment(from).format("HH:mm"),
    till: moment(till).format("HH:mm"),
  };
};

export const getCountriesOptions = options => {
  return options.countries.reduce((acc, country) => {
    const timezones = moment.tz.zonesForCountry(country.value, true) || [];

    const offsetToTimezone = timezones.reduce((res, { name, offset }) => {
      res[offset] = name;
      return res;
    }, {});

    const offsets = Object.keys(offsetToTimezone);

    const subOptions =
      offsets.length > 1
        ? offsets.map(item => ({
            label: `(UTC ${Number(item) / 60})`,
            value: Number(item) / 60,
            parentId: country.value,
          }))
        : [];

    return [...acc, country, ...subOptions];
  }, []);
};

type GetCountryProps = {
  country: string;
  callHours: ICallHourItem[];
  countries: IOption[];
  index: number;
};

type GetCountry = (props: GetCountryProps) => IOption | undefined;

export const getCountry: GetCountry = ({ country, callHours, countries, index }) => {
  if (!country) return;

  const value = countries.find(item => item.value === country);

  if (!value?.value) return;

  if (callHours.filter(item => item.country === country).length > 1) {
    const indexes = callHours.reduce((res, item, index) => {
      if (item.country === country) {
        return [...res, index];
      }

      return res;
    }, []);

    const i = indexes.findIndex(i => i === index) + 1;

    return {
      value: value.value,
      parentId: value.parentId,
      label: `${value.label} ${i}`,
    };
  }

  return value;
};
