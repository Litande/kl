import styled, { css } from "styled-components";
import { NavLink } from "react-router-dom";
import { useDispatch, useSelector } from "react-redux";

import ConnectionWidget from "components/connection/ConnectionWidget";
import { navActions, navigationStateSelector } from "components/navigation/navigationSlice";

const Navigation = () => {
  const { isCollapsed } = useSelector(navigationStateSelector);
  const dispatch = useDispatch();

  const toggleMenu = () => {
    dispatch(navActions.toggleMenu());
  };

  return (
    <Container isCollapsed={isCollapsed}>
      <ExpandIcon className="icon-collapse" onClick={toggleMenu} isCollapsed={isCollapsed} />
      <NavItem to="/">
        <NavItemIcon className="icon-dashboard" />
        <NavItemText>Dashboard</NavItemText>
      </NavItem>
      <NavItem to="/tracking">
        <NavItemIcon className="icon-traking" />
        <NavItemText>Tracking</NavItemText>
      </NavItem>
      <NavItem to="/leads">
        <NavItemIcon className="icon-visualization" />
        <NavItemText>Leads Queues</NavItemText>
      </NavItem>
      <NavItem to="/rules">
        <NavItemIcon className="icon-rules" />
        <NavItemText>Rule Engine</NavItemText>
      </NavItem>
      <NavItem to="/agents">
        <NavItemIcon className="icon-agents" />
        <NavItemText>Agents</NavItemText>
      </NavItem>
      <NavItem to="/tags">
        <NavItemIcon className="icon-tags" />
        <NavItemText>Tags</NavItemText>
      </NavItem>
      <NavItem to="/recordings">
        <NavItemIcon className="icon-call-rec" />
        <NavItemText>Call Recordings</NavItemText>
      </NavItem>
      <NavItem to="/settings">
        <NavItemIcon className="icon-settings" />
        <NavItemText>Settings</NavItemText>
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
  padding: 1rem;
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
  font-weight: 500;

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

  body.full-screen & {
    display: none;
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
