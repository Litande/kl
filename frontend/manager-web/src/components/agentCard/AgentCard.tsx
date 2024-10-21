import React, { useState } from "react";
import styled from "styled-components";
import Total from "components/total/Total";
import TagsRow from "components/tagsRow/TagsRow";
import EditAgentModal from "components/agentModal/EditAgentModal";
import ConfirmationModal from "components/confirmationModal/ConfirmationModal";
import Tooltip from "components/tooltipPopover/TooltipPopover";

import { AgentStatusStr, agentStatusToLabel, IAgent } from "types";
import CallService from "services/callService/CallService";
import { CallStatus, ConnectionMode } from "services/callService/types";

type CardProps = {
  data: IAgent;
  className?: string;
  onEditAgent?: () => void;
};
// TOdo: make separate actions component
function AgentCard({ className, data, onEditAgent }: CardProps) {
  const [connectionMode, setConnectionMode] = useState<ConnectionMode>();
  const [isAgentModalShown, setIsAgentModalShown] = useState(false);

  const { id, name, state: status, tags, score, managerRtcUrl } = data;

  const handleSave = () => {
    setIsAgentModalShown(false);
    onEditAgent();
  };
  const handleClose = () => {
    setIsAgentModalShown(false);
    onEditAgent();
  };
  const onAgentEdit = () => {
    setIsAgentModalShown(true);
  };

  const showConfirmationModal = (mode: ConnectionMode) => {
    if (isCallAvailable) {
      setConnectionMode(mode);
    }
  };

  const hideConfirmationModal = () => {
    setConnectionMode(null);
  };

  const isCallAvailable = managerRtcUrl && status.toString() === AgentStatusStr.InTheCall;

  const connectToCall = () => {
    hideConfirmationModal();
    if (CallService.getInstance().status.getValue() !== CallStatus.OFFLINE) {
      CallService.getInstance().disconnect();
      return;
    }
    CallService.getInstance().setCallInfo({
      mode: connectionMode,
      rtcUrl: managerRtcUrl,
      agent: name,
      phone: null,
      callTime: null,
    });

    CallService.getInstance().connect();
  };

  return (
    <Container status={status} className={className}>
      <Top>
        <Details>
          <Name>
            <AgentIcon className="icon-agent" />
            {name}
          </Name>
          <Score>
            Score
            <StyledTotal>{score}</StyledTotal>
          </Score>
        </Details>
        <TagsRow id={id} tags={tags} />
      </Top>
      <Bottom>
        <StatusContainer>
          <Status status={status}>{agentStatusToLabel[status]}</Status>
          {status !== AgentStatusStr.FillingFeedback && (
            <ActionIcons isCallAvailable={isCallAvailable}>
              <ActionIcon
                onClick={() => showConfirmationModal(ConnectionMode.ListenOnly)}
                className="icon-headphone"
              />
              {isCallAvailable ? (
                <StyledTooltip
                  width={225}
                  tooltip={"Attention! \n Do you want to connect to the call?"}
                  showOnHover
                >
                  <ActionIcon
                    onClick={() => showConfirmationModal(ConnectionMode.BothDirections)}
                    className="icon-call"
                  />
                </StyledTooltip>
              ) : (
                <ActionIcon
                  onClick={() => showConfirmationModal(ConnectionMode.BothDirections)}
                  className="icon-call"
                />
              )}
              <ActionIcon
                onClick={() => showConfirmationModal(ConnectionMode.AgentOnly)}
                className="icon-conect-call"
              />
            </ActionIcons>
          )}
        </StatusContainer>
        <Action onClick={onAgentEdit}>Manage</Action>
      </Bottom>
      {isAgentModalShown && <EditAgentModal onSave={handleSave} onClose={handleClose} id={id} />}
      {connectionMode && (
        <ConfirmationModal
          title="Connect to call"
          onConfirm={() => connectToCall()}
          onCancel={() => hideConfirmationModal()}
        >
          Are you sure you want to connect to the call?
        </ConfirmationModal>
      )}
    </Container>
  );
}

export default AgentCard;

type ContainerProps = {
  status: AgentStatusStr | string;
};

const Container = styled.div<ContainerProps>`
  box-sizing: border-box;
  position: relative;
  display: flex;
  flex-direction: column;
  width: 383px;
  padding: 0 0 0 10px;
  background: ${({ theme }) => theme.colors.bg.ternary};
  box-shadow: 0 1px 4px ${({ theme }) => theme.colors.border.primary};
  border-radius: 4px;

  &:before {
    content: "";
    position: absolute;
    top: 0;
    left: 0;
    bottom: 0;
    width: 10px;
    background: ${({ theme, status }) => theme.colors.agentStatus[status]?.card};
    border-radius: 4px 0 0 4px;
  }
`;

const Top = styled.div`
  display: flex;
  flex-direction: column;
  padding: 15px 15px 9px 0;
`;

const Details = styled.div`
  display: flex;
  justify-content: space-between;
  align-items: center;
  padding: 0 0 0 20px;
  ${({ theme }) => theme.typography.subtitle1}
`;

const Score = styled.div`
  display: flex;
  justify-content: space-between;
  align-items: center;
  color: ${({ theme }) => theme.colors.fg.secondary_light};
  text-transform: uppercase;
  ${({ theme }) => theme.typography.buttonsText5};
`;
const Name = styled.div`
  white-space: nowrap;
  overflow: hidden;
  text-overflow: ellipsis;
  padding: 0 10px 0 0;
`;

const AgentIcon = styled.i`
  margin: 0 20px 0 0;
  font-size: 24px;
  vertical-align: middle;
`;

const StyledTotal = styled(Total)`
  margin: 0 0 0 20px;
`;

const Bottom = styled.div`
  box-sizing: border-box;
  min-height: 65px;
  background: #f7f7f7;
  flex: 1;
  display: flex;
  align-items: center;
  justify-content: space-between;
  padding: 30px 8px 18px;
  ${({ theme }) => theme.typography.body1}
`;

const StatusContainer = styled.div`
  display: flex;
  align-items: center;
  height: 30px;
`;

const Status = styled.div<ContainerProps>`
  margin: 0 5px 0 0;
  color: ${({ theme, status }) => theme.colors.agentStatus[status]?.text};
`;
const Action = styled.div`
  cursor: pointer;
  text-decoration: underline;
  color: ${({ theme }) => theme.colors.fg.link};
`;

const ActionIcons = styled.div<{ isCallAvailable: boolean }>`
  display: flex;
  margin: 0 5px 0 0;
  opacity: ${({ isCallAvailable }) => (isCallAvailable ? "1" : "0.3")};

  i {
    cursor: ${({ isCallAvailable }) => (isCallAvailable ? "pointer" : "not-allowed")};
  }
`;

const ActionIcon = styled.i`
  font-size: 24px;
  padding: 3px;
  margin: 0 1px 0 0;
  background: ${({ theme }) => theme.colors.btn.secondary};
  color: ${({ theme }) => theme.colors.icons.tertiary};
  cursor: pointer;

  &:first-child {
    border-radius: 4px 0 0 4px;
  }

  &:last-child {
    border-radius: 0 4px 4px 0;
  }
`;

const StyledTooltip = styled(Tooltip)`
  box-sizing: border-box;
  padding: 9px;
  text-align: center;
  ${({ theme }) => theme.typography.smallText2}

  background: ${({ theme }) => theme.colors.bg.ternary};
  box-shadow: 0 1px 4px ${({ theme }) => theme.colors.border.primary};
  border-radius: 4px;
`;
