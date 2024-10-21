import React from "react";
import styled from "styled-components";
import { IActive } from "pages/dashboard/styles";

interface ISwitchComponentProps {
  isActive: boolean;
  onClick: () => void;
}

const SwitchComponent = ({ isActive, onClick }: ISwitchComponentProps) => {
  return (
    <SwitchContainer isActive={isActive} onClick={onClick}>
      <Switch isActive={isActive} />
    </SwitchContainer>
  );
};

export default SwitchComponent;

const SwitchContainer = styled.div<IActive>`
  width: 45px;
  height: 24px;
  border-radius: 18px;
  transition: background 0.2s ease-in-out;
  background: ${({ isActive, theme }) =>
    isActive ? theme.colors.btn.secondary : theme.colors.btn.secondary_off};
  cursor: pointer;
  position: relative;
  display: flex;
  align-items: center;
`;

const Switch = styled.div<IActive>`
  width: 16px;
  height: 16px;
  border-radius: 50%;
  background: ${({ theme }) => theme.colors.btn.primary_pressed};
  position: absolute;
  left: ${({ isActive }) => (isActive ? "24px" : "4px")};
  transition: left 0.2s ease-in-out;
`;
