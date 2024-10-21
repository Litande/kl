import { useEffect, useState } from "react";
import styled, { useTheme } from "styled-components";
import { useSelector } from "react-redux";
import {
  Controller,
  UseFieldArrayReturn,
  Control,
  UseFormRegister,
  UseFormWatch,
  UseFormResetField,
  FieldErrorsImpl,
} from "react-hook-form";

import { RootState } from "store";
import OptionsButton from "components/button/OptionsButton";
import MultiSelect from "components/multiSelect/MultiSelect";
import { conditionsActionsSelector } from "components/ruleEngine/rulesSelector";
import { IForm } from "components/ruleEngine/types";
import Input from "components/input/Input";
import { fieldTypeToInputType } from "components/ruleEngine/utils";
import ErrorMessage from "./ErrorMessage";
import MultipleInput from "components/multiInput";

type CombinationParts = keyof RootState["ruleEngineRules"];

/* eslint-disable @typescript-eslint/no-explicit-any */
interface IRow extends UseFieldArrayReturn<IForm, any, "id"> {
  rowId?: string | number;
  order?: string | number;
  rowType: CombinationParts;
  control: Control<any>;
  register: UseFormRegister<any>;
  resetField: UseFormResetField<any>;
  watch: UseFormWatch<any>;
  index: number;
  errors: FieldErrorsImpl<IForm>;
  removeRowHandler: (rowType, index) => void;
}
/* eslint-enable @typescript-eslint/no-explicit-any */

function Row({
  rowType,
  rowId,
  order,
  control,
  watch,
  register,
  resetField,
  index,
  errors,
  removeRowHandler,
  ...formArray
}: IRow) {
  const theme = useTheme();
  const rowKey = `${rowType}.${index}.`;
  const [state, setState] = useState(null);

  const ruleCategory = useSelector(conditionsActionsSelector);

  const watchName = watch(rowKey + "name")?.value;
  const operatorName = rowType === "actions" ? "actionOperation" : "comparisonOperation";
  const fieldsCatogories = ruleCategory[rowType]?.reduce((acc, r) => {
    if (acc && r.category && !acc[r.category]) {
      acc[r.category] = [];
    }
    return acc;
  }, {});

  useEffect(() => {
    const ruleOptionsByType = ruleCategory[rowType].find(option => option.name === watchName);
    setState(ruleOptionsByType);
    if (state?.name) {
      // DENGEROUS because innerFields doesn't rerender corectly
      resetField(rowKey + operatorName, { defaultValue: null });
      resetField(rowKey + "fields", { defaultValue: null });
      formArray.update(index, {
        name: {
          label: watchName,
          value: watchName,
        },
      });
    }
    // eslint-disable-next-line react-hooks/exhaustive-deps
  }, [watchName]);

  return (
    <RulesContainer>
      {index !== 0 && index !== formArray.fields.length && (
        <RowWrapper gap={0}>
          <InputsColumn>
            <GroupButton>AND</GroupButton>
          </InputsColumn>
          <ActionsColumn></ActionsColumn>
        </RowWrapper>
      )}
      <RowWrapper gap={20}>
        <InputsColumn>
          <RowWrapper gap={10}>
            <Field>
              <Controller
                name={`${rowKey}name`}
                control={control}
                rules={{ required: "field is required" }}
                render={({ field: { ref, ...rest } }) => (
                  <>
                    <MultiSelect
                      {...rest}
                      value={[rest.value]}
                      onChange={([value]) => rest.onChange(value)}
                      options={ruleCategory[rowType]?.map(r => {
                        return {
                          value: r.name,
                          label: r.displayName,
                          category: r.category,
                        };
                      })}
                      categories={Object.keys(fieldsCatogories).length > 1 && fieldsCatogories}
                    />
                    <ErrorMessage>
                      {errors[rowType] && errors[rowType][index]?.name?.message}
                    </ErrorMessage>
                  </>
                )}
              />
            </Field>
            {(state?.comparisonOperation || state?.actionOperation) && (
              <Field>
                <Controller
                  name={rowKey + operatorName}
                  control={control}
                  render={({ field: { ref, ...rest } }) => (
                    <MultiSelect
                      {...rest}
                      value={[rest.value]}
                      onChange={([value]) => rest.onChange(value)}
                      options={state?.comparisonOperation || state?.actionOperation}
                    />
                  )}
                />
              </Field>
            )}
            {state?.fields?.map((field, idx) => {
              const isInput =
                !field.values.length && !fieldTypeToInputType(field.type)?.isMultipleInput;
              const props = register(rowKey + "fields." + idx + ".value", {
                required: field.isRequired && "field is required",
              });
              return isInput ? (
                <Field key={idx}>
                  <Input
                    {...props}
                    placeholder={field.dimension}
                    type={fieldTypeToInputType(field.type)}
                  />
                  <HiddenInput
                    type="text"
                    value={field.type}
                    {...register(rowKey + "fields." + idx + ".type")}
                  />
                  <ErrorMessage>
                    {errors[rowType] && errors[rowType][index]?.fields[idx]?.value?.message}
                  </ErrorMessage>
                </Field>
              ) : (
                <Field key={idx}>
                  <Controller
                    name={rowKey + "fields." + idx + ".value"}
                    control={control}
                    rules={{
                      required: field.isRequired && "field is required",
                    }}
                    render={({ field: { ref, ...rest } }) => {
                      const { isMulti, isMultipleInput } = fieldTypeToInputType(field.type);
                      const value = rest.value;
                      return (
                        <>
                          {isMultipleInput ? (
                            <MultipleInput placeholder={field.dimension} {...rest} />
                          ) : (
                            <MultiSelect
                              {...rest}
                              {...fieldTypeToInputType(field.type)}
                              value={isMulti ? value : [value]}
                              placeholder={field.dimension}
                              options={field.values}
                              onChange={options => rest.onChange(isMulti ? options : options[0])}
                            />
                          )}
                          <ErrorMessage>
                            {errors[rowType] && errors[rowType][index]?.fields[idx]?.value?.message}
                          </ErrorMessage>
                        </>
                      );
                    }}
                  />
                  <HiddenInput
                    type="text"
                    value={field.type}
                    {...register(rowKey + "fields." + idx + ".type")}
                  />
                </Field>
              );
            })}
          </RowWrapper>
        </InputsColumn>
        <ActionsColumn>
          <OptionsButton
            onClick={() => {
              removeRowHandler(rowType, index);
            }}
            paintTO={theme.colors.error}
            iconType="close"
          />
        </ActionsColumn>
      </RowWrapper>
    </RulesContainer>
  );
}

