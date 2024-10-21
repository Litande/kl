import { colors } from "globalStyles/theme/palette";
import { css } from "styled-components";

export const borderStyle = {
  primary: `1px solid ${colors.border.primary}`,
  active: `1px solid ${colors.border.active}`,
};

export const defaultBorder = css`
  border-radius: 4px;
  border: ${borderStyle.primary};
`;

export const defaultShadow = css`
  border-radius: 8px;
  box-shadow: 0 2px 8px 0 rgba(0, 0, 0, 0.16);
`;
