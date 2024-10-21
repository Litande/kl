import styled, { CSSObject, useTheme } from "styled-components";
import { Control, Controller, useWatch } from "react-hook-form";
import Input from "components/input/Input";
import MultiSelect, { SelectedValue } from "components/multiSelect/MultiSelect";
import React, { useContext, useRef, useState, useEffect } from "react";
import { SettingItemContext } from "pages/settings/ContextProvider";

import ArrowExpand from "components/arrowExpand/ArrowExpand";
import moment from "moment-timezone";
import OptionsButton from "components/button/OptionsButton";
import { ICallHourItem, ICallHours } from "./types";
import useToggle from "hooks/useToggle";
import TimezoneItem from "./TimezoneItem";
import { getCallingHours, getCountry } from "./helpers";
import { TableHeader, TData, TRow, Divider, SaveIcon } from "./Common";
import useExpandCollapseAnimation from "hooks/useExpandCollapseAnimation";
import ConfirmationModal from "../../../../components/confirmationModal/ConfirmationModal";

type ComponentProps = {
  item: ICallHourItem;
  control: Control<ICallHours>;
  index: number;
  isEditing: boolean;
  onRemove: (index) => void;
  onSave: (i) => void;
  fields: ICallHourItem[];
  checkIsValid: (index) => Promise<boolean>;
};

