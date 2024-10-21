import React from "react";
import styled from "styled-components";
import headerLogo from "images/header_logo.png";

const HeaderSkeleton = () => {
  return (
    <Container>
      <LeftBlock>
        <Logo src={headerLogo} alt="aorta" />
        <EnvContainer>
          <Side>Agent</Side>
          <Env env={process.env.REACT_APP_ENV}>{process.env.REACT_APP_ENV}</Env>
        </EnvContainer>
      </LeftBlock>
      <RightBlock>
        <UserMenu>
          <UserIcon className="icon-user" />
          <UserName />
          <ArrowDown isExpanded={true} className="icon-arrow-down" />
        </UserMenu>
      </RightBlock>
    </Container>
  );
};

export default HeaderSkeleton;

const Logo = styled.img`
  height: 100%;
`;

const EnvContainer = styled.div`
  height: 100%;
  position: relative;
`;

const Side = styled.div`
  color: white;
  font-family: "Inter medium", serif;
  font-size: 12px;
  line-height: 24px;
  padding: 1px 10px 0;
  box-sizing: border-box;
`;

const Env = styled.div<{ env?: string }>`
  position: absolute;
  right: -10px;
  top: 0;
  background-color: ${props =>
    props.env === "dev" ? "green" : props.env === "qa" ? "yellow" : "red"};
  line-height: 24px;
  padding: 1px 10px 0;
  border-radius: 9px;
  text-transform: uppercase;
  font-family: "Inter medium", serif;
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
  background: ${({ theme }) => theme.colors.bg.primary};
  ${({ theme }) => theme.typography.body1};
  color: ${({ theme }) => theme.colors.bg.ternary};
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
