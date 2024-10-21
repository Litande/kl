import { Controller, UseFieldArrayReturn, UseFormReturn } from "react-hook-form";
import styled, { useTheme } from "styled-components";

import useToggle from "hooks/useToggle";
import OptionsButton from "components/button/OptionsButton";
import Input from "components/input/Input";
import MultiSelect from "components/multiSelect/MultiSelect";
import { IForm } from "components/ruleEngine/types";
import { STATUS_OPTIONS } from "components/ruleEngine/constants";
import Row from "components/ruleEngine/components/rules/RuleRow";
import ErrorMessage from "components/ruleEngine/components/rules/ErrorMessage";

// eslint-disable-next-line @typescript-eslint/no-explicit-any
interface IRuleFormProps extends UseFormReturn<IForm, any> {
  submitForm: (data) => void;
  conditions: UseFieldArrayReturn<IForm, "conditions", "id">;
  actions: UseFieldArrayReturn<IForm, "actions", "id">;
  onClose: VoidFunction;
  removeRowHandler: (rowType, index) => void;
  appendRowHandler: (rowType) => void;
}

function RuleForm(props: IRuleFormProps) {
  const {
    control,
    register,
    watch,
    resetField,
    submitForm,
    conditions,
    actions,
    formState,
    onClose,
    removeRowHandler,
    appendRowHandler,
  } = props;
  const theme = useTheme();

  const [isOpenConditions, setIsOpenConditions] = useToggle(true);
  const [isOpenActions, setIsOpenActions] = useToggle(true);

  return (
    <Container>
      <RuleRow>
        <Field>
          <Controller
            name="name"
            control={control}
            rules={{ required: "Rule name is required" }}
            render={({ field: { ...rest } }) => <Input label="Rule name" {...rest} />}
          />
          <ErrorMessage>{formState.errors?.name?.message}</ErrorMessage>
        </Field>
        <Field>
          <Controller
            name="status"
            control={control}
            render={({ field: { ref, ...rest } }) => (
              <MultiSelect
                {...rest}
                value={[rest.value]}
                onChange={([value]) => rest.onChange(value)}
                options={STATUS_OPTIONS}
                label="Status"
              />
            )}
          />
          <ErrorMessage>{formState.errors?.status?.message}</ErrorMessage>
        </Field>
        <Button onClick={onClose}>Cancel</Button>
      </RuleRow>
      <Wrapper>
        <RuleHeader isOpen={isOpenConditions}>
          <span>Condition</span>
          <OptionsButton
            onClick={setIsOpenConditions}
            paintTO={theme.colors.btn.secondary}
            iconType={isOpenConditions ? "expand" : "collaps"}
          />
        </RuleHeader>
        {isOpenConditions && (
          <RuleList>
            {!!conditions.fields.length && (
              <>
                {conditions.fields.map((row, index) => (
                  <Row
                    key={row.id}
                    index={index}
                    control={control}
                    register={register}
                    watch={watch}
                    resetField={resetField}
                    rowType={"conditions"}
                    {...conditions}
                    removeRowHandler={removeRowHandler}
                    errors={formState.errors}
                  />
                ))}
              </>
            )}
            <Button
              onClick={() => {
                appendRowHandler("conditions");
              }}
            >
              Add
            </Button>
          </RuleList>
        )}
      </Wrapper>
      <GroupButton>Then</GroupButton>
      <Wrapper>
        <RuleHeader isOpen={isOpenActions}>
          <span>Action</span>
          <OptionsButton
            onClick={setIsOpenActions}
            paintTO={theme.colors.btn.secondary}
            iconType={isOpenActions ? "expand" : "collaps"}
          />
        </RuleHeader>
        {isOpenActions && (
          <RuleList>
            {!!actions.fields.length && (
              <>
                {actions.fields.map((row, index) => (
                  <Row
                    key={row.id}
                    index={index}
                    control={control}
                    register={register}
                    watch={watch}
                    resetField={resetField}
                    rowType="actions"
                    errors={formState.errors}
                    removeRowHandler={removeRowHandler}
                    {...actions}
                  />
                ))}
              </>
            )}
            <Button
              onClick={() => {
                appendRowHandler("actions");
              }}
            >
              Add
            </Button>
          </RuleList>
        )}
      </Wrapper>
      <Button disabled={!formState.isValid && formState.isSubmitSuccessful} onClick={submitForm}>
        Save
      </Button>
    </Container>
  );
}

export default RuleForm;

const Container = styled.div`
  display: flex;
  flex-direction: column;
  gap: 1rem;
  position: relative;
  border: 1px solid rgba(0, 0, 0, 0.12);
  border-top: none;
  padding: 1rem;
`;

const Wrapper = styled.div`
  display: flex;
  flex-direction: column;
  border-radius: 4px;
  gap: 0 10px;
`;

const RuleHeader = styled.div<{ isOpen?: boolean }>`
  display: flex;
  justify-content: space-between;
  align-items: center;
  border: 1px solid rgba(0, 0, 0, 0.12);
  border-radius: 4px;
  padding: 1rem;
  ${({ theme }) => theme.typography.subtitle1}
  height: ${({ isOpen }) => (isOpen ? "4rem" : "auth")};
`;
const RuleList = styled.div`
  border: 1px solid rgba(0, 0, 0, 0.12);
  border-top: none;
  border-radius: 0 0 4px 4px;
  gap: 10px;
`;
const RuleRow = styled.div`
  display: flex;
  align-items: center;
  margin: 1rem 0;
  ${({ theme }) => theme.typography.subtitle1}
  gap: 10px;
`;

const Button = styled.button`
  display: flex;
  justify-content: center;
  margin: 1rem;
  margin-left: auto;
  width: max-content;
  gap: 5px;
  height: 36px;
  align-items: center;
  background-color: ${({ theme }) => theme.colors.btn.secondary};
  color: ${({ theme }) => theme.colors.bg.ternary};
  border-style: none;

  i {
    font-size: 1.5rem;
  }
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
  margin: auto;
  width: 10ch;
  display: flex;
  justify-content: center;
  align-items: center;
  height: 2.5rem;
  background: #ffffff;
  border: 1px solid rgba(0, 0, 0, 0.12);
  border-radius: 18px;

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