const CallHoursItem = ({
  item,
  control,
  index,
  onRemove,
  onSave,
  isEditing,
  fields,
  checkIsValid,
}: ComponentProps) => {
  const theme = useTheme();
  const [isExpanded, setIsExpanded] = useToggle(false);
  const [isDeleteConfirmationShown, setIsDeleteConfirmationShown] = useState(false);
  const [isValid, setIsValid] = useState(false);
  const [fieldsState, setFieldsState] = useState({
    country: "",
    from: "",
    till: "",
  });
  const timezonesRef = useRef(null);
  const { styles } = useExpandCollapseAnimation({
    isExpanded,
    height: timezonesRef.current?.clientHeight,
  });
  const { countries } = useContext(SettingItemContext);
  const [callHours, country] = useWatch({
    control,
    name: ["callHours", "country"],
  });

  const timezones = item.country ? moment.tz.zonesForCountry(item.country, true) : [];
  const hasMultipleTimezones = timezones.length > 1;

  const hasError = (country, key, value) => {
    if (!isEditing || !country) return false;
    const sameCountry = fields.filter(item => item.country === country);

    return Boolean(sameCountry.length ? sameCountry.find(item => item[key] === value) : false);
  };

  useEffect(() => {
    const { country, till, from } = fieldsState;

    if (country && till && from) {
      if (!hasError(country, "till", till) && !hasError(country, "from", from)) {
        setIsValid(true);
      }
    }
  }, [fieldsState, hasError]);

  const renderConvertedHours = index => {
    if (hasMultipleTimezones) {
      return (
        <TData colSpan={4}>
          <ExpandText>Expand the line to see converted hours </ExpandText>
        </TData>
      );
    }

    if (isEditing) {
      return <TData colSpan={4}></TData>;
    }

    const { from, till } = getCallingHours({
      country,
      timezone: timezones[0],
      callHour: callHours[index],
    });

    return (
      <>
        <TData>
          <Input value={from} disabled />
        </TData>
        <TData>
          <Divider />
        </TData>
        <TData>
          <Input value={till} disabled />
        </TData>
        <TData />
      </>
    );
  };

  return (
    <>
      <TRow key={index} isExpanded={isExpanded} isEditing={isEditing}>
        {!isEditing && (
          <TData>
            <ItemIndex>{index + 1}</ItemIndex>
          </TData>
        )}
        <TData colSpan={isEditing ? 2 : 1}>
          <Controller
            name={`callHours.${index}.country`}
            control={control}
            rules={{ required: true }}
            render={({ formState, field: { ref, ...rest } }) => {
              const value = getCountry({ country: rest.value, callHours, countries, index });

              const errors = formState.errors.callHours || [];
              const error = errors[index] ? errors[index].country : null;

              return (
                <StyledMultiSelect
                  {...rest}
                  value={value ? [value] : []}
                  label={isEditing ? "Country" : ""}
                  onChange={([{ value }]) => {
                    rest.onChange(value);
                    setFieldsState({ ...fieldsState, country: `${value}` });
                  }}
                  options={countries}
                  error={error}
                  disabled={!isEditing}
                />
              );
            }}
          />
        </TData>
        <TData isHidden={isEditing && !fieldsState.country}>
          <Controller
            name={`callHours.${index}.from`}
            control={control}
            rules={{ required: true }}
            render={({ field: { ref, ...rest }, formState }) => {
              const errors = formState.errors.callHours || [];
              const error = errors[index] ? errors[index].from : null;

              return (
                <Input
                  {...rest}
                  type="time"
                  label={isEditing ? "From" : ""}
                  error={error || hasError(fieldsState.country, "from", rest.value)}
                  onChange={e => {
                    rest.onChange(e.target.value);
                    setFieldsState({ ...fieldsState, from: e.target.value });
                  }}
                  disabled={!isEditing}
                />
              );
            }}
          />
        </TData>
        <TData isHidden={isEditing && !fieldsState.country}>
          <Divider />
        </TData>
        <TData isHidden={isEditing && !fieldsState.country}>
          <Controller
            name={`callHours.${index}.till`}
            rules={{ required: true }}
            control={control}
            render={({ formState, field: { ref, ...rest } }) => {
              const errors = formState.errors.callHours || [];
              const error = errors[index] ? errors[index].till : null;

              return (
                <Input
                  {...rest}
                  type="time"
                  label={isEditing ? "Till" : ""}
                  onChange={e => {
                    rest.onChange(e.target.value);
                    setFieldsState({ ...fieldsState, till: e.target.value });
                  }}
                  error={error || hasError(fieldsState.country, "till", rest.value)}
                  disabled={!isEditing}
                />
              );
            }}
          />
        </TData>
        <TData />
        {renderConvertedHours(index)}
        <TData>
          <Actions>
            {hasMultipleTimezones && <ArrowExpand onClick={() => setIsExpanded()} />}
            {isEditing && <SaveIcon onClick={() => isValid && onSave(index)} disabled={!isValid} />}
            <RemoveIcon
              paintTO={theme.colors.btn.error_secondary}
              iconType="close"
              onClick={() => {
                if (isEditing) {
                  onRemove(index);
                } else {
                  setIsDeleteConfirmationShown(true);
                }
              }}
            />
          </Actions>
        </TData>
      </TRow>

      <tr>
        <td colSpan={11}>
          <TableWrapper styles={styles}>
            <table ref={timezonesRef} width="100%">
              <StyledTableHeader />
              <tbody>
                {timezones.length > 1 &&
                  timezones.map((timezone, i) => (
                    <TimezoneItem
                      key={i}
                      timezone={timezone}
                      callHour={callHours[index]}
                      country={country}
                    />
                  ))}
              </tbody>
            </table>
          </TableWrapper>
        </td>
      </tr>
      {isDeleteConfirmationShown && (
        <ConfirmationModal
          title="Attention"
          onConfirm={() => onRemove(index)}
          onCancel={() => setIsDeleteConfirmationShown(false)}
          hasCloseIcon
        >
          {`Would you like to remove this item?`}
        </ConfirmationModal>
      )}
    </>
  );
};

export default CallHoursItem;

const StyledMultiSelect = styled(MultiSelect)`
  ${SelectedValue} {
    ${({ disabled }) => (disabled ? `padding-left: 0` : "")};
    ${({ disabled }) => (disabled ? `border: 1px solid transparent` : "")};
    background: transparent;
  }

  i {
    ${({ disabled }) => (disabled ? `display: none` : "")};
  }
`;

const TableWrapper = styled.div<{ styles: CSSObject }>`
  ${({ styles }) => styles};
`;

const ItemIndex = styled.div`
  width: 30px;
  text-align: center;
`;

const Actions = styled.div`
  width: 100px;
  display: flex;
  justify-content: flex-end;
  gap: 1rem;
`;

const RemoveIcon = styled(OptionsButton)`
  align-self: flex-end;
`;

const ExpandText = styled.span`
  ${({ theme }) => theme.typography.body3};
  color: ${({ theme }) => theme.colors.fg.secondary_light};
`;

const StyledTableHeader = styled(TableHeader)`
  visibility: collapse;
`;
