import styled from "styled-components";

export interface IActive {
  isActive: boolean;
}
export const PeriodItem = styled.div<IActive>`
  padding: 5px 26px;
  background: ${({ isActive, theme }) =>
    isActive ? theme.colors.btn.secondary : theme.colors.btn.secondary_non_active};
  ${({ theme }) => theme.typography.smallText2};
  border-radius: 18px;
  cursor: pointer;
  color: ${({ isActive, theme }) =>
    isActive ? theme.colors.fg.secondary_active : theme.colors.fg.secondary};
`;
