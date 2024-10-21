import React, { useState } from "react";
import { useSelector } from "react-redux";
import styled from "styled-components";

import Button from "components/button/Button";
import useBehavior from "hooks/useBehavior";

import Tag from "./Tag";
import AddTagsModal from "./AddTagsModal";
import behavior from "../behavior";
import { selectedTagsSelector } from "../selector";

const TagsFilter = () => {
  const [isModalOpen, setIsModalOpen] = useState(false);
  const { updateSelectedTags, deleteSelectedTag, deleteAllSelectedTags } = useBehavior(behavior);
  const selectedTags = useSelector(selectedTagsSelector);
  const updateFilter = tags => updateSelectedTags(tags);
  const deleteTagClickHandler = (tagId: number) => deleteSelectedTag(tagId);
  const cleanAllTagsClickHandler = () => deleteAllSelectedTags();
  const selectedTagIds = selectedTags.map(({ id }) => id);

  const renderTags = () => {
    return selectedTags.map(tag => (
      <Tag key={tag.id} title={tag.name} onClick={() => deleteTagClickHandler(tag.id)} />
    ));
  };

  return (
    <Container>
      <TagsList>{renderTags()}</TagsList>
      <ButtonsContainer>
        {Boolean(selectedTags.length) && (
          <CleanButton onClick={cleanAllTagsClickHandler} variant={"secondary"}>
            Clean all tags
          </CleanButton>
        )}
        <Button onClick={() => setIsModalOpen(true)}>View All Tags</Button>
      </ButtonsContainer>
      {isModalOpen && (
        <AddTagsModal
          title="Choose tags to display"
          onApply={updateFilter}
          onClose={() => setIsModalOpen(false)}
          tags={selectedTags}
          selectedTagIds={selectedTagIds}
        />
      )}
    </Container>
  );
};

export default TagsFilter;

const Container = styled.div`
  display: flex;
  justify-content: space-between;
  align-items: center;
  margin-bottom: 1rem;
`;

const ButtonsContainer = styled.div`
  display: flex;
  gap: 16px;
`;

const CleanButton = styled(Button)``;

const TagsList = styled.div`
  display: flex;
  gap: 8px;
`;
