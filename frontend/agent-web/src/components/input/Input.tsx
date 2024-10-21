import React, { ReactElement, forwardRef } from "react";
import styled from "styled-components";

interface IInputProps extends React.InputHTMLAttributes<HTMLInputElement> {
  searchIcon?: boolean;
  inputIcon?: ReactElement;
  label?: string;
  className?: string;
  containerClassName?: string;
  inputRef?: React.Ref<HTMLInputElement> | null;
}

const Input = forwardRef(function Input(
  { label, searchIcon, className, inputRef, inputIcon, ...inputProps }: IInputProps,
  ref: React.Ref<HTMLInputElement> | null
) {
  return (
    <Container className={className} ref={ref}>
      {label && <Label>{label}</Label>}
      <InputContainer>
        <StyledInput inputIcon={inputIcon} searchIcon={searchIcon} {...inputProps} />
        {searchIcon && <SearchIcon className="icon-ic-search" />}
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
  searchIcon: boolean;
  inputIcon;
};

const StyledInput = styled.input.attrs((props: StyledInputProps) => ({
  searchIcon: props.searchIcon,
  inputIcon: props.inputIcon,
}))`
  box-sizing: border-box;
  position: relative;
  width: 100%;
  height: 36px;
  padding: 8px 9px 8px ${({ searchIcon, inputIcon }) => (searchIcon || inputIcon) && "35px"};
  ${({ theme }) => theme.typography.body3}
  border: 1px solid ${({ theme }) => theme.colors.border.primary};
  border-radius: 4px;

  ::placeholder {
    color: ${({ theme }) => theme.colors.fg.secondary_disabled};
  }
`;
