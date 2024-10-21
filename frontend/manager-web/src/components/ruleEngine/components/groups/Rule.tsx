import { useSelector } from "react-redux";
import styled, { useTheme } from "styled-components";

import OptionsButton from "components/button/OptionsButton";
import { getRuleById } from "components/ruleEngine/groupSelector";
import { RuleTypes, Status } from "components/ruleEngine/types";
import ActiveStatusIcon from "components/icons/ActiveStatusIcon";
import Conditions from "components/ruleEngine/components/groups/Conditions";
import Actions from "components/ruleEngine/components/groups/Actions";
import useToggle from "hooks/useToggle";
import EditRule from "components/ruleEngine/components/rules/EditRule";
import useBehavior from "hooks/useBehavior";
import groupsBehavior from "components/ruleEngine/groupBehavior";
import ConfirmationModal from "components/confirmationModal/ConfirmationModal";
import DisabledStatusIcon from "components/icons/DisabledStatusIcon";

export interface IRule {
  ruleName: string;
  status: Status;
  rule?: {
    conditions: string;
    actions: string;
  };
  ruleId?: string | number;
  groupId?: string | number;
  ruleType?: RuleTypes;
}

function Rule({ ruleName, status, ruleId, groupId, ruleType }: IRule) {
  const theme = useTheme();
  const [isEditMode, setIsEditMode] = useToggle();
  const [isConfirmationModalShown, setIsConfirmationModalShown] = useToggle();
  const rules = useSelector(getRuleById(groupId, ruleId));
  const { removeRule } = useBehavior(groupsBehavior);
  const isDisabled = status === Status.DISABLE;

  return isEditMode ? (
    <EditRule ruleType={ruleType} urlParams={{ groupId, ruleId }} onClose={setIsEditMode} />
  ) : (
    <>
      <Wrapper isDisabled={isDisabled}>
        <Title>{ruleName}</Title>
        <StatusView>
          {status === Status.DISABLE ? <DisabledStatusIcon /> : <ActiveStatusIcon />}
        </StatusView>
        <Conditions ruleData={isEditMode ? null : rules.ruleData} />
        <Actions ruleData={isEditMode ? null : rules.ruleData} />
        <Options>
          <OptionsButton
            paintTO={theme.colors.btn.secondary}
            onClick={setIsEditMode}
            iconType="edit"
          />
          <OptionsButton
            paintTO={theme.colors.btn.error_secondary}
            iconType="close"
            onClick={() => setIsConfirmationModalShown()}
          />
        </Options>
      </Wrapper>
      {isConfirmationModalShown && (
        <ConfirmationModal
          title={`Remove rule ${ruleName}`}
          onConfirm={() => removeRule({ groupId, ruleId, ruleType })}
          onCancel={() => setIsConfirmationModalShown()}
        >
          Are you sure you want to delete rule {ruleName}?
        </ConfirmationModal>
      )}
    </>
  );
}

export default Rule;

const Wrapper = styled.div<{ isDisabled: boolean }>`
  opacity: ${({ isDisabled }) => (isDisabled ? 0.5 : 1)};
  display: flex;
  align-items: center;
  min-height: 3rem;
  padding: 1rem;
  border: 1px solid rgba(0, 0, 0, 0.12);
  border-top-left-radius: 4px;
  border-top-right-radius: 4px;
  border-top: none;
`;

const Title = styled.span`
  flex: 3 1 auto;
  max-width: 20ch;
  ${({ theme }) => theme.typography.subtitle4};
`;

const StatusView = styled.span`
  flex: 2 1 30px;
  display: flex;
  justify-content: center;
`;

const Options = styled.div`
  flex: 0 0 auto;
  display: flex;
  gap: 1rem;
`;
