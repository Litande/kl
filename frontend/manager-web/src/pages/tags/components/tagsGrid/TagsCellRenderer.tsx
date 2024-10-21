import React, { FC, useEffect, useRef, useState } from "react";
import styled from "styled-components";

import useBehavior from "hooks/useBehavior";
import { IRow, ITag } from "pages/tags/types";
import behavior from "pages/tags/behavior";
import useToggle from "hooks/useToggle";
import ArrowExpand from "components/arrowExpand/ArrowExpand";
import useWindowSize from "hooks/useWindowSize";
import useTimeout from "hooks/useTimeout";
import { isOverflown } from "pages/tags/utils";

import Tag from "../Tag";
import AddTagsModal from "../AddTagsModal";

type Props = {
  value: Array<ITag>;
  data: IRow;
  rowIndex: number;
};

const TagsCellRenderer: FC<Props> = ({ value: tags, data, rowIndex }) => {
  const [isExpand, toggleExpand] = useToggle();
  const [isExpandVisible, setIsExpandVisible] = useState(false);
  const { updateAgent } = useBehavior(behavior);
  const [isModalOpen, setIsModalOpen] = useState(false);
  const { windowResized } = useWindowSize(300);
  const selectedTagIds = tags.map(({ id }) => id);
  const containerRef = useRef();

  const onApply = tags =>
    updateAgent({
      ...data,
      tagIds: tags.map(({ id }) => id),
    });

  const removeTag = tagId => {
    const tagIds = data.tags.filter(({ id }) => id !== tagId).map(({ id }) => id);

    updateAgent({
      ...data,
      tagIds,
    });
  };

  const handleExpand = () => {
    toggleExpand();
  };

  useTimeout(() => {
    setIsExpandVisible(isOverflown(containerRef.current));
  }, 300);

  useEffect(() => {
    setIsExpandVisible(isOverflown(containerRef.current));
  }, [windowResized, tags.length]);

  return (
    <Wrap>
      <TagsContainer ref={containerRef} isExpand={isExpand}>
        {tags.map(({ id, name }) => (
          <Tag key={id} title={name} onClick={() => removeTag(id)} />
        ))}
      </TagsContainer>

      <ButtonsContainer>
        <ArrowExpand isVisible={isExpandVisible} onClick={handleExpand} />
        <FilterIcon onClick={() => setIsModalOpen(true)} className="icon-add-tag"></FilterIcon>
      </ButtonsContainer>
      {isModalOpen && (
        <AddTagsModal
          title="Add tags"
          onApply={onApply}
          onClose={() => setIsModalOpen(false)}
          tags={tags}
          selectedTagIds={selectedTagIds}
        />
      )}
    </Wrap>
  );
};

export default TagsCellRenderer;

const Wrap = styled.div`
  display: flex;
  gap: 8px;
  width: 100%;
  justify-content: space-between;
  padding-top: 10px;
  padding-bottom: 10px;
`;

const TagsContainer = styled.div<{ isExpand: boolean }>`
  display: flex;
  gap: 8px;
  flex-wrap: wrap;
  flex-direction: row;
  height: ${({ isExpand }) => (isExpand ? "inherit" : `37px`)};
  align-items: center;
  overflow: hidden;
`;

const ButtonsContainer = styled.div`
  display: flex;
  gap: 8px;
  align-items: flex-start;
`;

const FilterIcon = styled.i`
  border: 1px solid ${({ theme }) => theme.colors.border.secondary};
  border-radius: 4px;
  padding: 5px;
  font-size: 24px;
  cursor: pointer;
  
  ::before {
    color: ${({ theme }) => theme.colors.border.secondary};
  }
}
`;
