import styled, { useTheme } from "styled-components";
import React from "react";
import OptionsButton from "components/button/OptionsButton";
import { useSelector } from "react-redux";
import { filteredAgentsSelector } from "./selector";
import { columnDefs } from "./constants";
import { IOption } from "./types";
import DraggableRuleIcon from "components/ruleEngine/components/groups/DraggableRuleIcon";

type HeaderProps = {
  group: IOption;
  isOpen: boolean;
  height: number;
};

export const HEADER_HEIGHT = 105;

const Header = ({ group, isOpen }: HeaderProps) => {
  const theme = useTheme();
  return (
    <Container>
      <TitleWrap>
        <TitleLabelWrap>
          <DraggableRuleIcon style={{ marginRight: "0.8rem" }} />
          {group.label}
        </TitleLabelWrap>
        <TitleCollapseButtonWrap>
          <OptionsButton
            paintTO={theme.colors.btn.secondary}
            iconType={isOpen ? "expand" : "collaps"}
          />
        </TitleCollapseButtonWrap>
      </TitleWrap>
      <StatusRow isOpen={isOpen} />
    </Container>
  );
};

type StatusRowProps = {
  isOpen: boolean;
};

const StatusRow = ({ isOpen }: StatusRowProps) => {
  const filteredAgents = useSelector(filteredAgentsSelector);

  return (
    <StatusRowWrap isOpen={isOpen}>
      {columnDefs.map(({ key, label }) => {
        const agentsByGroup = filteredAgents.filter(agent =>
          Array.isArray(key) ? key.some(i => i === agent.state) : agent.state === key
        );

        return (
          <StatusItem key={Array.isArray(key) ? key.join("-") : key}>
            <div>{label}</div>
            <StatusItemAmount>{agentsByGroup.length}</StatusItemAmount>
          </StatusItem>
        );
      })}
    </StatusRowWrap>
  );
};

export const HeaderHelper = React.memo(Header);

const Container = styled.div`
  height: ${HEADER_HEIGHT}px;
`;

const StatusRowWrap = styled.div<{ isOpen: boolean }>`
  display: flex;
  flex-direction: row;
  flex-wrap: nowrap;
  padding: 0.9rem 0 1rem;
  gap: 20px;
  background: ${({ theme, isOpen }) => theme.colors.bg.light};
  ${({ theme }) => theme.typography.smallText1};
  font-weight: 400;
  text-transform: uppercase;
  color: ${({ theme }) => theme.colors.fg.secondary_light};
`;
const StatusItem = styled.div`
  flex: 1;
  display: flex;
  flex-direction: row;
  justify-content: space-between;
  align-items: center;
  padding: 0 15px;
`;

const StatusItemAmount = styled.div`
  box-sizing: border-box;
  text-align: center;
  min-width: 35px;
  padding: 3px 10px;
  background: #ffffff;
  border-radius: 4px;
  color: ${({ theme }) => theme.colors.btn.secondary};
`;

const TitleWrap = styled.div`
  cursor: pointer;
  display: flex;
  width: 100%;
  flex-direction: row;
  padding: 0.7rem 0;
  text-transform: uppercase;
  ${({ theme }) => theme.typography.subtitle2}
`;

const TitleLabelWrap = styled.div`
  display: flex;
  align-items: center;
  margin-left: 1.2rem;
  align-self: center;
`;

const TitleCollapseButtonWrap = styled.div`
  margin-left: auto;
  margin-right: 1rem;
`;
