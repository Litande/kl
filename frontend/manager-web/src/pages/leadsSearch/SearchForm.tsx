import { Controller, useForm } from "react-hook-form";
import React, { Fragment, useState } from "react";
import styled, { CSSObject } from "styled-components";
import MultiSelect, { IOption } from "components/multiSelect/MultiSelect";
import Input from "components/input/Input";
import Button from "components/button/Button";
import useExpandCollapseAnimation from "hooks/useExpandCollapseAnimation";

export interface IFormValues {
  leadId: string;
  phone: string;
  campaign: string;
  firstName: string;
  lastName: string;
  callType: string;
  weightMoreThan: string;
  weightLessThan: string;
  totalCalls: string;
  weight: string;
  brand: IOption[];
  leadStatus: IOption[];
  assignedAgent: IOption[];
  country: IOption[];
}

type IFormKey = keyof IFormValues;

export const defaultFields = {
  cid: "",
  phone: "",
  campaign: "",
  firstName: "",
  lastName: "",
  callType: "",
  totalCalls: "",
  weightMoreThan: "",
  weightLessThan: "",
  brand: [],
  leadStatus: [],
  assignedAgent: [],
  country: [],
};

interface IForm {
  options: {
    countries: IOption[];
    statuses: IOption[];
    agents: IOption[];
  };
  onSubmit: (data: IFormValues) => void;
  onReset: () => void;
}

function SearchForm({ onSubmit, onReset, options }: IForm) {
  const [isExpanded, setIsExpanded] = useState(true);
  const { styles } = useExpandCollapseAnimation({
    isExpanded,
    height: 146,
  });
  const { handleSubmit, getValues, control, reset, resetField } = useForm<IFormValues>({
    defaultValues: defaultFields,
  });
  const fields = getValues();
  const appliedFilters = Object.entries(fields).filter(([_, value]) => value?.length);

  const submitForm = (data: IFormValues) => {
    onSubmit(data);
    setIsExpanded(false);
  };
  const resetForm = () => {
    reset(defaultFields, { keepIsSubmitted: false });
    setIsExpanded(true);
    onReset();
  };

  const renderFields = () => {
    return (
      <Fragment>
        <Row>
          <Field>
            <Controller
              name="leadId"
              control={control}
              render={({ field: { ref, ...rest } }) => <Input {...rest} label="CID" />}
            />
          </Field>
          <Field>
            <Controller
              name="phone"
              control={control}
              render={({ field: { ref, ...rest } }) => <Input {...rest} label="Phone" />}
            />
          </Field>
          <Field>
            <Controller
              name="firstName"
              control={control}
              render={({ field: { ref, ...rest } }) => <Input {...rest} label="First Name" />}
            />
          </Field>
          <Field>
            <Controller
              name="lastName"
              control={control}
              render={({ field: { ref, ...rest } }) => <Input {...rest} label="Last Name" />}
            />
          </Field>
          <Field>
            <Controller
              name="country"
              control={control}
              render={({ field: { ref, ...rest } }) => (
                <MultiSelect
                  {...rest}
                  label="Country"
                  isMulti
                  isSearch
                  options={options.countries}
                />
              )}
            />
          </Field>
        </Row>
        <Row>
          <Field>
            <Controller
              name="weightMoreThan"
              control={control}
              render={({ field: { ref, ...rest } }) => <Input {...rest} label="Weigh more than" />}
            />
          </Field>
          <Field>
            <Controller
              name="weightLessThan"
              control={control}
              render={({ field: { ref, ...rest } }) => <Input {...rest} label="Weight less than" />}
            />
          </Field>
          <Field>
            <Controller
              name="totalCalls"
              control={control}
              render={({ field: { ref, ...rest } }) => <Input {...rest} label="Total Calls" />}
            />
          </Field>
          <Field>
            <Controller
              name="leadStatus"
              control={control}
              render={({ field: { ref, ...rest } }) => (
                <MultiSelect
                  {...rest}
                  label="Sales status"
                  isMulti
                  isSearch
                  options={options.statuses}
                />
              )}
            />
          </Field>
          <Field>
            <Controller
              name="assignedAgent"
              control={control}
              render={({ field: { ref, ...rest } }) => (
                <MultiSelect
                  {...rest}
                  label="Assigned To Agent"
                  isMulti
                  isSearch
                  options={options.agents}
                />
              )}
            />
          </Field>
        </Row>
      </Fragment>
    );
  };

  return (
    <FormContainer onSubmit={handleSubmit(submitForm)}>
      <FieldsContainer styles={styles}>{renderFields()}</FieldsContainer>
      <FormFooter>
        <FilterIcon
          onClick={() => setIsExpanded(!isExpanded)}
          className={isExpanded ? "icon-filter" : "icon-filter-applied"}
        ></FilterIcon>
        {appliedFilters && (
          <Filters>
            {appliedFilters.map(([key, value]: [IFormKey, IFormValues[IFormKey]]) => {
              if (typeof value === "string") {
                return (
                  <Filter key={key} onClick={() => resetField(key)}>
                    {value}
                    <CrossIcon className="icon-close" />
                  </Filter>
                );
              }

              return value.map(item => (
                <Filter key={key + item.value}>
                  {item.label}
                  <CrossIcon
                    className="icon-close"
                    onClick={() => {
                      const filters = {
                        ...fields,
                        [key]: value.filter(i => i.value !== item.value),
                      };
                      reset(filters, { keepIsSubmitted: true });
                    }}
                  />
                </Filter>
              ));
            })}
          </Filters>
        )}
        <Actions>
          <ResetButton type="button" variant="secondary" onClick={resetForm}>
            Reset
          </ResetButton>
          <SearchButton type="submit">Search Leads</SearchButton>
        </Actions>
      </FormFooter>
    </FormContainer>
  );
}

