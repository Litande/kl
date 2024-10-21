import styled from "styled-components";
import { useSelector } from "react-redux";
import { useForm } from "react-hook-form";

import useBehavior from "hooks/useBehavior";
import useToggle from "hooks/useToggle";
import Button from "components/button/Button";
import { IFormValues } from "pages/tags/types";
import behavior from "pages/tags/behavior";
import { scoringRulesSelector } from "pages/tags/selector";

import AddTags from "./components/AddTags";
import TagView from "./components/TagView";
import { useEffect, useState } from "react";
import LoadingOverlay from "components/loadingOverlay/LoadingOverlay";

const AgentsScoringRules = () => {
  const [isLoading, setIsLoading] = useState(true);
  const { createRule, getAllTags } = useBehavior(behavior);
  const [isShowForm, toggleIsShowForm] = useToggle(false);
  const scoringRules = useSelector(scoringRulesSelector);
  const { handleSubmit, control, reset } = useForm<IFormValues>({
    defaultValues: {
      name: "",
    },
  });

  const handleCancel = () => {
    toggleIsShowForm();
    reset();
  };

  useEffect(() => {
    getAllTags()
      .then()
      .finally(() => {
        setIsLoading(false);
      });
    // eslint-disable-next-line react-hooks/exhaustive-deps
  }, []);

  const renderScoringRules = scoringRules.map(({ id, name, value, lifetimeSeconds, status }) => (
    <TagView
      id={id}
      name={name}
      status={status}
      value={value}
      lifetimeSeconds={lifetimeSeconds}
      key={id}
    />
  ));

  return (
    <Container>
      {renderScoringRules}
      {isShowForm && (
        <AddTags
          control={control}
          handleSubmit={handleSubmit(
            createRule({
              toggle: toggleIsShowForm,
              reset,
            })
          )}
          handleCancel={handleCancel}
        />
      )}
      <ButtonsContainer isVisible={!isShowForm}>
        <Button onClick={toggleIsShowForm}>Add Tags</Button>
      </ButtonsContainer>
      {isLoading && <LoadingOverlay />}
    </Container>
  );
};

export default AgentsScoringRules;

const Container = styled.div`
  position: relative;
  display: flex;
  flex-direction: column;
  padding-top: 16px;
  gap: 16px;
  overflow-y: auto;
  height: calc(100vh - 265px);
  padding-bottom: 16px;
`;

type ButtonsContainerProps = {
  isVisible: boolean;
};

const ButtonsContainer = styled.div<ButtonsContainerProps>`
  justify-content: flex-end;
  display: ${({ isVisible }) => (isVisible ? "flex" : "none")};
`;
