import styled, { useTheme } from "styled-components";
import React from "react";
import Button from "components/button/Button";
import OptionsButton from "components/button/OptionsButton";
import { FormActions } from "./items/BaseStyles";
import { Control } from "react-hook-form";

type HeaderProps = {
  label: string | JSX.Element;
  isOpen: boolean;
  height: number;
};

export const HEADER_HEIGHT = 54;

const Header = ({ label, isOpen }: HeaderProps) => {
  const theme = useTheme();
  return (
    <TitleWrap>
      {label}
      <OptionsButton
        paintTO={theme.colors.btn.secondary}
        iconType={isOpen ? "expand" : "collaps"}
      />
    </TitleWrap>
  );
};

export const HeaderHelper = React.memo(Header);

export type FormControlProps = {
  // eslint-disable-next-line @typescript-eslint/no-explicit-any
  control: Control<any>;
  trigger?: (path) => Promise<boolean>;
};

export type ButtonSaveProps = {
  areActionsAvailable?: boolean;
  onCancel: () => void;
  onApply: (b?: boolean) => void;
};

export const Actions = ({ onCancel, onApply, areActionsAvailable }: ButtonSaveProps) => {
  return (
    <FormActions>
      <Button
        type="button"
        variant={"secondary"}
        onClick={onCancel}
        disabled={!areActionsAvailable}
      >
        Cancel
      </Button>
      <Button
        type="submit"
        variant={"primary"}
        onClick={() => onApply()}
        disabled={!areActionsAvailable}
      >
        Apply
      </Button>
    </FormActions>
  );
};

const TitleWrap = styled.div`
  box-sizing: border-box;
  display: flex;
  align-items: center;
  justify-content: space-between;
  width: 100%;
  height: ${HEADER_HEIGHT}px;
  padding: 0 1rem;
  cursor: pointer;
  text-transform: uppercase;
  ${({ theme }) => theme.typography.subtitle2}
`;
