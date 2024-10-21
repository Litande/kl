import React, { useState } from "react";
import styled from "styled-components";
import { ICellRendererParams } from "ag-grid-community";
import { IAction, IRow } from "../types";
import Tooltip from "components/tooltipPopover/TooltipPopover";
import ConfirmationModal from "components/confirmationModal/ConfirmationModal";
import CallService from "services/callService/CallService";
import { CallStatus, ConnectionMode } from "services/callService/types";

interface IActionsCellRendererParams extends ICellRendererParams<IRow> {
  actions: IAction[];
}

const ActionsCellRenderer = (props: IActionsCellRendererParams) => {
  const [connectionMode, setConnectionMode] = useState<ConnectionMode>();

  const showConfirmationModal = (mode: ConnectionMode) => {
    if (isCallAvailable) {
      setConnectionMode(mode);
    }
  };

  const hideConfirmationModal = () => {
    setConnectionMode(null);
  };

  const isCallAvailable =
    props.data.managerRtcUrl !== null && props.data.state.toString() === "InTheCall";
  const connectToCall = () => {
    hideConfirmationModal();
    if (CallService.getInstance().status.getValue() !== CallStatus.OFFLINE) {
      CallService.getInstance().disconnect();
      return;
    }
    CallService.getInstance().setCallInfo({
      mode: connectionMode,
      rtcUrl: props.data.managerRtcUrl,
      agent: props.data.name,
      phone: props.data.phoneNumber,
      callTime: props.data.leadAnsweredAt,
    });

    CallService.getInstance().connect();
  };

  const renderComponent = ({ tooltip, component, name, mode }: IAction) => {
    const additionalStyles = {
      style: {
        opacity: isCallAvailable ? 1 : 0.5,
        cursor: isCallAvailable ? "pointer" : "not-allowed",
      },
    };
    if (tooltip && isCallAvailable) {
      return (
        <StyledTooltip width={225} key={`${props.data.id}-${name}`} tooltip={tooltip} showOnHover>
          {component({
            key: `${props.data.id}-${name}`,
            onClick: () => showConfirmationModal(mode),
            ...additionalStyles,
          })}
        </StyledTooltip>
      );
    }

    return component({
      key: `${props.data.id}-${name}`,
      onClick: () => showConfirmationModal(mode),
      ...additionalStyles,
    });
  };

  return (
    <Container>
      {props.actions.map(action => renderComponent(action))}
      <div>
        <audio hidden controls autoPlay id="audioCtl" />
      </div>
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
};

const StyledTooltip = styled(Tooltip)`
  box-sizing: border-box;
  padding: 9px;
  text-align: center;
  ${({ theme }) => theme.typography.smallText2}

  background: ${({ theme }) => theme.colors.bg.ternary};
  box-shadow: 0 1px 4px ${({ theme }) => theme.colors.border.primary};
  border-radius: 4px;
`;

const Container = styled.span`
  display: flex;
  justify-content: center;
  align-items: center;
  height: 100%;

  i {
    font-size: 24px;
    padding: 3px;
    margin: 0 1px 0 0;
    background: ${({ theme }) => theme.colors.btn.secondary};
    color: ${({ theme }) => theme.colors.icons.tertiary};
    cursor: pointer;

    &:first-child {
      border-radius: 4px 0 0 4px;
    }

    &:last-of-type {
      border-radius: 0 4px 4px 0;
    }
  }
`;

export default ActionsCellRenderer;
