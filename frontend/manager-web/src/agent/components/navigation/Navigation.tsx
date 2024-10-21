import React from "react";
import styled from "styled-components";
import { NavLink } from "react-router-dom";

const Navigation = () => {
  return (
    <Container>
      <StyledNavLink to="/">
        <i className="icon-dashboard" />
        Dashboard
      </StyledNavLink>
    </Container>
  );
};

export default Navigation;

const Container = styled.div`
  box-sizing: border-box;
  display: flex;
  flex-direction: column;
  width: 230px;
  min-width: 230px;
  padding: 90px 17px 17px;
  background: ${({ theme }) => theme.colors.bg.ternary};
  border: 1px solid rgba(0, 0, 0, 0.12);
`;

const StyledNavLink = styled(NavLink)`
  display: flex;
  align-items: center;
  box-sizing: border-box;
  width: 100%;
  height: 55px;
  padding: 16px;
  ${({ theme }) => theme.typography.subtitle4}
  color: black;
  text-decoration: none;
  cursor: pointer;

  &:hover,
  &.active {
    background: ${({ theme }) => theme.colors.bg.active};
    border-radius: 4px;
  }

  i {
    font-size: 22px;
    margin-right: 18px;
    color: ${({ theme }) => theme.colors.icons.secondary};
  }
`;
