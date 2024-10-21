import styled, { useTheme } from "styled-components";
import { borderStyle } from "globalStyles/theme/border";
import { HelperTextWrap } from "pages/settings/items/BaseStyles";
import { Control, Controller } from "react-hook-form";
import Input from "components/input/Input";
import MultiSelect from "components/multiSelect/MultiSelect";
import { SettingType } from "pages/settings/types";
import { useContext } from "react";
import { SettingItemContext } from "pages/settings/ContextProvider";
import OptionsButton from "components/button/OptionsButton";

type ComponentProps = {
  index: number;
  control: Control;
  onRemove: (index: number) => void;
};
const TelephonyItem = ({ onRemove, control, index }: ComponentProps) => {
  const theme = useTheme();
  const { countries } = useContext(SettingItemContext);
  return (
    <Wrap>
      <IDWrap>{index + 1}</IDWrap>
      <PhoneWrap>
        <Controller
          name={`${SettingType.Telephony}.${index}.phoneNumber`}
          control={control}
          render={({ field: { ref, ...rest } }) => (
            <Input {...rest} autoComplete={"off"} type="tel" />
          )}
        />
      </PhoneWrap>
      <CountriesWrap>
        <HelperTextWrap>Applied for:</HelperTextWrap>
        <Controller
          name={`${SettingType.Telephony}.${index}.country`}
          control={control}
          render={({ field: { ref, ...rest } }) => {
            const value = countries.find(item => item.value === rest.value);

            return (
              <MultiSelect
                {...rest}
                options={countries}
                value={value ? [value] : []}
                onChange={([{ value }]) => rest.onChange(value)}
                displayStrategy="portal"
                isSearch
              />
            );
          }}
        />
      </CountriesWrap>
      <RemoveIcon
        paintTO={theme.colors.btn.error_secondary}
        iconType="close"
        onClick={() => onRemove(index)}
      />
    </Wrap>
  );
};

export default TelephonyItem;

const Wrap = styled.div`
  position: relative;
  display: flex;
  flex-direction: row;
  border-bottom: ${borderStyle.primary};
  padding: 0.4rem 3.5rem 0.4rem 0.4rem;
  gap: 32px;
  align-items: center;
  width: 100%;
  box-sizing: border-box;

  &:hover {
    background: ${({ theme }) => theme.colors.bg.active};
  }
`;

const IDWrap = styled.div``;
const PhoneWrap = styled.div`
  flex: 1;
  max-width: 350px;
`;

const CountriesWrap = styled.div`
  display: flex;
  flex: 1.35;
  align-items: center;
  flex-direction: row;
  gap: 0.5rem;
  max-width: 440px;
`;

const RemoveIcon = styled(OptionsButton)`
  position: absolute;
  right: 0;
`;
