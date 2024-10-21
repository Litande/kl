import React, { useEffect, useState } from "react";
import styled from "styled-components";
import { ICellRendererParams } from "ag-grid-community";
import TooltipPopover from "components/tooltipPopover/TooltipPopover";
import LeadDetails from "components/leadDetails/LeadDetails";
import { ILead } from "types";
import trackingApi from "services/api/tracking";
import { AxiosResponse } from "axios";

const LeadCellRenderer = (props: ICellRendererParams) => {
  const [leadData, setLeadData] = useState<ILead>(null);
  const [isTooltipShown, setIsTooltipShown] = useState(false);
  const [side, setSide] = useState<"left" | "right">("left");
  const handleLeadDetailsClose = ({ id }) => {
    props.api.applyTransaction({ remove: [{ leadId: id }] });
  };

  useEffect(() => {
    if (isTooltipShown) {
      trackingApi
        .getLeadInfo({ id: props.data.leadId })
        .then(({ data }: AxiosResponse<ILead>) => setLeadData(data));
    } else {
      setLeadData(null);
    }
  }, [isTooltipShown, props.data.leadId]);
  return (
    <Container>
      <UserInfo>
        <StyledTooltip
          width={383}
          position={side}
          onToggle={({ isShown, event }) => {
            setSide(event.clientX > window.innerWidth / 2 ? "left" : "right");
            setIsTooltipShown(isShown);
          }}
          tooltip={
            <LeadDetails data={leadData} variant="blacklist" onClose={handleLeadDetailsClose} />
          }
        >
          <InfoIcon isTooltipShown={true} className="icon-info icon-show-on-hover" />
        </StyledTooltip>
        ID {props.value}
      </UserInfo>
    </Container>
  );
};

export default LeadCellRenderer;

const Container = styled.div`
  display: flex;
  justify-content: space-between;
  align-items: center;
`;

const StyledTooltip = styled(TooltipPopover)`
  margin-left: 5px;

  &:after {
    background: ${({ theme }) => theme.colors.bg.secondary};
  }
`;

const InfoIcon = styled.i<{ isTooltipShown: boolean }>`
  margin: 0 10px 0 0;
  font-size: 24px;
  align-items: center;
  color: ${({ theme }) => theme.colors.leadGroups.darkBlue};
  cursor: pointer;
  ${({ isTooltipShown }) => isTooltipShown && "opacity: 1 !important"};
`;

const UserInfo = styled.div`
  display: flex;
  align-items: center;
  margin: 0 10px 0 0;
`;

const UserIcon = styled.i`
  position: relative;
  margin: 0 16px 0 0;
  font-size: 20px;
  color: ${({ theme }) => theme.colors.icons.primary};
`;
