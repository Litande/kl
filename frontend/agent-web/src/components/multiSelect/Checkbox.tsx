import styled from "styled-components";
import CheckMark from "./CheckMark";
import { ReactNode } from "react";

type Props = {
  onChange?: (b: boolean) => void;
  isSelected: boolean;
  children: ReactNode;
};

const Checkbox = ({ onChange, isSelected, children }: Props) => {
  return (
    <CheckboxContainer onClick={() => onChange && onChange(!isSelected)}>
      <IconContainer isSelected={isSelected}>
        <StyledCheckMark isSelected={isSelected} />
      </IconContainer>
      <CheckboxText>{children}</CheckboxText>
    </CheckboxContainer>
  );
};

export default Checkbox;

const IconContainer = styled.div<{ isSelected: boolean }>`
  box-sizing: border-box;
  display: flex;
  align-items: center;
  justify-content: center;
  width: 18px;
  min-width: 18px;
  height: 20px;
  margin: 0 10px 0 2px;
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
