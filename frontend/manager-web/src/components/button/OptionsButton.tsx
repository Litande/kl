import { ButtonHTMLAttributes, ReactElement } from "react";
import styled from "styled-components";

enum ICON_NAME {
  edit = "icon-edit",
  expand = "icon-arrow-up",
  collaps = "icon-arrow-down",
  close = "icon-close",
  refresh = "icon-refresh",
}

interface IOptionsButton extends ButtonHTMLAttributes<HTMLButtonElement> {
  paintTO: string;
  iconType: keyof typeof ICON_NAME;
  hasOutline?: boolean;
  content?: ReactElement;
}

function OptionsButton({
  paintTO,
  iconType,
  content = null,
  hasOutline = true,
  ...props
}: IOptionsButton) {
  return (
    <Button paintTO={paintTO} iconType={iconType} {...props}>
      <i className={ICON_NAME[iconType]} /> {content}
    </Button>
  );
}

export default OptionsButton;

const Button = styled.button<Pick<IOptionsButton, "paintTO" | "iconType">>`
  min-width: 36px;
  min-height: 36px;
  width: max-content;
  background-color: #fff;
  color: ${({ paintTO }) => paintTO};
  border-color: ${({ paintTO }) => paintTO};
  font-size: 1.5rem;
  line-height: 2.3rem;
  border-width: 1px;
  border-style: solid;
  border-radius: 4px;
`;
