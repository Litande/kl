import { useMemo } from "react";
import { ContentWrap, Wrap } from "pages/settings/items/BaseStyles";
import styled from "styled-components";
import { BaseAction } from "pages/settings/types";
import MultiSelect, { SubOptionsContainer } from "components/multiSelect/MultiSelect";
import { useContext, useState } from "react";
import { SettingItemContext } from "pages/settings/ContextProvider";
import { Control, Controller, useFieldArray } from "react-hook-form";
import Button from "components/button/Button";
import { ICallHours } from "./types";
import { getCountriesOptions } from "./helpers";
import { TableHeader } from "./Common";
import CallHoursItem from "./CallHoursItem";

type ComponentProps = BaseAction & {
  control: Control<ICallHours>;
  trigger: (fieldPath: string) => Promise<boolean>;
};

const CallHoursSetting = ({ control, trigger, ...savedProps }: ComponentProps) => {
  const options = useContext(SettingItemContext);
  const [editingIndexes, setEditingIndexes] = useState([]);
  const countriesAndTimezones = useMemo(() => {
    return getCountriesOptions(options);
  }, [options]);

  const { fields, append, remove } = useFieldArray({
    control,
    name: "callHours",
  });

  const checkIsValid = async index => {
    return await trigger(`callHours.${index}`);
  };

  const handleSave = async index => {
    const isValid = await trigger(`callHours.${index}`);

    if (isValid) {
      setEditingIndexes([]);
      savedProps.onApply();
    }
  };
  const handleRemove = index => {
    remove(index);

    if (editingIndexes.includes(index)) {
      setEditingIndexes(editingIndexes.filter(i => i !== index));
    } else {
      savedProps.onApply(false);
    }
  };

  const handleAdd = () => {
    append({
      country: "",
      from: "",
      till: "",
    });
    setEditingIndexes([...editingIndexes, fields.length]);
  };

  return (
    <StyledWrap>
      <ContentWrap>
        <ContentItemWrap>
          <LabelWrap>
            Chase calling hours by countries according to your default timezone. you can choose
            another timezone to simulate the calling hours in another timezone. Be aware that we
            detect related countries according to lead timezone, therefore, make sure you cover the
            entire list of relevant countries. For example, Vatican Time Zone is not included in
            Italy.
          </LabelWrap>
          <Controller
            name="country"
            control={control}
            render={({ field: { ref, ...rest } }) => {
              const { name, offset } = rest.value || {};

              const value = countriesAndTimezones.find(item => {
                if (offset) return item.parentId === name && item.value === offset;

                return item.value === name;
              });

              return (
                <SelectContainer>
                  <StyledMultiSelect
                    {...rest}
                    label="Convert Calling Hours to"
                    value={value ? [value] : []}
                    onChange={([{ value, parentId }]) => {
                      rest.onChange({
                        name: parentId || value,
                        offset: parentId ? value : null,
                      });
                      savedProps.onApply(false);
                    }}
                    options={countriesAndTimezones}
                  />
                </SelectContainer>
              );
            }}
          />
        </ContentItemWrap>
        <table>
          <TableHeader />
          <tbody>
            {fields.map((item, index) => (
              <CallHoursItem
                key={item.id}
                item={item}
                control={control}
                onRemove={handleRemove}
                onSave={handleSave}
                index={index}
                isEditing={editingIndexes.includes(index)}
                fields={fields}
                checkIsValid={checkIsValid}
              />
            ))}
          </tbody>
        </table>
        {!editingIndexes.length && (
          <ActionsWrap>{<Button onClick={handleAdd}>Add Country</Button>}</ActionsWrap>
        )}
      </ContentWrap>
    </StyledWrap>
  );
};

export default CallHoursSetting;

const ContentItemWrap = styled.div`
  display: flex;
  flex-direction: row;
  align-items: flex-start;
  margin-bottom: 1rem;
  padding: 0 1rem;
`;

const LabelWrap = styled.label`
  box-sizing: border-box;
  width: 63%;
  padding: 0 10px 3px;
  ${({ theme }) => theme.typography.smallText1}
  color: ${({ theme }) => theme.colors.fg.secondary_disabled};
`;

const ActionsWrap = styled.div`
  display: flex;
  justify-content: space-between;
  padding: 1rem;
`;

const SelectContainer = styled.div`
  width: 37%;
`;

const StyledMultiSelect = styled(MultiSelect)`
  min-width: 150px;
  max-width: 280px;

  @media (max-width: 1360px) {
    width: 50%;

    ${SubOptionsContainer} {
      max-width: 150px;
      right: -150px;
    }
  }
`;

const StyledWrap = styled(Wrap)`
  padding: 1rem 0;
`;
