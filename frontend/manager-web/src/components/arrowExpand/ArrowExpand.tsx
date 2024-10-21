import { FC } from "react";
import styled from "styled-components";
import useToggle from "hooks/useToggle";

type Props = {
  defaultValue?: boolean;
  onClick: () => void;
  isVisible?: boolean;
};

const ArrowExpand: FC<Props> = ({ onClick, defaultValue = false, isVisible = true }) => {
  const [isOpen, toggleOpen] = useToggle(defaultValue);
  const handleClick = () => {
    toggleOpen();
    onClick();
  };

  return (
    <Wrap isVisible={isVisible} onClick={handleClick} isOpen={isOpen} className="icon-arrow-down" />
  );
};

export default ArrowExpand;

const Wrap = styled.i<{ isOpen: boolean; isVisible: boolean }>`
  border: 1px solid ${({ theme }) => theme.colors.border.secondary};
  border-radius: 4px;
  padding: 5px;
  font-size: 24px;
  cursor: pointer;
  color: ${({ theme }) => theme.colors.btn.secondary};

  ${({ isOpen }) => (isOpen ? "transform: rotate(180deg);" : "")}
  ${({ isOpen }) => (isOpen ? "top: 6px;" : "")}
  ${({ isVisible }) => (isVisible ? "" : "display: none;")}
`;
