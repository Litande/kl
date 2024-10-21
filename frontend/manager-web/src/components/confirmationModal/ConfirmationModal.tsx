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
  onCancel: () => void;
  hasCloseIcon?: boolean;
  children: ReactNode;
};

const ConfirmationModal = ({
  onConfirm,
  onCancel,
  cancelButtonText = "Cancel",
  confirmButtonText = "Yes",
  title,
  hasCloseIcon,
  className,
  children,
}: Props) => {
  return (
    <StyledModal isOpen={true} handleClose={onCancel} className={className}>
      <ModalHeader>
        {title}
        {hasCloseIcon && <CloseModal className="icon-close" onClick={onCancel} />}
      </ModalHeader>
      <ModalContent>
        <TextWrap>{children}</TextWrap>
        <ButtonWrap>
          <StyledButton variant="secondary" onClick={onCancel}>
            {cancelButtonText}
          </StyledButton>
          <StyledButton onClick={onConfirm}>{confirmButtonText}</StyledButton>
        </ButtonWrap>
      </ModalContent>
    </StyledModal>
  );
};

export default ConfirmationModal;

const StyledModal = styled(ModalPortal)`
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
  font-size: 1.5rem;
  color: ${({ theme }) => theme.colors.modal.headerTextColor};
  cursor: pointer;
`;

const ModalContent = styled.div`
  display: flex;
  justify-content: center;
  align-items: center;
  flex-direction: column;
  padding: 2rem 1rem 1rem;
`;
const TextWrap = styled.div`
  ${({ theme }) => theme.typography.body3}
`;
const ButtonWrap = styled.div`
  display: flex;
  margin: 30px 0 0;
  gap: 16px;
`;
const StyledButton = styled(Button)`
  min-width: 120px;
`;
