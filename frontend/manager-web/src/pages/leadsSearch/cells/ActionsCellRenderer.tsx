import React, { useEffect, useState } from "react";
import styled from "styled-components";
import { ICellRendererParams } from "ag-grid-community";
import TooltipPopover from "components/tooltipPopover/TooltipPopover";
import LeadDetails from "components/leadDetails/LeadDetails";
import trackingApi from "services/api/tracking";
import { AxiosResponse } from "axios";
import { ILead } from "types";
import ConfirmationModal from "components/confirmationModal/ConfirmationModal";

const ActionsCellRenderer = (props: ICellRendererParams) => {
  const [isConfirmationShown, setIsConfirmationShown] = useState(false);
  const [leadData, setLeadData] = useState<ILead>(null);
  const [isTooltipShown, setIsTooltipShown] = useState(false);
  const showConfirmation = () => setIsConfirmationShown(true);
  const hideConfirmation = () => setIsConfirmationShown(false);

  const addToBlacklist = () => {
    trackingApi.addBlacklist({ id: props.data.leadId });
    setIsConfirmationShown(false);
  };

  const updateRowData = ({ status }) => {
    props.api.applyTransaction({ update: [{ ...props.data, leadStatus: status }] });
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
      <StyledTooltip
        width={383}
        position="left"
        onToggle={({ isShown }) => setIsTooltipShown(isShown)}
        tooltip={
          <LeadDetails
            data={leadData}
            variant="action"
            options={props.context.leadOptions}
            onClose={updateRowData}
          />
        }
      >
        <InfoIcon isTooltipShown={isTooltipShown} className="icon-info" />
      </StyledTooltip>
      <BlacklistIcon className="icon-add-blacklist" onClick={showConfirmation} />
      {isConfirmationShown && (
        <ConfirmationModal
          title="Black list"
          onConfirm={addToBlacklist}
          onCancel={hideConfirmation}
          hasCloseIcon
        >
          {`Are you sure to move ${props.data.firstName} ${props.data.lastName} to the Black List?`}
        </ConfirmationModal>
      )}
    </Container>
  );
};

export default ActionsCellRenderer;

const StyledTooltip = styled(TooltipPopover)`
  margin-left: 5px;

  &:after {
    background: ${({ theme }) => theme.colors.bg.secondary};
  }
`;

const Container = styled.div`
  display: flex;
  justify-content: space-between;
  align-items: center;
  height: 100%;
`;

const InfoIcon = styled.i<{ isTooltipShown: boolean }>`
  margin: 0 18px 0 0;
  font-size: 24px;
  color: ${({ theme }) => theme.colors.leadGroups.darkBlue};
  cursor: pointer;
`;

const BlacklistIcon = styled.i`
  box-sizing: border-box;
  padding: 5px;
  font-size: 20px;
  border: 1px solid ${({ theme }) => theme.colors.btn.secondary};
  color: ${({ theme }) => theme.colors.btn.secondary};
  cursor: pointer;
`;
