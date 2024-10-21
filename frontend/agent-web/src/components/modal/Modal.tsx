import React, { ReactNode } from "react";
import styled from "styled-components";
import ModalPortal from "components/modalPortal/ModalPortal";
import Button from "components/button/Button";

type Props = {
  title: string;
  cancelButtonText?: string;
  confirmButtonText?: string;
  className?: string;
  onConfirm: () => void;
  onCancel?: () => void;
  hasCloseIcon?: boolean;
  hasCancel?: boolean;
  children?: ReactNode | string;
};

const Modal = ({
  onConfirm,
  onCancel,
  hasCancel = true,
  cancelButtonText = "Cancel",
  confirmButtonText = "Yes",
  title,
  children,
  hasCloseIcon,
  className,
}: Props) => {
  return (
    <StyledModal isOpen={true} handleClose={onCancel} className={className}>
      <ModalHeader>
        {title}
        {hasCloseIcon && <CloseModal className="icon-ic-close" onClick={onCancel} />}
      </ModalHeader>
      <ModalContent>
        {children}
        <ButtonWrap>
          {hasCancel && (
            <StyledButton variant="secondary" onClick={onCancel}>
              {cancelButtonText}
            </StyledButton>
          )}
          <StyledButton onClick={onConfirm}>{confirmButtonText}</StyledButton>
        </ButtonWrap>
      </ModalContent>
    </StyledModal>
  );
};

export default Modal;

const StyledModal = styled(ModalPortal)`
  box-sizing: border-box;
  width: 580px;
  background: ${({ theme }) => theme.colors.bg.ternary};
  border-radius: 6px;
  opacity: 1;
`;

const ModalHeader = styled.div`
  display: flex;
  align-items: center;
  justify-content: space-between;
  padding: 16px 20px 16px 25px;
  background: ${({ theme }) => theme.colors.bg.primary};
  border-radius: 6px 6px 0 0;
  text-transform: uppercase;
  ${({ theme }) => theme.typography.subtitle3};
  color: ${({ theme }) => theme.colors.btn.primary};
`;

const CloseModal = styled.i`
  cursor: pointer;
`;

const ModalContent = styled.div`
  box-sizing: border-box;
  display: flex;
  justify-content: center;
  align-items: center;
  flex-direction: column;
  min-height: calc(100% - 66px);
  padding: 16px;
`;

export const ButtonWrap = styled.div`
  display: flex;
  padding: 30px 0 0;
  gap: 16px;
`;
const StyledButton = styled(Button)`
  min-width: 120px;
`;
