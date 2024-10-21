import React, { ReactNode } from "react";
import styled from "styled-components";

interface IProps {
  color?: string;
  className?: string;
  children: ReactNode;
}

const Total: React.FC<IProps> = ({ children, className, color }) => {
  return (
    <Container className={className} color={color}>
      {children}
    </Container>
  );
};

export default Total;

const Container = styled.div`
  box-sizing: border-box;
  padding: 4px 7px;
  height: 20px;
  min-width: 35px;
  display: flex;
  align-items: center;
  justify-content: center;
  background: ${({ theme, color }) => color || theme.colors.btn.secondary};
  border-radius: 4px;
  color: ${({ theme }) => theme.colors.btn.primary};
  ${({ theme }) => theme.typography.smallText3}
`;
