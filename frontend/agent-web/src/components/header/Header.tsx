import React, { useRef } from "react";
import styled from "styled-components";
import headerLogo from "images/header_logo.png";
import useToggle from "hooks/useToggle";
import { useNavigate } from "react-router-dom";
import { ROUTES } from "router/enums";
import { useOutsideAlerter } from "hooks/useOutsideAlterer";
import { useAgent } from "data/user/useAgent";
import Breaks from "./Breaks";

const Header = () => {
  const menuRef = useRef(null);
  const { agent, logout } = useAgent();
  const navigate = useNavigate();
  const [isExpanded, toggleExpanded] = useToggle(false);

  useOutsideAlerter(
    menuRef,
    () => {
      if (isExpanded) {
        toggleExpanded();
      }
    },
    [isExpanded]
  );

  const handleLogout = () => {
    logout();
    navigate(`/${ROUTES.AUTH}`);
  };

  const renderMenu = () => {
    return (
      <Menu>
        <MenuItem data-testid="menu-item-log-out" onClick={handleLogout}>
          Log out
        </MenuItem>
      </Menu>
    );
  };

  return (
    <Container>
      <LeftBlock>
        <Logo src={headerLogo} alt="aorta" />
        <EnvContainer>
          <Side>Agent</Side>
          {/*<Env env={process.env.REACT_APP_ENV}>{process.env.REACT_APP_ENV}</Env>*/}
        </EnvContainer>
      </LeftBlock>
      <RightBlock>
        <Breaks />
        <UserMenu onClick={toggleExpanded} ref={menuRef} data-testid="user-menu">
          <UserIcon className="icon-ic-user" />
          <UserName>{`${agent?.firstName} ${agent?.lastName}`}</UserName>
          <ArrowDown isExpanded={isExpanded} className="icon-ic-arrowDown" />
          {isExpanded && renderMenu()}
        </UserMenu>
      </RightBlock>
    </Container>
  );
};

export default Header;

const Logo = styled.img`
  height: 100%;
  max-height: 48px;
`;

const EnvContainer = styled.div`
  height: 100%;
  position: relative;
`;

const Side = styled.div`
  font-family: "Inter medium";
  font-size: 12px;
  line-height: 24px;
  padding: 1px 10px 0;
  box-sizing: border-box;
`;

const Env = styled.div<{ env: string }>`
  position: absolute;
  right: -10px;
  top: 0px;
  background-color: ${props =>
    props.env === "dev" ? "green" : props.env === "qa" ? "yellow" : "red"};
  line-height: 24px;
  padding: 1px 10px 0;
  border-radius: 9px;
  text-transform: uppercase;
  font-family: "Inter medium";
  font-size: 12px;
  border: 1px solid white;
  box-sizing: border-box;
  color: white;
  transform: translateX(100%);
`;

const Container = styled.div`
  display: flex;
  flex-direction: row;
  flex-wrap: nowrap;
  align-items: center;
  justify-content: space-between;
  box-sizing: border-box;
  height: 70px;
  padding: 9px 16px 9px 32px;
  ${({ theme }) => theme.typography.body1};
  background: ${({ theme }) => theme.colors.bg.primary};
  color: ${({ theme }) => theme.colors.fg.secondary};
`;

const LeftBlock = styled.div`
  display: flex;
  align-items: center;
`;

const RightBlock = styled.div`
  display: flex;
  align-items: center;
`;

const ArrowDown = styled.i<{ isExpanded: boolean }>`
  margin: 0 0 0 15px;
  font-size: 24px;
  transform: ${({ isExpanded }) => (isExpanded ? "rotate(180deg)" : "")};
`;

const UserIcon = styled.i`
  margin: 0 10px 0 0;
  font-size: 24px;
`;

const UserName = styled.div`
  flex: 1;
`;

const UserMenu = styled.div`
  position: relative;
  box-sizing: border-box;
  display: flex;
  min-width: 170px;
  align-items: center;
  cursor: pointer;
  padding: 6px;
  line-height: 24px;
  background: ${({ theme }) => theme.colors.btn.secondary};
  border-radius: 4px;
`;

const Menu = styled.div`
  position: absolute;
  right: 0;
  bottom: -100%;
  left: 0;
  display: flex;
  flex-direction: column;
  border-radius: 0 0 4px;
  color: initial;

  box-shadow: 0 0 4px rgba(0, 0, 0, 0.25);
`;
const MenuItem = styled.div`
  box-sizing: border-box;
  padding: 0 16px;
  line-height: 36px;
  background: ${({ theme }) => theme.colors.bg.ternary};

  &:hover {
    background: ${({ theme }) => theme.colors.bg.active};
  }
`;
