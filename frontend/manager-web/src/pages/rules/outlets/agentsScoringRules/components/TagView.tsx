import { FC } from "react";
import styled, { useTheme } from "styled-components";
import { useForm } from "react-hook-form";

import useBehavior from "hooks/useBehavior";
import useToggle from "hooks/useToggle";
import OptionsButton from "components/button/OptionsButton";
import ActiveStatusIcon from "components/icons/ActiveStatusIcon";
import { typography } from "globalStyles/theme/fonts";
import behavior from "pages/tags/behavior";
import {
  ACTION_OPTIONS,
  DURATION_OPTIONS,
  IFormValues,
  IScoringRule,
  STATUS_OPTIONS,
} from "pages/tags/types";
import { getDurationText, getValueText } from "pages/tags/utils";
import { TagStatus } from "types";

import AddTags from "./AddTags";

interface IProps extends IScoringRule {
  isEdit?: boolean;
}

const TagView: FC<IProps> = ({ id, name, status, value, lifetimeSeconds, isEdit = false }) => {
  const { deleteRule, updateRule } = useBehavior(behavior);
  const { handleSubmit, getValues, control } = useForm<IFormValues>({
    defaultValues: {
      name,
      value: Math.abs(value),
      status: status === TagStatus.Enable ? STATUS_OPTIONS[0] : STATUS_OPTIONS[1],
      action: value >= 0 ? ACTION_OPTIONS[0] : ACTION_OPTIONS[1],
      lifetimeSeconds:
        DURATION_OPTIONS.find(({ value }) => value === lifetimeSeconds) || DURATION_OPTIONS.at(-1),
    },
  });
  const theme = useTheme();
  const [isEditMode, toggleIsEditMode] = useToggle(isEdit);
  const handleDeleteButtonClick = () => deleteRule(id);
  const isStatusEnable = status === TagStatus.Enable;

  return (
    <Container>
      {isEditMode ? (
        <AddTags
          control={control}
          handleSubmit={handleSubmit(() => {
            updateRule({
              id,
              ...getValues(),
            });
            toggleIsEditMode();
          })}
          handleCancel={toggleIsEditMode}
          buttonTitle="Apply"
        />
      ) : (
        <>
          <Title>{name}</Title>
          <StatusView>
            {isStatusEnable ? (
              <ActiveStatusIcon />
            ) : (
              <i className="icon-stop">
                <span className="path1" />
                <span className="path2" />
              </i>
            )}
          </StatusView>
          <ValueText>{getValueText(value)}</ValueText>
          <Duration>{getDurationText(lifetimeSeconds)}</Duration>
          <ButtonsContainer>
            <OptionsButton
              paintTO={theme.colors.btn.secondary}
              onClick={toggleIsEditMode}
              iconType="edit"
            />
            <OptionsButton
              paintTO={theme.colors.error}
              onClick={handleDeleteButtonClick}
              iconType="close"
            />
          </ButtonsContainer>
        </>
      )}
    </Container>
  );
};

const Container = styled.div`
  display: flex;
  align-items: center;
  border: 1px solid rgba(0, 0, 0, 0.12);
  border-radius: 4px;
  padding: 21px 16px;
  font-family: "Inter regular";
`;

const Title = styled.div`
  ${typography.subtitle1}
  text-transform: uppercase;
  width: 30%;
`;

const StatusView = styled.div`
  width: 5%;

  svg {
    width: 18px;
  }
`;

const ValueText = styled.div`
  ${typography.body1}
  width: 25%;
`;

const Duration = styled.div`
  ${typography.body1}
  width: 15%;
`;

const ButtonsContainer = styled.div`
  display: flex;
  justify-content: flex-end;
  gap: 16px;
  width: 35%;
`;

export default TagView;
