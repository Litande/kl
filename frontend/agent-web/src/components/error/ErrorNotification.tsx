import React from "react";
import styled from "styled-components";
import { typography } from "globalStyles/theme/fonts";
import CallError from "data/errors/CallError";

type ComponentProps = {
  error: CallError;
};

const ErrorNotification = ({ error }: ComponentProps) => {
  return (
    <Wrap>
      <Content>
        <TitleContent>
          <IconRound />
          {error.code}
        </TitleContent>
        <DescriptionContent> {error.message} </DescriptionContent>
      </Content>
    </Wrap>
  );
};

export default ErrorNotification;

const Wrap = styled.div`
  position: absolute;
  display: flex;
  top: 4rem;
  right: 4rem;
  z-index: 2;
  width: 400px;
  background: white;
  border-radius: 8px;
`;

const Content = styled.div`
  display: flex;
  width: 100%;
  flex-direction: column;
  padding: 1rem 3rem;
  background: rgba(227, 76, 76, 0.05);
  border: 1px solid #e34c4c;
  box-shadow: 0 2px 8px rgba(36, 36, 36, 0.24);
  border-radius: 8px;
`;

const IconRound = styled.span`
  position: absolute;
  left: -1.5rem;
  top: 1px;
  width: 12px;
  height: 12px;
  border-radius: 8px;
  border: 2px solid ${({ theme }) => theme.colors.error.errorTitle};
  margin-top: 2px;
`;

const TitleContent = styled.div`
  display: flex;
  position: relative;
  ${typography.subtitle1}
  color: ${({ theme }) => theme.colors.error.errorTitle};
`;

const DescriptionContent = styled.div`
  ${typography.body2}
  color: ${({ theme }) => theme.colors.error.errorDesc};
`;
