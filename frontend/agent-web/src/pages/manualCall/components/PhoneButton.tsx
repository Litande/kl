import { FC } from "react";
import styled from "styled-components";
import { typography } from "globalStyles/theme/fonts";

type Props = {
  value: string;
  onClick: (symbol) => void;
};

const PhoneButton: FC<Props> = ({ onClick, value }) => {
  return <PhoneButtonContainer onClick={onClick}>{value}</PhoneButtonContainer>;
};

export default PhoneButton;

export const PhoneButtonContainer = styled.div`
  display: flex;
  align-items: center;
  justify-content: center;
  width: 60px;
  height: 60px;
  border: 1px solid ${({ theme }) => theme.colors.btn.secondary};
  border-radius: 4px;
  ${typography.subtitle2}
  font-family: "Inter regular";
  cursor: pointer;

  &:hover {
    background: ${({ theme }) => theme.colors.bg.active};
  }
`;
