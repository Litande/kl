import React, { KeyboardEventHandler } from "react";

import CreatableSelect from "react-select/creatable";
import styled from "styled-components";

const components = {
  DropdownIndicator: null,
};

interface IOption {
  readonly label: string;
  readonly value: string;
}

const createOption = (label: string) => ({
  label,
  value: label,
});

type MultipleInputProps = {
  onChange: (value) => void;
  placeholder?: string;
  value: IOption[];
};

export default function MultipleInput({ onChange, value = [], ...props }: MultipleInputProps) {
  //https://react-select.com/styles#the-classnameprefix-prop
  const [inputValue, setInputValue] = React.useState("");
  const [values, setValues] = React.useState<readonly IOption[]>(value);

  const handleKeyDown: KeyboardEventHandler = event => {
    if (!inputValue) return;
    if (values.find(el => el.label === inputValue)) return;
    const newValue = [...values, createOption(inputValue)];
    switch (event.key) {
      case "Enter":
        setValues(newValue);
        onChange(newValue);
        setInputValue("");
        event.preventDefault();
    }
  };

  return (
    <StyledCreatableSelect
      {...props}
      components={components}
      inputValue={inputValue}
      isClearable
      isMulti
      classNamePrefix="select"
      menuIsOpen={false}
      onChange={(newValue: IOption[]) => setValues(newValue)}
      onInputChange={newValue => setInputValue(newValue)}
      onKeyDown={handleKeyDown}
      value={values}
    />
  );
}

const StyledCreatableSelect = styled(CreatableSelect)`
  ${({ theme }) => theme.typography.body1}

  .select__control {
    min-height: 36px;
    box-shadow: none;
    border: 1px solid ${({ theme }) => theme.colors.border.primary};

    :hover,
    :active {
      border: 1px solid ${({ theme }) => theme.colors.border.active};
    }
  }

  .select__control--menu-is-open {
    border: 1px solid ${({ theme }) => theme.colors.border.active};

    :hover,
    :active {
      border: 1px solid ${({ theme }) => theme.colors.border.active};
    }
  }

  .select__placeholder {
    color: ${({ theme }) => theme.colors.fg.secondary_light_disabled};
  }

  & .select__value-container {
    padding: 0 8px;
    max-height: 5rem;
    overflow-y: auto;
  }

  .select__input-container {
    margin: 0 2px;
    padding: 0;
  }

  .select__indicator {
    padding: 7px;

    svg {
      fill: ${({ theme }) => theme.colors.fg.secondary_light};
    }
  }

  .select__indicator-separator {
    display: none;
  }

  .select__option--is-selected {
    background-color: ${({ theme }) => theme.colors.bg.active};
    color: ${({ theme }) => theme.colors.fg.secondary};

    :hover,
    :active {
      background-color: ${({ theme }) => theme.colors.bg.active};
    }
  }
  .select__multi-value {
    display: flex;
    height: 1.9rem;
    gap: 0.3rem;
    margin: 2px;
    align-items: center;
    padding: 0 8px;
    background: ${({ theme }) => theme.colors.bg.tag};
    border-radius: 4px;
    ${({ theme }) => theme.typography.body1};
  }

  .select__multi-value__remove {
    display: flex;
    justify-content: center;
    align-items: center;
    width: 1.5rem;
    height: 1.5rem;
    text-align: center;
    background: ${({ theme }) => theme.colors.bg.ternary};
    border-radius: 50%;
    color: ${({ theme }) => theme.colors.fg.secondary_light};
    cursor: pointer;

    & > svg {
      width: 1.5rem;
      height: 1.5rem;
    }
  }
  @media (min-width: 1600px) {
    width: calc(90vw * 0.4);
  }
`;
