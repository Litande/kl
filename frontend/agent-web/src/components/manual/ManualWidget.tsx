import React from "react";
import styled from "styled-components";
import { useAgent } from "data/user/useAgent";
import { typography } from "globalStyles/theme/fonts";
import useMute from "hooks/useMute";

const ManualWidget = () => {
  const { agent } = useAgent();
  const { isMute, toggleMute } = useMute();

  const onClose = () => {
    agent.endManualCall();
  };

  return (
    <Wrap>
      <ContentWrap>
        <LabelWrap>Manual Call</LabelWrap>
        <LabelWrap data-testid="manual-widget-phone">
          {agent.getActiveLead()?.phoneNumber}
        </LabelWrap>
        <IconsContainer>
          <Icon onClick={toggleMute} className={isMute ? "icon-ic-micOff" : "icon-ic-micOn"} />
          {false && <Icon className={"icon-ic-holdCall"} />}
          <Icon
            onClick={onClose}
            className={"icon-ic-endCall"}
            data-testid="manual-widget-end-call-button"
          />
          <IconClose onClick={onClose} className={"icon-ic-close"} />
        </IconsContainer>
      </ContentWrap>
    </Wrap>
  );
};

export default ManualWidget;

const Wrap = styled.div`
  position: absolute;
  bottom: 0;
  right: 0;
  width: 440px;
  background: ${({ theme }) => theme.colors.bg.primary};
`;

const ContentWrap = styled.div`
  padding: 1rem;
  display: flex;
  align-items: center;
  color: white;
  gap: 0.5rem;
  ${typography.subtitle4}
`;

const LabelWrap = styled.div`
  text-transform: uppercase;
`;

const IconsContainer = styled.div`
  margin-left: auto;
  display: flex;
  flex-direction: row;
  gap: 1rem;
`;

const IconClose = styled.i`
  display: flex;
  align-items: center;
  cursor: pointer;
  font-size: 16px;
`;

const Icon = styled.i`
  font-size: 24px;
  cursor: pointer;
`;