export default Row;

const RulesContainer = styled.div`
  border-radius: 4px;
  border-top: none;
  margin: 1rem;
`;

const RowWrapper = styled.div<{ gap: number }>`
  display: flex;
  flex-direction: row;
  gap: ${props => props.gap + "px"};
`;

const InputsColumn = styled.div`
  display: flex;
  flex-direction: column;
  flex: 10 1 auto;
  gap: 10px;
  position: relative;
`;

const ActionsColumn = styled.div`
  display: flex;
  flex-direction: row;
  gap: 10px;
  align-items: flex-start;
  justify-content: flex-end;
  flex: 0 1 auto;
`;

const Field = styled.div`
  position: relative;
  box-sizing: border-box;
  width: calc(35% - 16px);
  @media (min-width: 1200px) {
    width: calc(35% - 16px);
  }

  @media (min-width: 1600px) {
    width: calc(16.6% - 16px);
  }
`;

const GroupButton = styled.div`
  ${({ theme }) => theme.typography.buttonsText};
  width: 10ch;
  display: flex;
  justify-content: center;
  align-items: center;
  height: 2.5rem;
  background: #ffffff;
  border: 1px solid rgba(0, 0, 0, 0.12);
  border-radius: 18px;
  margin: 10px auto;
  margin-top: 0;

  ::before {
    content: " ";
    position: absolute;
    left: 0;
    right: 0;
    z-index: -1;

    height: 1px;
    background: ${({ theme }) => theme.colors.bg.divider};
  }
`;

const HiddenInput = styled.input`
  visibility: hidden;
  width: 1px;
  height: 1px;
  opacity: 0;
`;
