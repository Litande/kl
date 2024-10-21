import styled from "styled-components";

import { NavLink, Outlet } from "react-router-dom";

import { rulesOutlets } from "router/Configuration";
import { useTranslation } from "react-i18next";

const Rules = () => {
  const { t } = useTranslation();
  return (
    <Container>
      <PageTitle>Rule engine</PageTitle>
      <RuleEngineOutletsWrapper>
        {rulesOutlets
          .filter(el => !el.exclude)
          .map(outlet => (
            <RuleNavLink key={outlet.path} to={outlet.path}>
              {t(`routeLabels.${outlet.path}`)}
            </RuleNavLink>
          ))}
      </RuleEngineOutletsWrapper>
      <Outlet />
    </Container>
  );
};

export default Rules;

const Container = styled.div`
  display: flex;
  flex-direction: column;
  height: 100%;
`;

const RuleEngineOutletsWrapper = styled.div`
  display: flex;
  flex-direction: row;
  min-height: 3rem;
  width: 100%;
  background-color: ${({ theme }) => theme.colors.bg.secondary};
`;

const RuleNavLink = styled(NavLink)`
  display: flex;
  align-items: center;
  justify-content: center;
  box-sizing: border-box;
  width: max-content;
  padding: 0px 16px;
  ${({ theme }) => theme.typography.subtitle3}
  color: ${({ theme }) => theme.colors.fg.secondary_light};
  text-decoration: none;
  cursor: pointer;
  position: relative;

  /* &:hover, */
  &.active {
    color: ${({ theme }) => theme.colors.fg.link};
    ::before {
      content: " ";
      display: block;
      position: absolute;
      background-color: ${({ theme }) => theme.colors.fg.link};
      bottom: 0;
      height: 4px;
      width: 100%;
      border-radius: 4px 4px 0px 0px;
    }
  }

  i {
    font-size: 22px;
    margin-right: 18px;
    color: ${({ theme }) => theme.colors.icons.secondary};
  }
`;

const PageTitle = styled.h1`
  white-space: nowrap;
  text-transform: uppercase;
  padding: 1rem 0;
  border-bottom: 1px solid ${({ theme }) => theme.colors.border.primary};
`;
