import React, { useState, useRef, useEffect } from "react";
import styled, { css } from "styled-components";
import Input from "components/input/Input";
import Checkbox from "components/multiSelect/Checkbox";
import { useOutsideAlerter } from "hooks/useOutsideAlterer";
import LeadQueueSettingsIcon from "components/icons/LeadQueueSettingsIcon";
import useGroupsSettings from "pages/tracking/groups/useGroupsSettings";

type Attribute = {
  key: string;
  label: string;
  value: number | string;
  editable?: boolean;
};

export interface ICardData {
  id: number;
  name: string;
  teamId?: number;
  attributes: Array<Attribute>;
  dropRateParameters: {
    dropRateUpperThreshold: number;
    dropRateLowerThreshold: number;
    dropRatePeriod: number;
    ratioStep: number;
    maxRatio: number | null;
    minRatio: number | null;
    ratioFreezeTime: number;
  };
}

export interface IInfoCard {
  className?: string;
  containerClassName?: string;
  data: ICardData;
  color?: string;
  selectable?: boolean;
  isSelected?: boolean;
  onSelect?: (teamId: number, isSelected: boolean) => void;
  onAttributeEdit?: (ICardData, attribute) => void;
}

const InfoCard: React.FC<IInfoCard> = ({
  data,
  className,
  selectable,
  isSelected,
  onAttributeEdit,
  onSelect,
  color,
}) => {
  const [editingKey, setEditingKey] = useState(null);
  const inputRef = useRef(null);
  const { toggleModal, groupSettingsModal } = useGroupsSettings({
    queueId: data.id,
    dropRateParameters: data.dropRateParameters,
    groupName: data.name,
  });
  const setEditing = ({ key, editable }: Attribute) => {
    if (editable) {
      setEditingKey(key);
    }
  };

  const handleKeyPress = e => {
    if (e.code === "Enter") {
      onAttributeEdit(data, { [editingKey]: inputRef.current?.value });
      setEditingKey(null);
    }
  };

  useEffect(() => {
    if (editingKey) {
      inputRef.current?.focus();
    }
  }, [editingKey]);

  useOutsideAlerter(
    inputRef,
    () => {
      onAttributeEdit(data, { [editingKey]: inputRef.current?.value });
      setEditingKey(null);
    },
    [editingKey]
  );

  const handleChange = () => {
    if (selectable) {
      onSelect(data.teamId, !isSelected);
    }
  };

  return (
    <Container className={className} selectable={selectable} onClick={handleChange}>
      <Header>
        {groupSettingsModal()}
        <Name paintTo={color}>
          {selectable ? (
            <StyledCheckbox isSelected={isSelected}>{data.name}</StyledCheckbox>
          ) : (
            data.name
          )}
        </Name>
        <LeadQueueSettingsIcon onClick={toggleModal} />
      </Header>
      <InfoBlocksContainer isSelected={isSelected}>
        {data.attributes.map((attr: Attribute) => (
          <InfoBlock key={attr.key}>
            <InfoBlockTop>{attr.label}</InfoBlockTop>
            <InfoBlockBottom editable={attr.editable} onClick={() => setEditing(attr)}>
              {editingKey === attr.key ? (
                <StyledInput
                  type="number"
                  min={0}
                  defaultValue={attr.value}
                  ref={inputRef}
                  onKeyPress={handleKeyPress}
                />
              ) : (
                attr.value
              )}
            </InfoBlockBottom>
          </InfoBlock>
        ))}
      </InfoBlocksContainer>
    </Container>
  );
};

export default InfoCard;

const activeCss = css`
  border: 1px solid ${({ theme }) => theme.colors.border.active};
  box-shadow: 0 0 3pt 2pt ${({ theme }) => theme.colors.border.activeRadius};
`;

const Container = styled.div<{ selectable: boolean }>`
  box-sizing: border-box;
  display: flex;
  flex-direction: column;
  border: 1px solid ${({ theme }) => theme.colors.border.primary};
  border-radius: 4px;
  box-shadow: 0 1px 4px ${({ theme }) => theme.colors.border.primary};
  cursor: ${({ selectable }) => (selectable ? "pointer" : "default")};

  &:hover {
    ${({ selectable }) => selectable && activeCss};
  }
`;

const Header = styled.div`
  display: flex;
  justify-content: space-between;
  background-color: white;
  align-items: center;
  padding: 16px;

  position: relative;
  svg {
    cursor: pointer;
  }
`;

const Name = styled.div<{ paintTo: string }>`
  display: flex;
  flex-direction: row;
  justify-content: space-between;
  align-items: center;
  background: white;
  border-radius: 4px 4px 0 0;
  ${({ theme }) => theme.typography.subtitle2}
  color: ${({ theme, paintTo: color }) => color || theme.colors.btn.secondary_pressed};
`;

const InfoBlocksContainer = styled.div<{ isSelected: boolean }>`
  display: flex;
  padding: 14px 16px 10px;
  background: ${({ theme, isSelected }) => (isSelected ? "#E5F0FF" : theme.colors.bg.light)};
  border-radius: 0 0 8px 8px;
`;
const InfoBlock = styled.div`
  flex: 1;
  display: flex;
  flex-direction: column;
`;
const InfoBlockTop = styled.div`
  margin: 0 0 5px;
  ${({ theme }) => theme.typography.smallText3};
  color: ${({ theme }) => theme.colors.fg.secondary_light};
  text-transform: uppercase;
`;
const InfoBlockBottom = styled.div<{ editable?: boolean }>`
  display: flex;
  align-items: center;
  min-height: 24px;
  ${({ theme }) => theme.typography.body2};
  text-transform: uppercase;
  cursor: ${({ editable }) => (editable ? "pointer" : "inherit")};
  color: ${({ theme }) => theme.colors.fg.secondary};
`;

const StyledInput = styled(Input)`
  position: relative;
  top: 0;
  width: 100%;
  max-width: 70px;
  height: 24px;

  span {
    border-radius: 4px;
  }

  input {
    height: 24px;
    padding: 0 0 0 3px;
    ${({ theme }) => theme.typography.smallText2}
  }
`;

const StyledCheckbox = styled(Checkbox)`
  margin-right: 20px;
`;
