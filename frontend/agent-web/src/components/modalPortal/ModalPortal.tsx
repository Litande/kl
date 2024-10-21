import React, { FC, useEffect, useRef } from "react";
import styled from "styled-components";
import Portal, { PORTAL_ID } from "components/portal/Portal";

type Props = {
  children?: React.ReactNode;
  isOpen: boolean;
  className?: string;
  handleClose?: () => void;
};

const ModalPortal: FC<Props> = ({ className, children, isOpen, handleClose }) => {
  const nodeRef = useRef(null);
  useEffect(() => {
    const closeOnEscapeKey = e => (e.key === "Escape" ? handleClose && handleClose() : null);

    document.body.addEventListener("keydown", closeOnEscapeKey);
    return () => {
      document.body.removeEventListener("keydown", closeOnEscapeKey);
    };
  }, [handleClose]);

  return (
    <Portal id={PORTAL_ID.MODAL}>
      <Container ref={nodeRef} isOpen={isOpen}>
        <Content className={className}>{children}</Content>
      </Container>
    </Portal>
  );
};

export default ModalPortal;

type ModalProps = {
  isOpen: boolean;
};

const Container = styled.div<ModalProps>`
  position: fixed;
  inset: 0; /* inset sets all 4 values (top right bottom left) much like how we set padding, margin etc., */
  flex-direction: column;
  align-items: center;
  justify-content: center;
  transition: all 0.3s ease-in-out;
  overflow: hidden;
  z-index: 999;
  padding: 40px 20px 20px;
  opacity: ${({ isOpen }) => (isOpen ? 1 : 0)};
  display: ${({ isOpen }) => (isOpen ? "flex" : "none")};
  background: rgba(0, 0, 0, 0.12);
`;

const Content = styled.div``;
