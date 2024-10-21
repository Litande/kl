import React, { useState, useEffect } from "react";
import styled from "styled-components";
import { ILead } from "types";
import LeadDetailsRow from "./LeadDetailsRow";
import Button from "components/button/Button";
import ConfirmationModal from "components/confirmationModal/ConfirmationModal";
import trackingApi from "services/api/tracking";
import MultiSelect, { IOption } from "../multiSelect/MultiSelect";
import moment from "moment";

type Variant = "simple" | "blacklist" | "action";

type LeadDetailsProps = {
  variant?: Variant;
  data: ILead;
  options?: {
    agents: IOption[];
    statuses: IOption[];
  };
  // eslint-disable-next-line @typescript-eslint/no-explicit-any
  onClose: (options: any) => void;
};

const getHeaderText = (variant: Variant) => {
  switch (variant) {
    case "blacklist":
      return "Information";
    case "action":
      return "Action";
    case "simple":
      return null;
  }
};

function LeadDetails({ options, data, onClose, variant = "simple" }: LeadDetailsProps) {
  const [isConfirmationShown, setIsConfirmationShown] = useState(false);

  const [agent, setAgent] = useState([]);
  const [status, setStatus] = useState([]);

  const showConfirmation = () => {
    setIsConfirmationShown(true);
  };

  useEffect(() => {
    if (variant === "action" && data) {
      const status = options.statuses?.find(status => status.value === data.leadStatus);
      const agent = options.agents?.find(status => status.value === Number(data?.affiliateId));

      setStatus(status ? [status] : []);
      setAgent(agent ? [agent] : []);
    }
    // eslint-disable-next-line react-hooks/exhaustive-deps
  }, [data, options]);

  useEffect(() => {
    return () => {
      if (status.length) {
        trackingApi.setLeadStatus({ leadId: data.leadId, status: status[0].value });
      }
      if (agent.length) {
        trackingApi.setLeadAgent({ leadId: data.leadId, agentId: agent[0].value });
      }

      if (variant === "action" && data?.leadStatus !== status[0]?.value) {
        onClose({
          status: status[0]?.value,
        });
      }
    };
    // eslint-disable-next-line react-hooks/exhaustive-deps
  }, [status, agent, variant]);

  const handleClose = () => setIsConfirmationShown(false);

  const onConfirm = () => {
    trackingApi.addBlacklist({ id: data.leadId });
    setIsConfirmationShown(false);
    onClose({ id: data.leadId });
  };

  const getDuration = () => {
    if (data.leadAnsweredAt || data.agentAnsweredAt) {
      const startTime = moment(data.leadAnsweredAt || data.agentAnsweredAt);
      const endTime = moment();

      const hh = moment.utc(endTime.diff(startTime)).format("HH");
      const mm = moment.utc(endTime.diff(startTime)).format("mm");
      const ss = moment.utc(endTime.diff(startTime)).format("ss");

      return `${hh}:${mm}:${ss}`;
    }

    return "";
  };

  if (!data) return null;

  const renderBlacklistBtn = () => (
    <ContentItem>
      <div />
      <Button onClick={showConfirmation}>
        <TaskIcon className="icon-add-blacklist" />
        Add Black List
      </Button>
    </ContentItem>
  );

  const renderActions = () => {
    return (
      <ActionsContainer>
        <MultiSelect
          options={options.statuses}
          onChange={value => setStatus(value)}
          value={status}
          placeholder="Change status"
        />
        <MultiSelect
          options={options.agents}
          onChange={value => setAgent(value)}
          value={agent}
          placeholder="Change Assignments"
        />
      </ActionsContainer>
    );
  };

  const renderFooter = () => {
    switch (variant) {
      case "simple":
        return null;
      case "blacklist":
        return renderBlacklistBtn();
      case "action":
        return renderActions();
    }
  };

  return (
    <Container>
      <Header>
        {getHeaderText(variant)}
        <CrossIcon className="icon-close" />
      </Header>
      <Content>
        <ContentItem>
          <ItemName>ID</ItemName>
          <ItemValue>{data.leadId}</ItemValue>
        </ContentItem>
        <ContentItem>
          <ItemName>Lead status</ItemName>
          <ItemValue>{data.leadStatus}</ItemValue>
        </ContentItem>
        <ContentItem>
          <ItemName>Call duration</ItemName>
          <ItemValue>{getDuration()}</ItemValue>
        </ContentItem>
        <ContentItem>
          <ItemName>Group</ItemName>
          <ItemValue>{data?.leadGroup}</ItemValue>
        </ContentItem>
        <ContentItem>
          <ItemName>Lead details</ItemName>
          <ItemValue>
            <LeadDetailsRow data={data} dividerWidth={5} />
          </ItemValue>
        </ContentItem>
        {renderFooter()}
      </Content>
      {isConfirmationShown && (
        <ConfirmationModal title="Black list" onConfirm={onConfirm} onCancel={handleClose}>
          {`Are you sure to move ${data.name} to the Black List?`}
        </ConfirmationModal>
      )}
    </Container>
  );
}

export default LeadDetails;

const Container = styled.div`
  width: 383px;
  border-radius: 0 0 4px 4px;
  box-shadow: 0 2px 4px ${({ theme }) => theme.colors.border.primary};
  background: ${({ theme }) => theme.colors.bg.secondary};
`;

const Header = styled.div`
  display: flex;
  justify-content: space-between;
  border-radius: 4px 4px 0 0;
  align-items: center;
  padding: 17px 20px;
  background: ${({ theme }) => theme.colors.modal.headerBackground};
  color: ${({ theme }) => theme.colors.btn.primary};
  ${({ theme }) => theme.typography.subtitle3};
  text-transform: uppercase;
`;

const Content = styled.div`
  padding: 0 16px 6px;
`;
const ContentItem = styled.div`
  display: flex;
  justify-content: space-between;
  padding: 10px 0;
  ${({ theme }) => theme.typography.smallText1};
`;
const ItemName = styled.div`
  white-space: nowrap;
  text-transform: uppercase;
  color: ${({ theme }) => theme.colors.fg.secondary_light};
`;

const ActionsContainer = styled.div`
  display: flex;
  flex-direction: column;
  padding: 18px 32px;
  gap: 16px;
`;

const TaskIcon = styled.i`
  margin: 0 6px 0 0;
  font-size: 22px;
`;
const ItemValue = styled.div``;

const CrossIcon = styled.i`
  font-size: 1.5rem;
  color: ${({ theme }) => theme.colors.modal.headerTextColor};
  cursor: pointer;
`;
