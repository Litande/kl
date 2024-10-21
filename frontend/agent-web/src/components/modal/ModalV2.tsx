import React, { ReactNode } from "react";
import styled from "styled-components";
import ModalPortal from "components/modalPortal/ModalPortal";

type Props = {
  title: string;
  className?: string;
  hasCloseIcon?: boolean;
  onCancel?: () => void;
  children?: ReactNode | string;
};

const Modal = ({ title, children, hasCloseIcon, onCancel, className }: Props) => {
  return (
    <StyledModal isOpen={true} handleClose={onCancel} className={className}>
      <ModalHeader>
        {title}
        {hasCloseIcon && <CloseModal className="icon-ic-close" onClick={onCancel} />}
      </ModalHeader>
      <ModalContent>{children}</ModalContent>
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
  color: #d9d9d9;
`;

export const ModalContent = styled.div`
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
