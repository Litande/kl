import React, { useState } from "react";
import styled from "styled-components";
import { Controller, useForm } from "react-hook-form";
import MultiSelect from "components/multiSelect/MultiSelect";
import Button from "components/button/Button";
import { useSelector } from "react-redux";
import { filterOptionsSelector } from "./selector";
import useBehavior from "hooks/useBehavior";
import behavior from "./behavior";
import AddAgentModal from "components/agentModal/AddAgentModal";

export interface IOption {
  label: string;
  value: string;
}

export interface IFormValues {
  users: IOption[];
  tags: IOption[];
  states: IOption[];
  groups: IOption[];
}

const defaultFields = {
  users: [],
  tags: [],
  states: [],
  groups: [],
};

const Filters = ({ onAddAgent }: { onAddAgent: () => void }) => {
  const [isAddAgentModalShown, setIsAddAgentModalShown] = useState(false);
  const { applyFilters } = useBehavior(behavior);

  const { handleSubmit, control, reset } = useForm<IFormValues>({
    defaultValues: defaultFields,
  });

  const handleAddAgent = () => {
    onAddAgent();
    setIsAddAgentModalShown(false);
  };

  const showAgentModal = () => setIsAddAgentModalShown(true);

  const submitForm = (data: IFormValues) => {
    applyFilters(data);
  };
  const resetForm = () => {
    reset(defaultFields, { keepIsSubmitted: false });
  };

  const filterOptions = useSelector(filterOptionsSelector);

  return (
    <FormContainer onSubmit={handleSubmit(submitForm)}>
      <Field>
        <Controller
          name="users"
          control={control}
          render={({ field: { ref, ...rest } }) => (
            <MultiSelect
              {...rest}
              label="Users"
              placeholder="All"
              isMulti
              options={filterOptions["users"]}
            />
          )}
        />
      </Field>
      <Field>
        <Controller
          name="tags"
          control={control}
          render={({ field: { ref, ...rest } }) => (
            <MultiSelect
              {...rest}
              placeholder="None selected"
              label="Tags"
              options={filterOptions["tags"]}
              isMulti
            />
          )}
        />
      </Field>
      <Field>
        <Controller
          name="states"
          control={control}
          render={({ field: { ref, ...rest } }) => (
            <MultiSelect
              {...rest}
              label="State"
              placeholder="All"
              options={filterOptions["states"]}
              isMulti
            />
          )}
        />
      </Field>
      <Field>
        <Controller
          name="groups"
          control={control}
          render={({ field: { ref, ...rest } }) => (
            <MultiSelect
              {...rest}
              placeholder="None selected"
              label="Groups"
              options={filterOptions["groups"]}
              isMulti
            />
          )}
        />
      </Field>
      <Actions>
        <ResetButton variant="secondary" onClick={resetForm}>
          Reset
        </ResetButton>
        <ApplyButton type="submit">Apply</ApplyButton>
        <AgentIcon className="icon-add-agent" onClick={showAgentModal} />
      </Actions>
      {isAddAgentModalShown && <AddAgentModal onSave={handleAddAgent} onClose={handleAddAgent} />}
    </FormContainer>
  );
};

export default Filters;

const FormContainer = styled.form`
  flex: 1;
  justify-content: flex-end;
  display: flex;
  align-items: flex-end;
  max-width: calc(100% - 150px);
  border-radius: 0 0 4px 4px;
  text-transform: initial;
`;

const Field = styled.div`
  box-sizing: border-box;
  width: calc(25% - 16px);
  margin: 0 8px;

  @media (min-width: 1200px) {
    width: calc(20% - 16px);
  }

  @media (min-width: 1600px) {
    width: calc(16.6% - 16px);
  }
`;

const Actions = styled.div`
  display: flex;
  align-items: center;
  justify-content: space-between;
  gap: 10px;
  margin: 0 0 0 10px;
`;

const AgentIcon = styled.i`
  padding: 5px;
  font-size: 24px;
  color: ${({ theme }) => theme.colors.btn.secondary};
  border: 1px solid ${({ theme }) => theme.colors.btn.secondary};
  border-radius: 4px;
  cursor: pointer;
`;

const ResetButton = styled(Button)``;
const ApplyButton = styled(Button)``;
