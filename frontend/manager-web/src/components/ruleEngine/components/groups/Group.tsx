import { useForm } from "react-hook-form";
import styled, { CSSObject, useTheme } from "styled-components";
import { useRef } from "react";

import useToggle from "hooks/useToggle";
import useBehavior from "hooks/useBehavior";
import OptionsButton from "components/button/OptionsButton";
import Rule, { IRule } from "components/ruleEngine/components/groups/Rule";
import groupsBehavior from "components/ruleEngine/groupBehavior";
import { RuleTypes, Status } from "components/ruleEngine/types";
import GroupForm from "components/ruleEngine/components/groups/GroupForm";
import { STATUS_OPTIONS } from "components/ruleEngine/constants";
import CreateRule from "components/ruleEngine/components/rules/CreateRule";
import DraggableRuleIcon from "components/ruleEngine/components/groups/DraggableRuleIcon";
import GlobeIcon from "components/ruleEngine/components/groups/GlobeIcon";
import Button from "components/button/Button";
import ConfirmationModal from "components/confirmationModal/ConfirmationModal";
import useExpandCollapseAnimation from "hooks/useExpandCollapseAnimation";
import ActiveStatusIcon from "components/icons/ActiveStatusIcon";
import DisabledStatusIcon from "components/icons/DisabledStatusIcon";

interface IRuleGroup {
  groupName: string;
  groupId: number | string;
  status: Status;
  ruleType: RuleTypes;
  rules: Array<IRule>;
}

interface IFormValues {
  groupName: string;
  status: {
    value: string;
    label: string;
  };
}

const COLLAPSED_GROUP_HEIGHT = 97;

function RuleGroup({ groupName, status, groupId, rules, ruleType }: IRuleGroup) {
  const { editGroup, removeGroup } = useBehavior(groupsBehavior);
  const rulesContainerRef = useRef(null);

  const [isEditMode, setIsEditMode] = useToggle();
  const [isOpen, setIsOpen] = useToggle();
  const [isAddMode, setIsAddMode] = useToggle();
  const [isConfirmationModalShown, setIsConfirmationModalShown] = useToggle();

  const { styles } = useExpandCollapseAnimation({
    isExpanded: isOpen,
    minHeight: COLLAPSED_GROUP_HEIGHT,
    height:
      rulesContainerRef.current?.clientHeight + COLLAPSED_GROUP_HEIGHT || COLLAPSED_GROUP_HEIGHT,
    autoHeight: true,
  });
  const { handleSubmit, getValues, control, formState } = useForm<IFormValues>({
    defaultValues: {
      groupName,
      status: STATUS_OPTIONS.find(opt => opt.value === status) ?? STATUS_OPTIONS[1],
    },
  });
  const groupField = getValues();
  const theme = useTheme();

  return (
    <Container styles={styles}>
      <Wrapper>
        <GroupContent>
          {isEditMode ? (
            <GroupForm control={control} errors={formState.errors} options={STATUS_OPTIONS} />
          ) : (
            <>
              <DraggableRuleIcon style={{ marginRight: "1rem" }} />
              <CountryIcon />
              <Title>{groupField.groupName}</Title>
              <StatusView>
                {groupField.status.value === Status.ACTIVE ? (
                  <ActiveStatusIcon />
                ) : (
                  <DisabledStatusIcon />
                )}
                {groupField.status.label}
              </StatusView>
            </>
          )}
          <Options>
            {isOpen ? (
              <>
                {isEditMode ? (
                  <>
                    <Button
                      onClick={handleSubmit(
                        editGroup({
                          toggle: setIsEditMode,
                          groupId,
                          ruleType,
                        })
                      )}
                    >
                      Save
                    </Button>
                    <Button onClick={setIsEditMode} variant="secondary">
                      Cancel
                    </Button>
                  </>
                ) : (
                  <Button onClick={setIsEditMode}>
                    <i className="icon-edit" />
                    Rename
                  </Button>
                )}
              </>
            ) : (
              <>
                <OptionsButton
                  paintTO={theme.colors.btn.secondary}
                  iconType="edit"
                  onClick={() => {
                    setIsEditMode();
                    setIsOpen();
                  }}
                />
                <OptionsButton
                  paintTO={theme.colors.btn.error_secondary}
                  onClick={() => setIsConfirmationModalShown()}
                  iconType="close"
                />
              </>
            )}
            <OptionsButton
              onClick={setIsOpen}
              paintTO={theme.colors.btn.secondary}
              iconType={isOpen ? "expand" : "collaps"}
            />
          </Options>
        </GroupContent>
        <RulesContainer styles={styles} ref={rulesContainerRef}>
          {rules.map(rule => (
            <Rule
              groupId={groupId}
              key={rule.ruleId}
              ruleId={rule.ruleId}
              ruleName={rule.ruleName}
              status={rule.status}
              ruleType={ruleType}
            />
          ))}
          {isAddMode ? (
            <CreateRule onClose={setIsAddMode} ruleType={ruleType} urlParams={{ groupId }} />
          ) : (
            <ButtonWrapper>
              <Button onClick={setIsAddMode}>Add rule</Button>
            </ButtonWrapper>
          )}
        </RulesContainer>
      </Wrapper>
      {isConfirmationModalShown && (
        <ConfirmationModal
          title={`Remove group ${groupName}`}
          onConfirm={() => removeGroup({ groupId, ruleType })}
          onCancel={() => setIsConfirmationModalShown()}
        >
          Are you sure you want to delete group {groupName}?
        </ConfirmationModal>
      )}
    </Container>
  );
}

export default RuleGroup;

const Container = styled.div<{ styles: CSSObject }>`
  margin: 1rem 0 0;
  ${({ styles }) => styles};
`;

const Wrapper = styled.div`
  display: flex;
  flex-direction: column;
`;

const GroupContent = styled.div`
  box-sizing: border-box;
  display: flex;
  align-items: center;
  min-height: ${COLLAPSED_GROUP_HEIGHT}px;
  padding: 0 1rem;
  border: 1px solid rgba(0, 0, 0, 0.12);
  border-radius: 4px;

  form {
    display: flex;
    gap: 10px;
    flex: 8 1 auto;
  }
`;

const RulesContainer = styled.div<{ styles: CSSObject }>`
  display: flex;
  flex-direction: column;
`;

const CountryIcon = styled(GlobeIcon)`
  width: 64px;
  height: 64px;
  margin-right: 15px;
`;

const Title = styled.span`
  flex: 3 1 auto;
  max-width: 20ch;
  ${({ theme }) => theme.typography.subtitle1};
  text-transform: uppercase;

  input {
    max-width: 20ch;
  }
`;

const StatusView = styled.span`
  flex: 5 1 auto;
  display: flex;
  gap: 1rem;
  align-items: center;
  ${({ theme }) => theme.typography.body1};
  font-weight: 400;
  option {
    ${({ theme }) => theme.typography.body1};
  }
`;

const Options = styled.div`
  flex: 0 0 auto;
  display: flex;
  gap: 1rem;
`;

const ButtonWrapper = styled.div`
  display: flex;
  align-items: center;
  justify-content: flex-end;
  min-height: 2rem;
  padding: 1rem;
  border: 1px solid rgba(0, 0, 0, 0.12);
  border-top: none;
  border-bottom-right-radius: 4px;
  border-bottom-left-radius: 4px;
`;
