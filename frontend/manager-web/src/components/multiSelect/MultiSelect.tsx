import { Fragment, useEffect, useRef, useState } from "react";
import styled from "styled-components";
import Input from "components/input/Input";

import Checkbox from "./Checkbox";
import Portal, { PORTAL_ID } from "../portal/Portal";

export interface IOption {
  label: string | number;
  value: string | number;
  parentId?: string | number;
  category?: string;
}

const OPTIONS_HEIGHT = 240;
// Todo: add proptypes extension
interface ISelectProps {
  isMulti?: boolean;
  isSearch?: boolean;
  className?: string;
  label?: string;
  options: IOption[];
  placeholder?: string;
  categories?: string[];
  value: IOption[];
  // eslint-disable-next-line @typescript-eslint/no-explicit-any
  error?: any;
  disabled?: boolean;
  displayStrategy?: "portal" | "absolute";
  onChange?: (selectedItems: IOption[]) => void;
}

const MultiSelect = ({
  value = [],
  label,
  error,
  options,
  isMulti,
  onChange,
  categories,
  placeholder,
  isSearch,
  disabled,
  className,
  displayStrategy = "absolute",
}: ISelectProps) => {
  const containerRef = useRef(null);
  const searchInputRef = useRef(null);
  const [isOpen, setIsOpen] = useState(false);
  const [filteredOptions, setFilteredOptions] = useState([]);
  const [expandedOptionId, setExpandedOptionId] = useState(null);

  const selectedOptions = value && (value[0]?.value || value[0]) ? value : [];

  const handleScroll = () => {
    setIsOpen(false);
  };

  useEffect(() => {
    const handleClick = function (e) {
      if (!containerRef.current?.contains(e.target)) {
        setIsOpen(false);
      }
    };

    document.addEventListener("click", handleClick);
    document.addEventListener("scroll", handleScroll);

    return () => {
      document.removeEventListener("click", handleClick);
      document.removeEventListener("scroll", handleScroll);
    };
  }, []);
  const toggle = () => {
    if (!disabled) {
      setIsOpen(!isOpen);
    }
  };

  const toggleOption = option => {
    const isSelected = selectedOptions.find(item => item.value === option.value);

    if (isSelected) {
      if (isMulti) {
        onChange(selectedOptions.filter(item => item.value !== option.value));
        !isMulti && setIsOpen(false);
        return;
      }
    }

    if (isMulti) {
      onChange([...selectedOptions, option]);
      isSearch && searchInputRef.current?.focus();
    } else {
      onChange([option]);
      setIsOpen(false);
    }
  };

  useEffect(() => {
    if (isOpen) {
      searchInputRef.current?.focus();
    } else {
      options.length && setFilteredOptions(options);
    }
  }, [isOpen, options]);

  useEffect(() => {
    if (!isOpen) {
      setExpandedOptionId(null);
    }
  }, [isOpen]);

  const handleSearch = () => {
    setFilteredOptions(
      options.filter(option =>
        String(option.label).toLowerCase().includes(searchInputRef.current.value.toLowerCase())
      )
    );
  };

  useEffect(() => {
    if (!isSearch) {
      return;
    }
    const handleEscapeKey = e => {
      if (e.key === "Escape" && options.length && searchInputRef.current) {
        searchInputRef.current.value = "";
        setFilteredOptions(options);
      }
    };

    document.body.addEventListener("keydown", handleEscapeKey);
    return () => {
      document.body.removeEventListener("keydown", handleEscapeKey);
    };
    // eslint-disable-next-line react-hooks/exhaustive-deps
  }, [options]);

  const getDropdownPosition = () => {
    if (containerRef.current) {
      const { bottom, left, width } = containerRef.current.getBoundingClientRect();

      return { top: bottom, left, width };
    }
  };

  const toggleExpandSubOptions = value => {
    if (expandedOptionId === value) {
      setExpandedOptionId(null);
    } else {
      setExpandedOptionId(value);
    }
  };

  const cats = filteredOptions.reduce((acc, opt) => {
    if (acc && acc[opt.category]) {
      const isOptionExists = acc[opt.category].find(el => el.value === opt.value);
      if (!isOptionExists) {
        acc[opt.category].push(opt);
      }
      return acc;
    }
  }, categories);

  const isNoSearchResults =
    options.length && searchInputRef.current?.value.length && !filteredOptions.length;

  const renderOption = ({ value, label }) => {
    const hasSubOptions = options.find(item => item.parentId === value);
    const isSelected = Boolean(
      selectedOptions.find(item => item.value === value || item.parentId === value)
    );
    return (
      <Option
        isSelected={isSelected}
        key={value}
        onClick={e => {
          e.stopPropagation();
          hasSubOptions ? toggleExpandSubOptions(value) : toggleOption({ value, label });
        }}
      >
        {isMulti ? (
          <Checkbox isSelected={Boolean(selectedOptions.find(item => item.value === value))}>
            {label}
          </Checkbox>
        ) : (
          <OptionContent>
            {label}
            {hasSubOptions ? <ArrowRight className="icon-arrow-down"></ArrowRight> : null}
          </OptionContent>
        )}
      </Option>
    );
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
        {isSearch && (
          <SearchInputContainer>
            <Input searchIcon ref={searchInputRef} onChange={handleSearch} />
          </SearchInputContainer>
        )}
        {!!isNoSearchResults && <NoSearchResults>No search results...</NoSearchResults>}
        {cats
          ? Object.keys(cats).map(cat => {
              return (
                <Fragment key={cat}>
                  <OptionGroup>{cat}</OptionGroup>
                  {cats[cat].map(renderOption)}
                </Fragment>
              );
            })
          : filteredOptions.filter(item => !item.parentId).map(renderOption)}
      </OptionsContainer>
    );
  };

  const renderSelectedOptionsValue = option => {
    if (option.parentId) {
      const parent = options.find(item => item.value === option.parentId);

      return `${parent.label} ${option.label}`;
    }

    return option.label;
  };

  const renderSelectedOption = () => {
    return (
      <>
        <SelectedOptionText>
          {selectedOptions.length ? (
            isMulti ? (
              `Select (${selectedOptions.length})`
            ) : (
              renderSelectedOptionsValue(selectedOptions[0])
            )
          ) : (
            <Placeholder>{placeholder || "Select..."}</Placeholder>
          )}
        </SelectedOptionText>
      </>
    );
  };

  const renderOptionsWrapper = () => {
    if (displayStrategy === "portal") {
      return (
        <Portal id={PORTAL_ID.DROPDOWN}>
          <div
            className="dropdown"
            style={{
              position: "absolute",
              ...getDropdownPosition(),
            }}
          >
            {renderOptions()}
          </div>
        </Portal>
      );
    }

    return renderOptions();
  };

  return (
    <Container ref={containerRef} className={className}>
      {label && <Label>{label}</Label>}
      <SelectedValue isOpen={isOpen} onClick={toggle} hasError={Boolean(error)}>
        {renderSelectedOption()}
        <ArrowExpand isOpen={isOpen} className="icon-arrow-down" />
      </SelectedValue>
      {isOpen && renderOptionsWrapper()}
      {isOpen && expandedOptionId && (
        <SubOptionsContainer>
          {options
            .filter(item => item.parentId === expandedOptionId)
            .map((item, index) => {
              const parent = options.find(({ value }) => value === item.parentId);
              const isSelected = Boolean(selectedOptions.find(({ value }) => value === item.value));
              return (
                <Option
                  isSelected={isSelected}
                  onClick={() => toggleOption(item)}
                  key={index}
                >{`${parent.value} ${item.label}`}</Option>
              );
            })}
        </SubOptionsContainer>
      )}
    </Container>
  );
};

