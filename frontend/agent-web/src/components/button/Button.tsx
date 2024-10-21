import React, { ReactNode } from "react";
import styled from "styled-components";

type ButtonVariant = "primary" | "secondary" | undefined;

interface IButtonProps extends React.ButtonHTMLAttributes<HTMLButtonElement> {
  icon?: ReactNode;
  className?: string;
  children: ReactNode;
  variant?: ButtonVariant;
}

const Button = ({ children, icon, variant, className, ...buttonProps }: IButtonProps) => {
  return (
    <StyledButton className={className} variant={variant} {...buttonProps}>
      {icon ? icon : null}
      {children}
    </StyledButton>
  );
};

export default Button;

type StyledButtonProps = Omit<IButtonProps, "icon" | "children" | "className">;

const StyledButton = styled.button<StyledButtonProps>`
  box-sizing: border-box;
  display: flex;
  flex-direction: row;
  justify-content: center;
  align-items: center;
  flex-wrap: nowrap;
  height: 36px;
  white-space: nowrap;
  padding: 8px 16px;
  border: none;
  cursor: ${({ disabled }) => (disabled ? "inherit" : "pointer")};
  opacity: ${({ disabled }) => (disabled ? 0.3 : 1)};
  ${({ theme }) => theme.typography.buttonsText};

  ${({ variant, theme }) => {
    if (variant === "secondary") {
      return `
        background: ${theme.colors.btn.primary};
        color: ${theme.colors.btn.secondary};
        border: 1px solid ${theme.colors.btn.secondary};
      `;
    }
    return `
        background: ${theme.colors.btn.secondary};
        color: ${theme.colors.btn.primary};
      `;
  }});

  > i {
    font-size: 18px;
    margin: 0 8px 0 0;
  }
`;
