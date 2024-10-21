import { ChangeEvent } from "react";
import ReactSelect from "react-select";
import styled from "styled-components";

export interface IOption {
  value: string | number;
  label: string;
}
// Todo: add proptypes extension
interface ISelectProps {
  className?: string;
  label?: string;
  options: Array<IOption>;
  placeholder?: string;
  onChange: (e: ChangeEvent<HTMLSelectElement>) => void;
}

const Select = (props: ISelectProps) => {
  const { className, label, ...selectProps } = props;

  return (
    <Container>
      {props.label && <Label>{props.label}</Label>}
      <StyledReactSelect
        {...selectProps}
        components={{
          DropdownIndicator: () => <ArrowExpand className="icon-arrow-down" />,
        }}
        classNamePrefix={`${className} select`}
      />
    </Container>
  );
};

export default Select;

const Container = styled.div``;

const Label = styled.div`
  padding: 0 0 3px 10px;
  ${({ theme }) => theme.typography.smallText1}
  color: ${({ theme }) => theme.colors.fg.secondary_light};
  text-transform: uppercase;
`;

const ArrowExpand = styled.i`
  font-size: 22px;
  margin: 0 5px 0 0;
  color: ${({ theme }) => theme.colors.fg.secondary_light};
`;

const StyledReactSelect = styled(ReactSelect)`
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

  .select__value-container {
    padding: 0 8px;
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
`;
