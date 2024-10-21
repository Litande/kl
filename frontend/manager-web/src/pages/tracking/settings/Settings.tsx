import styled, { css } from "styled-components";
import Button from "components/button/Button";
import React, { useEffect, useState } from "react";
import trackingApi from "services/api/tracking";
import InfoCard from "components/infoCard/InfoCard";
import { attributes, MODAL_TABS, TEAMS_TAB } from "./constants";
import ModalV2 from "components/modal/ModalV2";

interface ITeam {
  name: string;
  teamId: number;
}

type Props = {
  handleClose: () => void;
  handleSave: ({ teams }: { teams: ITeam[] }) => void;
  selectedTeams: ITeam[];
};

const Settings = ({ handleClose, handleSave, selectedTeams }: Props) => {
  const [activeTab, setActiveTab] = useState(TEAMS_TAB.name);
  const [teams, setTeams] = useState([]);

  useEffect(() => {
    trackingApi.getTeams().then(({ data: teams }) => {
      setTeams(
        teams.items.map(team => ({
          ...team,
          name: team.name,
          isSelected: selectedTeams.find(({ name }) => name === team.name),
          attributes,
        }))
      );
    });
    // eslint-disable-next-line react-hooks/exhaustive-deps
  }, []);

  const handleSelect = (teamId, isSelected) => {
    const teamsMap = teams.map(team => (team.teamId === teamId ? { ...team, isSelected } : team));

    setTeams(teamsMap);
  };
  const handleSaveClick = () => {
    handleSave({ teams: teams.filter(team => team.isSelected) });
  };

  const renderTeams = () => (
    <TeamsContainer>
      {teams.map(team => (
        <InfoCardContainer
          data={team}
          key={team.name}
          color="#1E1954"
          selectable
          isSelected={team.isSelected}
          onSelect={handleSelect}
        />
      ))}
    </TeamsContainer>
  );

  return (
    <StyledModal title={`Parameters`} onCancel={handleClose} hasCloseIcon>
      <ModalContent>
        <TabsContainer>
          {MODAL_TABS.map(({ name, label }) => (
            <Tab isActive={activeTab === name} key={name} onClick={() => setActiveTab(name)}>
              {label}
            </Tab>
          ))}
        </TabsContainer>
        <Content>{activeTab === TEAMS_TAB.name && renderTeams()}</Content>
      </ModalContent>
      <ModalFooter>
        <StyledButton variant="secondary" onClick={handleClose}>
          Cancel
        </StyledButton>
        <StyledButton onClick={handleSaveClick}>Apply</StyledButton>
      </ModalFooter>
    </StyledModal>
  );
};

export default Settings;

const StyledModal = styled(ModalV2)`
  box-sizing: border-box;
  width: 70vw;
  min-width: 620px;
  max-width: 950px;
  background: ${({ theme }) => theme.colors.bg.ternary};
  border-radius: 6px;
`;

const ModalContent = styled.div`
  height: 470px;
  width: 100%;
  overflow: auto;
  padding: 2px 16px 16px;
  border-bottom: 1px solid ${({ theme }) => theme.colors.border.primary};
`;
const ModalFooter = styled.div`
  display: flex;
  align-items: center;
  justify-content: center;
  padding: 16px 0 0;
  gap: 16px;
`;

const TabsContainer = styled.div`
  display: flex;
  justify-content: center;
  margin: -16px 0 0;
`;

const ActiveTab = css`
  color: ${({ theme }) => theme.colors.fg.link};

  &:after {
    content: "";
    position: absolute;
    right: 0;
    bottom: 0;
    left: 0;
    height: 4px;
    border-radius: 4px 4px 0 0;
    background: ${({ theme }) => theme.colors.fg.link};
  }
`;

const Tab = styled.div<{ isActive: boolean }>`
  position: relative;
  min-width: 110px;
  padding: 14px 0;
  text-align: center;
  cursor: pointer;
  color: ${({ theme }) => theme.colors.fg.secondary_light};
  ${({ theme }) => theme.typography.subtitle4};

  ${({ isActive }) => (isActive ? ActiveTab : null)};

  &:hover {
    ${ActiveTab};
  }
`;

const Content = styled.div`
  padding: 16px 0;
`;

const TeamsContainer = styled.div`
  display: flex;
  flex-wrap: wrap;
  gap: 16px;
`;

const StyledButton = styled(Button)`
  min-width: 120px;
`;

const InfoCardContainer = styled(InfoCard)`
  width: calc(50% - 8px);

  @media (min-width: 1200px) {
    width: calc(33% - 8px);
  }
`;
