import { ReactNode, useEffect, useState } from "react";
import styled from "styled-components";
import CheckMark from "../multiSelect/CheckMark";

type Props = {
  onChange?: (b: boolean) => void;
  isSelected: boolean;
  children: ReactNode;
};

const CheckboxWithInnerState = ({ onChange, isSelected, children }: Props) => {
  const [isCheckboxSelected, setIsCheckboxSelected] = useState(isSelected);
  useEffect(() => {
    setIsCheckboxSelected(isSelected);
  }, [isSelected]);

  const handleClick = () => {
    setIsCheckboxSelected(!isCheckboxSelected);
    onChange(!isCheckboxSelected);
  };

  return (
    <CheckboxContainer onClick={handleClick}>
      <IconContainer isSelected={isCheckboxSelected}>
        <StyledCheckMark isSelected={isCheckboxSelected} />
      </IconContainer>
      <CheckboxText>{children}</CheckboxText>
    </CheckboxContainer>
  );
};

export default CheckboxWithInnerState;

const IconContainer = styled.div<{ isSelected: boolean }>`
  box-sizing: border-box;
  display: flex;
  align-items: center;
  justify-content: center;
  width: 20px;
  height: 20px;
  margin: 0 20px 0 2px;
  border: 1px solid
    ${({ theme, isSelected }) =>
      isSelected ? theme.colors.border.active : theme.colors.fg.secondary_light};
  border-radius: 4px;
`;

const StyledCheckMark = styled(CheckMark)<{ isSelected: boolean }>`
  display: ${({ isSelected }) => (isSelected ? "block" : "none")};
  font-size: 10px;
  color: ${({ theme }) => theme.colors.border.active};
`;

const CheckboxContainer = styled.div`
  display: flex;
  align-items: center;
  cursor: pointer;
`;

const CheckboxText = styled.div`
  overflow: hidden;
  text-overflow: ellipsis;
`;
