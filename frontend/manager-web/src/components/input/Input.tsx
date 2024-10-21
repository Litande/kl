import React, { ReactElement, forwardRef, ForwardedRef } from "react";
import styled from "styled-components";

interface IInputProps extends React.InputHTMLAttributes<HTMLInputElement> {
  searchIcon?: boolean;
  inputIcon?: ReactElement;
  label?: string;
  labelComponent?: ReactElement;
  className?: string;
  containerClassName?: string;
  // eslint-disable-next-line @typescript-eslint/no-explicit-any
  error?: any;
}

const Input = forwardRef(function Input(
  { label, searchIcon, className, labelComponent, inputIcon, error, ...inputProps }: IInputProps,
  ref: ForwardedRef<HTMLInputElement> | null
) {
  const handleClick = e => {
    if (inputProps.type === "time") {
      e.preventDefault();
    }
  };

  return (
    <Container className={className}>
      {label && <Label>{label}</Label>}
      {labelComponent && labelComponent}
      <InputContainer>
        <StyledInput
          ref={ref}
          hasIcon={Boolean(inputIcon || searchIcon)}
          hasError={Boolean(error)}
          onClick={handleClick}
          {...inputProps}
        />
        {searchIcon && <SearchIcon className="icon-search-large" />}
        {inputIcon}
      </InputContainer>
    </Container>
  );
});

export default Input;

const Container = styled.div`
  display: flex;
  flex-direction: column;
`;

const SearchIcon = styled.i`
  position: absolute;
  left: 8px;
  top: calc(50% - 7px);
  opacity: 0.6;
  font-size: 14px;
  z-index: 1;
`;

const Label = styled.label`
  padding: 0 0 6px 10px;
  ${({ theme }) => theme.typography.smallText1}
  color: ${({ theme }) => theme.colors.fg.secondary_disabled};
  text-transform: uppercase;
`;

const InputContainer = styled.span`
  position: relative;
`;

type StyledInputProps = {
  hasIcon: boolean;
  hasError: boolean;
};

export const StyledInput = styled.input<StyledInputProps>`
  box-sizing: border-box;
  position: relative;
  width: 100%;
  height: 36px;
  padding: 8px 9px 8px ${({ hasIcon }) => hasIcon && "35px"};
  ${({ theme }) => theme.typography.body3}
  border: 1px solid ${({ theme, hasError }) =>
    hasError ? theme.colors.error : theme.colors.border.primary};
  border-radius: 4px;
  background: ${({ theme }) => theme.colors.bg.ternary};

  &:focus-visible {
    border: 1px solid ${({ theme }) => theme.colors.border.active};
    box-shadow: 0 0 3pt 2pt ${({ theme }) => theme.colors.border.activeRadius};
    outline: none;
  }

  ::placeholder {
    color: ${({ theme }) => theme.colors.fg.secondary_disabled};
  }

  ::-webkit-outer-spin-button,
  ::-webkit-inner-spin-button {
    -webkit-appearance: none;
    margin: 0;
  }

  &[type="number"] {
    -moz-appearance: textfield;
  }

  &[type="time"]::-webkit-calendar-picker-indicator {
    background: none;
  }
  &[disabled] {
    background: ${({ theme }) => theme.colors.bg.secondary};
  }
`;
