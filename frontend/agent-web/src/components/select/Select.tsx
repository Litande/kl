import React from "react";
import ReactSelect from "react-select";
import styled from "styled-components";

export interface IOption {
  value: string | number;
  label: string;
}
// Todo: add proptypes extension
interface ISelectProps {
  className?: string;
  containerClassName?: string;
  label?: string;
  options: Array<IOption>;
}

const Select = (props: ISelectProps) => {
  const { className, containerClassName, label, ...selectProps } = props;

  return (
    <div className={containerClassName}>
      {props.label && <Label>{props.label}</Label>}
      <StyledReactSelect {...selectProps} classNamePrefix={`${className} select`} />
    </div>
  );
};

export default Select;

const Label = styled.div`
  padding: 0 0 3px 10px;
  ${({ theme }) => theme.typography.smallText1}
  color: rgba(0, 0, 0, 0.5);
  text-transform: uppercase;
`;

const StyledReactSelect = styled(ReactSelect)`
  ${({ theme }) => theme.typography.body1}
  .select__control {
    min-height: 36px;
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
`;
