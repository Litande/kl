import { useEffect, useState } from "react";
import styled from "styled-components";
import { ICellRendererParams } from "ag-grid-community";
import { AxiosResponse } from "axios";

import FullNameTooltip from "components/fullNameTooltip/FullNameTooltip";
import TooltipPopover from "components/tooltipPopover/TooltipPopover";
import AgentCard from "components/agentCard/AgentCard";

import { IRow } from "../types";
import trackingApi from "services/api/tracking";
import { IAgent } from "types";

// Todo: check agent types across the whole app
const defaults = {
  agentId: null,
  userName: "",
  email: "",
  firstName: "",
  lastName: "",
  score: null,
  status: "",
  teamIds: [],
  leadQueueIds: [],
  tags: [],
  id: null,
  name: "",
};

const AgentCellRenderer = (props: ICellRendererParams<IRow>) => {
  const [agent, setAgent] = useState<IAgent>(defaults);
  const [isTooltipShown, setIsTooltipShown] = useState(false);

  const updateAgent = () => {
    trackingApi
      .getAgent({ id: props.data.id })
      .then(({ data }: AxiosResponse<IAgent>) =>
        setAgent({ ...data, id: data.agentId, name: `${data.firstName} ${data.lastName}` })
      );
  };

  useEffect(() => {
    if (isTooltipShown) {
      setAgent(props.data);
    } else {
      setAgent(defaults);
    }
  }, [isTooltipShown, props.data]);

  return (
    <Container>
      <UserIcon className="icon-user" />
      <UserInfo>
        <FullNameTooltip>{props.data.name}</FullNameTooltip>
      </UserInfo>
      <StyledTooltip
        width={383}
        position="top"
        onToggle={({ isShown }) => setIsTooltipShown(isShown)}
        tooltip={<AgentCard data={agent} onEditAgent={updateAgent} />}
      >
        <InfoIcon className="icon-info" />
      </StyledTooltip>
    </Container>
  );
};

const Container = styled.div`
  position: relative;
  display: flex;
  align-items: center;
`;

const UserInfo = styled.div`
  display: flex;
  align-items: center;
  max-width: calc(100% - 60px);
`;

const UserIcon = styled.i`
  position: relative;
  margin: 0 16px 0 0;
  font-size: 20px;
  color: ${({ theme }) => theme.colors.icons.primary};
`;

const StyledTooltip = styled(TooltipPopover)`
  &:after {
    background: ${({ theme }) => theme.colors.bg.light};
  }
`;

const InfoIcon = styled.i`
  position: absolute;
  right: 0;
  font-size: 24px;
  color: ${({ theme }) => theme.colors.leadGroups.darkBlue};
  cursor: pointer;
`;

export default AgentCellRenderer;