export default MultiSelect;

export const SubOptionsContainer = styled.div`
  position: absolute;
  top: 103px;
  right: -200px;
  width: 200px;
  height: 180px;
  background: ${({ theme }) => theme.colors.bg.ternary};
  box-shadow: 0 2px 4px rgba(0, 0, 0, 0.16);
  border-radius: 0 0 4px 4px;
  z-index: 10;
`;

const ArrowRight = styled.i`
  display: inline-block;
  transform: rotate(270deg);
  font-size: 1rem;
`;

const OptionContent = styled.div`
  display: flex;
  justify-content: space-between;
`;

const Container = styled.div`
  position: relative;
  width: 100%;
`;

export const SelectedValue = styled.div<{ isOpen: boolean; hasError: boolean }>`
  box-sizing: border-box;
  position: relative;
  display: flex;
  align-items: center;
  width: 100%;
  height: 36px;
  padding: 0 25px 0 9px;
  background: ${({ theme }) => theme.colors.bg.ternary};
  ${({ theme }) => theme.typography.body1}
  border: 1px solid ${({ theme, isOpen, hasError }) =>
    isOpen
      ? theme.colors.border.active
      : hasError
      ? theme.colors.error
      : theme.colors.border.primary};
  box-shadow: ${({ isOpen, theme }) => isOpen && `0 0 3pt 2pt ${theme.colors.border.activeRadius}`};
  border-radius: 4px;
`;
const Label = styled.div`
  padding: 0 0 6px 10px;
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
  display: flex;
  flex-direction: column;
  max-height: ${OPTIONS_HEIGHT}px;
  overflow: auto;
  ${({ theme }) => theme.typography.body1}
  background: ${({ theme }) => theme.colors.bg.ternary};
  border: 1px solid ${({ theme }) => theme.colors.border.primary};
`;

const Placeholder = styled.span`
  opacity: 0.3;
`;

const SelectedOptionText = styled.span`
  overflow: hidden;
  text-overflow: ellipsis;
`;

const OptionGroup = styled.div`
  padding: 1rem;
  font-weight: 700;
`;

const Option = styled.div<{ isSelected?: boolean }>`
  padding: 1rem 2rem;
  cursor: pointer;
  background: ${({ theme, isSelected }) => (isSelected ? theme.colors.bg.active : "inherit")};

  &:hover {
    background: ${({ theme }) => theme.colors.bg.active};
  }
`;

const SearchInputContainer = styled.div`
  padding: 8px 8px 0;
`;

const NoSearchResults = styled.div`
  padding: 8px;
`;