export default SearchForm;

const FormContainer = styled.form`
  display: flex;
  flex-direction: column;
  flex-wrap: wrap;
  padding: 1rem 0;
`;

const FieldsContainer = styled.div<{ styles: CSSObject }>`
  box-sizing: border-box;
  display: flex;
  flex-direction: column;
  gap: 1rem;
  ${({ styles }) => styles};
`;

const Row = styled.div`
  display: flex;
  align-items: center;
  gap: 1rem;
`;
const Filters = styled.div`
  display: flex;
  flex-wrap: wrap;
  gap: 4px;
  width: 100%;
  margin: 0 5px;
`;

const Filter = styled.div`
  display: flex;
  justify-content: space-between;
  align-items: center;
  padding: 10px;
  background: rgba(82, 148, 195, 0.12);
  border-radius: 4px;
  ${({ theme }) => theme.typography.smallText3};
  line-height: 1;
`;

const CrossIcon = styled.i`
  display: flex;
  align-items: center;
  justify-content: center;
  width: 16px;
  height: 16px;
  text-align: center;
  font-size: 12px;
  background: ${({ theme }) => theme.colors.bg.ternary};
  border-radius: 50%;
  color: ${({ theme }) => theme.colors.fg.secondary_light};
  cursor: pointer;
`;

const Field = styled.div`
  box-sizing: border-box;
  width: calc(25% - 16px);
  flex-grow: 1;

  @media (min-width: 1200px) {
    width: calc(20% - 16px);
  }

  @media (min-width: 1600px) {
    width: calc(16.6% - 16px);
  }
`;

const FormFooter = styled.div`
  width: 100%;
  display: flex;
  justify-content: space-between;
  align-items: center;
`;

const FilterIcon = styled.i`
  box-sizing: border-box;
  display: flex;
  justify-content: center;
  align-items: center;
  height: 36px;
  min-width: 36px;
  font-size: 24px;
  border: 1px solid ${({ theme }) => theme.colors.btn.secondary};
  border-radius: 4px;
  color: ${({ theme }) => theme.colors.btn.secondary};
  cursor: pointer;
`;

const Actions = styled.div`
  display: flex;
`;

const ResetButton = styled(Button)`
  width: 136px;
  margin: 0 16px 0 0;
`;
const SearchButton = styled(Button)`
  width: 136px;
`;
