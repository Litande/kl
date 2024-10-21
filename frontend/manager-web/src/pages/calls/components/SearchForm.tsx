import React, { useEffect, useState } from "react";
import { Controller, useForm } from "react-hook-form";
import styled, { CSSObject } from "styled-components";
import moment from "moment";

import useBehavior from "hooks/useBehavior";
import Input from "components/input/Input";
import Button from "components/button/Button";
import DatePicker from "components/datePicker/DatePicker";
import { ILabelValue } from "types";

import { IFormKey, IFormValues } from "../types";
import { defaultFields } from "../common";
import behavior from "../behavior";
import MultiSelect from "components/multiSelect/MultiSelect";
import useExpandCollapseAnimation from "hooks/useExpandCollapseAnimation";
import useOptions from "hooks/useOptions";

const durations = [
  {
    label: "> 1 min",
    value: 1,
  },
  {
    label: "> 5 min",
    value: 5,
  },
  {
    label: "> 10 min",
    value: 10,
  },
  {
    label: "> 15 min",
    value: 15,
  },
  {
    label: "> 20 min",
    value: 20,
  },
  {
    label: "> 30 min",
    value: 30,
  },
  {
    label: "> 60 min",
    value: 60,
  },
];

const callTypes: Array<ILabelValue> = [
  {
    label: "Manual",
    value: "manual",
  },
  {
    label: "Predictive",
    value: "predictive",
  },
];

const getFilterValue = value => {
  if (typeof value === "string") {
    return value;
  }
  if (value instanceof Date) {
    return moment(value).format("DD/MM/YYYY");
  }
};

function SearchForm() {
  const { applySearch, getCalls, applyPagination } = useBehavior(behavior);
  const { agents, countries, statuses } = useOptions({
    withCountries: true,
    withAgents: true,
    withStatuses: true,
  });
  const [isShowForm, setIsShowForm] = useState(true);
  const { handleSubmit, getValues, control, reset, resetField } = useForm<IFormValues>({
    defaultValues: defaultFields,
  });
  const { styles } = useExpandCollapseAnimation({ isExpanded: isShowForm, height: 146 });
  const fields = getValues();
  const appliedFilters = Object.entries(fields).filter(([_, value]) => value);
  const submitForm = (data: IFormValues) => {
    applyPagination({ page: 1 });
    applySearch(data);
    getCalls();
    setIsShowForm(false);
  };

  const resetForm = () => {
    reset(defaultFields, { keepIsSubmitted: false });
    applyPagination({ page: 1 });
    applySearch();
    getCalls();
    setIsShowForm(true);
  };
  const handleFilterIconClick = () => setIsShowForm(!isShowForm);

  useEffect(() => {
    getCalls();
    // eslint-disable-next-line react-hooks/exhaustive-deps
  }, []);

  const renderFields = () => {
    return (
      <FieldsContainer styles={styles}>
        <Row>
          <Field>
            <Controller
              name="agents"
              control={control}
              render={({ field: { ref, ...rest } }) => (
                <MultiSelect {...rest} label="Agents" options={agents} isMulti />
              )}
            />
          </Field>
          <Field>
            <Controller
              name="groupName"
              control={control}
              render={({ field: { ref, ...rest } }) => <Input {...rest} label="Group Name" />}
            />
          </Field>
          <Field>
            <Controller
              name="leadId"
              control={control}
              render={({ field: { ref, ...rest } }) => <Input {...rest} label="Lead Id" />}
            />
          </Field>
          <Field>
            <Controller
              name="leadPhone"
              control={control}
              render={({ field: { ref, ...rest } }) => <Input {...rest} label="Lead Phone" />}
            />
          </Field>
          <Field>
            <Controller
              name="country"
              control={control}
              render={({ field: { ref, ...rest } }) => (
                <MultiSelect {...rest} label="country" options={countries} isMulti isSearch />
              )}
            />
          </Field>
        </Row>
        <Row>
          <Field>
            <Controller
              name="callType"
              control={control}
              render={({ field: { ref, ...rest } }) => (
                <MultiSelect {...rest} label="call Type" options={callTypes} />
              )}
            />
          </Field>
          <Field>
            <Controller
              name="leadStatusAfter"
              control={control}
              render={({ field: { ref, ...rest } }) => (
                <MultiSelect {...rest} label="lead Status" options={statuses} isMulti />
              )}
            />
          </Field>
          <Field>
            <Controller
              name="fromDate"
              control={control}
              render={({ field: { ref, ...rest } }) => (
                <DatePicker
                  {...rest}
                  initialDate={rest.value || null}
                  onSelect={date => rest.onChange(date)}
                  label="From date"
                />
              )}
            />
          </Field>
          <Field>
            <Controller
              name="tillDate"
              control={control}
              render={({ field: { ref, ...rest } }) => (
                <DatePicker
                  {...rest}
                  initialDate={rest.value || null}
                  onSelect={date => rest.onChange(date)}
                  label="Till date"
                />
              )}
            />
          </Field>
          <Field>
            <Controller
              name="duration"
              control={control}
              render={({ field: { ref, ...rest } }) => (
                <MultiSelect {...rest} label="duration" options={durations} />
              )}
            />
          </Field>
        </Row>
      </FieldsContainer>
    );
  };

  return (
    <FormContainer onSubmit={handleSubmit(submitForm)}>
      {renderFields()}
      <FormFooter>
        <FilterIcon
          className={isShowForm ? "icon-filter" : "icon-filter-applied"}
          onClick={handleFilterIconClick}
        ></FilterIcon>
        {appliedFilters && (
          <Filters>
            {appliedFilters.map(([key, value]: [IFormKey, IFormValues[IFormKey]]) => {
              if (!Array.isArray(value)) {
                return (
                  <Filter key={key} onClick={() => resetField(key)}>
                    {getFilterValue(value)}
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
          <SearchButton type="submit">Search Call</SearchButton>
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
  border-bottom: 1px solid ${({ theme }) => theme.colors.border.primary};
`;

const Row = styled.div`
  display: flex;
  align-items: center;
  gap: 1rem;
`;

const FieldsContainer = styled.div<{ styles: CSSObject }>`
  box-sizing: border-box;
  display: flex;
  flex-direction: column;
  gap: 1rem;
  ${({ styles }) => styles};
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
  flex-grow: 1;
  box-sizing: border-box;
  width: calc(25% - 16px);
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
  border: 1px solid ${({ theme }) => theme.colors.border.secondary};
  border-radius: 4px;
  cursor: pointer;

  ::before {
    color: ${({ theme }) => theme.colors.border.secondary};
  }
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
