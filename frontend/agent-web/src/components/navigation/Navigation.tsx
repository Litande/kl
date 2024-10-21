import React from "react";
import styled, { css } from "styled-components";
import { NavLink } from "react-router-dom";
import { ROUTES } from "router/enums";

import ConnectionWidget from "components/connection/ConnectionWidget";
import useToggle from "hooks/useToggle";

const Navigation = () => {
  const [isCollapsed, toggleMenu] = useToggle();

  return (
    <Container isCollapsed={isCollapsed}>
      <ExpandIcon className="icon-ic-collapse" onClick={toggleMenu} isCollapsed={isCollapsed} />
      <NavItem to={ROUTES.DASHBOARD}>
        <NavItemIcon className="icon-ic-dashboard" />
        <NavItemText>Board</NavItemText>
      </NavItem>
      <NavItem to={`/${ROUTES.DIALING}`}>
        <NavItemIcon className="icon-ic-dialing" />
        <NavItemText>Dialing</NavItemText>
      </NavItem>
      <NavItem to={`/${ROUTES.FUTURE_CALLBACK}`}>
        <NavItemIcon className="icon-ic-futureCallback" />
        <NavItemText>Future Call Back</NavItemText>
      </NavItem>
      <NavItem to={`/${ROUTES.MANUAL_CALL}`}>
        <NavItemIcon className="icon-ic-manualCall" />
        <NavItemText>Manual Call</NavItemText>
      </NavItem>
      <NavItem to={`/${ROUTES.HISTORY}`}>
        <NavItemIcon className="icon-ic-history" />
        <NavItemText>History</NavItemText>
      </NavItem>
      <NetworkWidgetWrap>
        <ConnectionWidget isFullInfo={!isCollapsed} />
      </NetworkWidgetWrap>
    </Container>
  );
};

export default Navigation;

const expandCSS = css`
  width: 90px;
  min-width: 90px;
  align-items: center;
`;

const activeCSS = css`
  background: ${({ theme }) => theme.colors.bg.active};
  border-radius: 4px;
`;

const NavItem = styled(NavLink)`
  display: flex;
  align-items: center;
  box-sizing: border-box;
  width: 100%;
  height: 55px;
  padding: 16px;
  ${({ theme }) => theme.typography.subtitle3}
  color: ${({ theme }) => theme.colors.fg.secondary};
  text-decoration: none;
  cursor: pointer;

  &:hover,
  &.active {
    ${activeCSS};
  }
`;

const NavItemText = styled.span`
  width: 150px;
  margin: 0 0 0 18px;
  white-space: nowrap;
  overflow: hidden;
  transition: width 0.5s, margin 0.6s;
`;

const NavItemIcon = styled.i`
  font-size: 24px;
  color: ${({ theme }) => theme.colors.icons.secondary};
`;

const ExpandIcon = styled.div<{ isCollapsed: boolean }>`
  display: flex;
  justify-content: center;
  align-items: center;
  align-self: flex-start;
  width: 56px;
  height: 56px;
  margin: 0 0 0.5rem;
  color: ${({ theme }) => theme.colors.border.active};
  font-size: 24px;
  cursor: pointer;
  transform: ${({ isCollapsed }) => (!isCollapsed ? "" : "rotate(180deg)")};

  &:hover {
    ${activeCSS};
  }

  ${({ isCollapsed }) => isCollapsed && activeCSS};
`;

const Container = styled.div<{ isCollapsed: boolean }>`
  box-sizing: border-box;
  display: flex;
  flex-flow: column nowrap;
  flex-direction: column;
  width: 230px;
  min-width: 230px;
  padding: 16px;
  background: ${({ theme }) => theme.colors.bg.ternary};
  border: 1px solid ${({ theme }) => theme.colors.border.primary};
  transition: 0.5s;
  ${({ isCollapsed }) => isCollapsed && expandCSS};

  ${NavItemText} {
    width: ${({ isCollapsed }) => isCollapsed && "0"};
    margin: ${({ isCollapsed }) => isCollapsed && "0"};
  }

  ${NavItem} {
    justify-content: ${({ isCollapsed }) => isCollapsed && "center"};
  }
`;

const NetworkWidgetWrap = styled.div`
  display: flex;
  align-items: center;
  box-sizing: border-box;
  width: 100%;
  padding-left: 16px;
  padding-right: 16px;
  margin-top: auto;
`;
