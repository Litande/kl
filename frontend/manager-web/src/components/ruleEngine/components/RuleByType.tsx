import { useEffect } from "react";
import { useForm } from "react-hook-form";
import styled from "styled-components";
import { useSelector } from "react-redux";

import useBehavior from "hooks/useBehavior";
import groupBehavior from "components/ruleEngine/groupBehavior";
import RuleGroup from "components/ruleEngine/components/groups/Group";
import GroupForm from "components/ruleEngine/components/groups/GroupForm";
import { groupsSelector, isLoadingSelector } from "components/ruleEngine/groupSelector";
import { RuleTypes } from "components/ruleEngine/types";
import { STATUS_OPTIONS } from "components/ruleEngine/constants";
import LoadingOverlay from "components/loadingOverlay/LoadingOverlay";

interface IFormValues {
  groupName: string;
  status: {
    value: string;
    label: string;
  };
}

function RuleByType({ ruleType }: { ruleType: RuleTypes }) {
  const isLoading = useSelector(isLoadingSelector);
  const { getRuleGroups, addGroup } = useBehavior(groupBehavior);
  const ruleEngineGroups = useSelector(groupsSelector);
  const { handleSubmit, control, reset, formState } = useForm<IFormValues>({
    defaultValues: { groupName: "", status: STATUS_OPTIONS[1] },
  });

  useEffect(() => {
    getRuleGroups(ruleType);
    // eslint-disable-next-line react-hooks/exhaustive-deps
  }, []);

  useEffect(() => {
    if (formState.isSubmitSuccessful) {
      reset({ groupName: "", status: STATUS_OPTIONS[1] });
    }
  }, [formState, reset]);

  return (
    <Container>
      {ruleEngineGroups.map(el => (
        <RuleGroup
          key={el.groupId}
          groupId={el.groupId}
          groupName={el.groupName}
          status={el.status}
          rules={el.rules}
          ruleType={ruleType}
        />
      ))}
      <ButtonWrapper>
        <GroupForm
          control={control}
          options={STATUS_OPTIONS}
          errorMessage={formState?.errors?.groupName?.message}
        />
        <Button onClick={handleSubmit(addGroup({ ruleType }))}>Add rule</Button>
      </ButtonWrapper>
      {isLoading && <LoadingOverlay />}
    </Container>
  );
}

export default RuleByType;

const Container = styled.div`
  position: relative;
  height: 100%;
`;

const Button = styled.button`
  display: flex;
  justify-content: center;
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

const ButtonWrapper = styled.div`
  display: flex;
  align-items: center;
  min-height: 2rem;
  padding: 1rem;
  border-radius: 4px;

  form {
    display: flex;
    flex: 5 1 auto;
    gap: 10px;
  }
`;
