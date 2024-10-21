import { useState } from "react";
import styled from "styled-components";

const CheckIcon = () => (
  <Icon width="15" height="10" viewBox="0 0 12 9" fill="none" xmlns="http://www.w3.org/2000/svg">
    <path
      d="M1 3.8L4.75 8L11 1"
      stroke="#5142AE"
      strokeWidth="2"
      strokeLinecap="round"
      strokeLinejoin="round"
    />
  </Icon>
);

const Checkbox = ({
  checked = false,
  disabled = false,
  value,
  onChange,
  inputRef,
  ...rest
}: // eslint-disable-next-line @typescript-eslint/no-explicit-any
any) => {
  const [isChecked, setIsChecked] = useState(checked);

  const onChangeStatus = e => {
    onChange(e);
    setIsChecked(e.target.checked);
  };

  return (
    <CheckboxContainer checked={isChecked}>
      <HiddenCheckbox
        ref={inputRef}
        checked={isChecked}
        disabled={disabled}
        onChange={onChangeStatus}
        value={value}
        {...rest}
      />
      <StyledCheckbox disabled={disabled} checked={isChecked}>
        <CheckIcon />
      </StyledCheckbox>
    </CheckboxContainer>
  );
};

export default Checkbox;

const CheckboxContainer = styled.label<{ checked?: boolean }>`
  display: flex;
  justify-content: center;
  align-items: center;
  min-width: 75px;
  height: 100%;
  ${props => props.checked && "background: #f3f8ff"};
`;

const Icon = styled.svg`
  fill: none;
  stroke: white;
  stroke-width: 2px;
`;

const HiddenCheckbox = styled.input.attrs({ type: "checkbox" })`
  visibility: hidden;
  border: 0;
  height: 1px;
  margin: -1px;
  overflow: hidden;
  padding: 0;
  position: absolute;
  white-space: nowrap;
  width: 1px;
`;

const StyledCheckbox = styled.div<{ checked?: boolean; disabled?: boolean }>`
  display: inline-flex;
  justify-content: center;
  align-items: center;
  width: 20px;
  height: 20px;
  vertical-align: middle;
  border: 1px solid ${props => (props.checked ? "#5142AE" : "rgba(0, 0, 0, 0.6)")};
  background: #fff;
  border-radius: 3px;
  transition: all 150ms;
  opacity: ${props => (props.disabled ? 0.3 : 1)};
  ${Icon} {
    visibility: ${props => (props.checked ? "visible" : "hidden")};
  }
`;
