import React, { useState, useRef, useMemo } from "react";
import Tooltip from "../tooltip/Tooltip";
import styled from "styled-components";
import { useFonts } from "hooks/useFonts";
import useWindowSize from "hooks/useWindowSize";
import { getAmountToRender } from "./utils";

interface ITagsRowProps {
  id: string | number;
  tags: { id: number; name: string; value: number }[];
}

function TagsRow({ id, tags }: ITagsRowProps) {
  const [isPopoverOpen, setIsPopoverOpen] = useState(false);
  const { windowResized } = useWindowSize(400);
  const containerRef = useRef<HTMLDivElement>();

  const isFontLoaded = useFonts("Inter regular");

  const amountToRender = useMemo(
    () => getAmountToRender(isFontLoaded, tags, containerRef),
    // eslint-disable-next-line react-hooks/exhaustive-deps
    [windowResized, tags, isFontLoaded, containerRef]
  );

  const hasMoreTags = amountToRender < tags.length;
  const moreTags = tags.slice(amountToRender, tags.length);

  const handleMouseOver = () => {
    setIsPopoverOpen(true);
  };

  const handleMouseOut = () => {
    setIsPopoverOpen(false);
  };

  const renderTagsTooltip = () => (
    <TagsTooltipContainer>
      {moreTags.map(({ id, name }) => (
        <Tag key={id}>{name}</Tag>
      ))}
    </TagsTooltipContainer>
  );

  return (
    <TagsContainer ref={containerRef} hidden={!isFontLoaded}>
      <Tags>
        {tags.slice(0, amountToRender).map(({ id, name }) => (
          <Tag key={id}>{name}</Tag>
        ))}
      </Tags>
      {hasMoreTags && (
        <Tooltip id={id} tooltip={renderTagsTooltip()} isOpen={isPopoverOpen}>
          <TagsCount
            onClick={() => setIsPopoverOpen(!isPopoverOpen)}
            onMouseOver={handleMouseOver}
            onMouseOut={handleMouseOut}
          >
            +{moreTags.length}
          </TagsCount>
        </Tooltip>
      )}
    </TagsContainer>
  );
}

export default TagsRow;

const TagsContainer = styled.div`
  box-sizing: border-box;
  display: flex;
  justify-content: space-between;
  align-items: center;
  min-height: 42px;
  margin: 0 0 0 7px;
  padding: 15px 0 0;
  ${({ theme }) => theme.typography.smallText1};
  overflow: hidden;
  visibility: ${({ hidden }) => (hidden ? "hidden" : "visible")};
`;

const Tags = styled.div`
  display: flex;
`;

const TagsTooltipContainer = styled.div`
  max-width: 330px;
  display: flex;
  flex-wrap: wrap;
  row-gap: 10px;
  justify-content: center;
`;

const TagsCount = styled.div`
  cursor: pointer;
  color: ${({ theme }) => theme.colors.fg.link};
  ${({ theme }) => theme.typography.body1};
`;

const Tag = styled.div`
  position: relative;
  padding: 5px 12px 5px 20px;
  margin: 0 7px 0 0;
  white-space: nowrap;
  background: rgba(82, 148, 195, 0.12);
  border-radius: 18px;

  &:before {
    content: "";
    position: absolute;
    top: calc(50% - 3px);
    left: 8px;
    width: 6px;
    height: 6px;
    background: white;
    border-radius: 50%;
  }
`;
