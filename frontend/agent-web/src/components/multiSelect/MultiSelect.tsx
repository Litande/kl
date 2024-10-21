import { useState, useEffect, useRef } from "react";
import Checkbox from "./Checkbox";

import styled from "styled-components";

export interface IOption {
  label: string | number;
  value: string | number;
}
// Todo: add proptypes extension
interface ISelectProps {
  isMulti?: boolean;
  className?: string;
  label?: string;
  options: IOption[];
  placeholder?: string;
  value: IOption[];
  onChange?: (selectedItems: IOption[]) => void;
}

const MultiSelect = (props: ISelectProps) => {
  const containerRef = useRef(null);
  const [isOpen, setIsOpen] = useState(false);
  const [optionMaxWidth, setOptionMaxWidth] = useState("auto");

  const {
    value: selectedOptions,
    label,
    options,
    isMulti,
    onChange,
    placeholder,
    className,
  } = props;

  useEffect(() => {
    const handleClick = function (e) {
      if (!containerRef.current?.contains(e.target)) {
        setIsOpen(false);
      }
    };

    document.addEventListener("click", handleClick);

    return () => {
      document.removeEventListener("click", handleClick);
    };
  }, []);

  useEffect(() => {
    if (containerRef.current) {
      setOptionMaxWidth(`${containerRef.current.offsetWidth}px`);
    }
  }, [containerRef]);

  const toggle = () => setIsOpen(!isOpen);

  const toggleOption = option => {
    const isSelected = selectedOptions.find(item => item.value === option.value);

    if (isSelected) {
      onChange(selectedOptions.filter(item => item.value !== option.value));
      !isMulti && setIsOpen(false);
      return;
    }

    if (isMulti) {
      onChange([...selectedOptions, option]);
    } else {
      onChange([option]);
      setIsOpen(false);
    }
  };

  const renderOptions = () => {
    if (!options.length) {
      return (
        <OptionsContainer>
          <Option>No results...</Option>
        </OptionsContainer>
      );
    }
    return (
      <OptionsContainer>
        {props.options.map(({ value, label }) => (
          <Option
            isSelected={Boolean(props.value.find(item => item.value === value))}
            key={value}
            onClick={e => {
              e.stopPropagation();
              toggleOption({ value, label });
            }}
          >
            {isMulti ? (
              <Checkbox isSelected={Boolean(props.value.find(item => item.value === value))}>
                {label}
              </Checkbox>
            ) : (
              label
            )}
          </Option>
        ))}
      </OptionsContainer>
    );
  };

  const renderSelectedOption = () => {
    return (
      <>
        <SelectedOptionText>
          {selectedOptions.length ? (
            isMulti ? (
              `Select (${selectedOptions.length})`
            ) : (
              selectedOptions[0].label
            )
          ) : (
            <Placeholder>{placeholder || "Select..."}</Placeholder>
          )}
        </SelectedOptionText>
      </>
    );
  };

  return (
    <Container ref={containerRef} className={className}>
      {label && <Label>{label}</Label>}
      <SelectedValue maxWidth={optionMaxWidth} onClick={toggle}>
        {renderSelectedOption()}
        <ArrowExpand isOpen={isOpen} className="icon-ic-arrowDown" />
      </SelectedValue>
      {isOpen && renderOptions()}
    </Container>
  );
};

export default MultiSelect;

const Container = styled.div`
  position: relative;
  width: 100%;
`;

const SelectedValue = styled.div<{ maxWidth: string }>`
  box-sizing: border-box;
  position: relative;
  display: flex;
  align-items: center;
  width: 100%;
  max-width: ${({ maxWidth }) => maxWidth};
  height: 36px;
  padding: 0 25px 0 9px;
  background: ${({ theme }) => theme.colors.bg.ternary};
  ${({ theme }) => theme.typography.body1}
  border: 1px solid ${({ theme }) => theme.colors.border.primary};
  border-radius: 4px;
`;
const Label = styled.div`
  padding: 0 0 3px 10px;
  ${({ theme }) => theme.typography.smallText1}
  color: ${({ theme }) => theme.colors.fg.secondary_light};
  text-transform: uppercase;
`;

const ArrowExpand = styled.i<{ isOpen: boolean }>`
  position: absolute;
  top: 8px;
  right: 5px;
  font-size: 22px;
  color: ${({ theme }) => theme.colors.fg.secondary_light};

  ${({ isOpen }) => (isOpen ? "transform: rotate(180deg);" : "")}
  ${({ isOpen }) => (isOpen ? "top: 6px;" : "")}
`;

const OptionsContainer = styled.div`
  position: absolute;
  right: 0;
  left: 0;
  z-index: 10;
  max-height: 240px;
  overflow: auto;
  ${({ theme }) => theme.typography.body1}
  background: ${({ theme }) => theme.colors.bg.ternary};
  border: 1px solid ${({ theme }) => theme.colors.border.primary};
`;

const Placeholder = styled.span`
  opacity: 0.3;
`;

const SelectedOptionText = styled.div`
  overflow: hidden;
  text-overflow: ellipsis;
`;

const Option = styled.div<{ isSelected?: boolean }>`
  padding: 14px 8px;
  cursor: pointer;
  overflow: hidden;
  text-overflow: ellipsis;
  background: ${({ theme, isSelected }) => (isSelected ? theme.colors.bg.active : "inherit")};

  &:hover {
    background: ${({ theme }) => theme.colors.bg.active};
  }
`;
