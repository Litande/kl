import React, { FC, useEffect, useRef } from "react";
import { useSelector } from "react-redux";
import styled from "styled-components";
import Button from "components/button/Button";
import CheckboxWithInnerState from "components/checkbox/CheckboxWithInnerState";

import { enableScoringRulesSelector } from "../selector";
import { ISelectedTag } from "../types";
import ModalV2 from "components/modal/ModalV2";

type Props = {
  title: string;
  onApply: (selectedTags) => void;
  onClose: () => void;
  tags: Array<ISelectedTag>;
  selectedTagIds?: Array<number>;
};

const AddTagsModal: FC<Props> = ({ title, onApply, onClose, tags, selectedTagIds = [] }) => {
  const scoringRules = useSelector(enableScoringRulesSelector);
  const selectedNewTagsRef = useRef(tags);

  useEffect(() => {
    selectedNewTagsRef.current = tags;
  }, [tags]);

  const handleAdd = () => {
    onApply(selectedNewTagsRef.current);
    onClose();
  };

  const handleCheckBoxChange = ({ id, isSelected, name }) => {
    const getNewItemsOnAdd = () => [
      ...selectedNewTagsRef.current,
      {
        id,
        name,
      },
    ];
    const getNewItemsOnDelete = () =>
      [...selectedNewTagsRef.current].filter(({ id: tagId }) => tagId !== id);

    selectedNewTagsRef.current = isSelected ? getNewItemsOnAdd() : getNewItemsOnDelete();
  };

  const renderTags = scoringRules.length
    ? scoringRules.map(({ name, id }) => {
        const isSelected = !!selectedTagIds.find(tagId => tagId === id);
        return (
          <CheckboxWithInnerState
            key={id}
            isSelected={isSelected}
            onChange={isSelected => {
              handleCheckBoxChange({
                id,
                name,
                isSelected,
              });
            }}
          >
            {name}
          </CheckboxWithInnerState>
        );
      })
    : "No Tags";

  return (
    <StyledModal onCancel={onClose} title={title} hasCloseIcon>
      <Content>{renderTags}</Content>
      <ButtonsContainer>
        <StyledButton variant="secondary" onClick={onClose}>
          Cancel
        </StyledButton>
        <StyledButton onClick={handleAdd}>Add</StyledButton>
      </ButtonsContainer>
    </StyledModal>
  );
};

export default AddTagsModal;

const StyledModal = styled(ModalV2)`
  width: 400px;
  height: 380px;
  box-shadow: 0 2px 4px rgba(0, 0, 0, 0.16);
  border-radius: 4px;
  background: ${({ theme }) => theme.colors.bg.ternary};
`;

const Content = styled.div`
  box-sizing: border-box;
  flex: 1;
  display: flex;
  flex-direction: column;
  width: 100%;
  max-height: 235px;
  gap: 25px;
  padding: 0 0 15px;
  margin: 0 0 15px;
  overflow: hidden;
  overflow-y: auto;
  ${({ theme }) => theme.typography.body1}
`;

const ButtonsContainer = styled.div`
  display: flex;
  justify-content: space-between;
  width: 100%;
`;

const StyledButton = styled(Button)`
  width: 170px;
`;
