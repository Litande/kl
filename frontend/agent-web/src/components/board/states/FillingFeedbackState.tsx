import React, { useState, useMemo } from "react";

import { PageWrapContent } from "components/layout/AgentLayout";
import { useAgent } from "data/user/useAgent";
import CNVLeadInfo from "components/board/trader/CNVLeadInfo";
import PageTitle from "components/layout/PageTitle";
import Button from "components/button/Button";
import Modal from "components/modal/Modal";
import styled from "styled-components";
import moment from "moment";
import Label from "components/label/Label";
import { convertSecondsToTimeString } from "utils/timerUtils";
import MultiSelect from "components/multiSelect/MultiSelect";
import DatePicker from "components/datePicker/DatePicker";
import DetailsModal from "components/detailsModal/DetailsModal";
import { LeadStatus } from "data/user/types";
import useToggle from "hooks/useToggle";
import useAgentStatus from "hooks/useAgentStatus";
import useFillingFeedbackStateSettings from "components/board/states/hooks/useFillingFeedbackStateSettings";
import AllCommentsModal from "components/allCommentsModal/AllCommentsModal";
import NewCommentModal from "components/newCommentModal/NewCommentModal";
import useCommentsModal from "components/board/states/hooks/useCommentsModal";

const hasCallback = (status: LeadStatus) => {
  const statusesWithCallback = ["CallAgainPersonal"];

  return Boolean(statusesWithCallback.find(item => item === status?.value));
};

const FillingFeedbackState = () => {
  const { agent } = useAgent();
  const { updater } = useAgentStatus();
  const [isDetailsShown, setIsDetailsShown] = useToggle(false);
  const [isCallbackModalShown, setIsCallbackModalShown] = useState(false);
  const [date, setDate] = useState(moment(new Date()).set("minute", 0).toDate());
  const [isSubmitFeedbackClicked, setIsSubmitFeedbackClicked] = useState(false);

  const lead = useMemo(() => {
    return agent.lead;
  }, [updater]);

  const [leadStatus, setLeadStatus] = useState<LeadStatus>(() =>
    lead?.availableStatuses?.find(status => status.value === lead.status)
  );

  const handleCallAgain = () => {
    agent.callAgain();
  };

  const toggleDetailsModal = () => {
    setIsDetailsShown();
  };

  const onSave = () => {
    agent.saveFeedback({
      leadId: lead.id,
      sessionId: lead.sessionId,
      leadStatus: leadStatus?.value,
      remindOn: hasCallback(leadStatus) ? date : null,
      comment: lead.comment.getValue(),
    });
  };

  const handleSubmit = () => {
    if (hasCallback(leadStatus)) {
      setIsCallbackModalShown(true);
    } else if (!lead.comment.getValue().length) {
      setIsSubmitFeedbackClicked(true);
      toggleIsNewCommentsModalShown();
    } else {
      onSave();
    }
  };

  const { isCallAgainButtonEnable } = useFillingFeedbackStateSettings(agent);

  const {
    isAllCommentsModalShown,
    isNewCommentsModalShown,
    handleAddCommentButtonClick,
    handleBackButtonClick,
    toggleIsAllCommentsModalShown,
    toggleIsNewCommentsModalShown,
    handleCloseModal,
  } = useCommentsModal();

  const handleSubmitNewComment = () => {
    return isSubmitFeedbackClicked ? handleSubmit() : handleCloseModal();
  };

  if (!lead) {
    return <></>;
  }

  return (
    <PageWrapContent>
      <PageTitle label={"Dialing/Feedback"} />
      <LeadInfoWrap>
        <LeftElementsWrap>
          <Label label={lead?.name} header={"Lead"} />
        </LeftElementsWrap>
        <CenterElementsWrap>
          <Label
            label={convertSecondsToTimeString(lead?.feedback?.timeCall)}
            header={"Call Time"}
            styles={{ minWidth: "140px" }}
          />
          <StyledSelect
            options={lead?.availableStatuses}
            onChange={([value]) => setLeadStatus(value)}
            value={leadStatus ? [leadStatus] : []}
            label="Status"
          />
        </CenterElementsWrap>
        <ConfigButtonsWrap>
          {/*<Button>Show History</Button>*/}
          <Button onClick={toggleIsAllCommentsModalShown}>Comments</Button>
          <Button onClick={toggleDetailsModal}>View Details</Button>
          <Button disabled={!isCallAgainButtonEnable} onClick={handleCallAgain}>
            Call again
          </Button>
        </ConfigButtonsWrap>
      </LeadInfoWrap>

      <CNVLeadInfo />
      <SubmitWrap>
        <Button
          onClick={handleSubmit}
          style={{ width: "180px" }}
          data-testid="submit-feedback-button"
        >
          Submit Feedback
        </Button>
      </SubmitWrap>
      {isCallbackModalShown && (
        <Modal title="Callback" hasCancel={false} confirmButtonText="Ok" onConfirm={onSave}>
          <ModalText>Select Date and Time for the call</ModalText>
          <Field>
            <StyledDatePicker
              label="From Date"
              initialDate={date}
              onSelect={date => setDate(date)}
            />
          </Field>
          <Field>
            <StyledDatePicker
              label="Time"
              showTime={true}
              initialDate={date}
              onSelect={date => setDate(date)}
            />
          </Field>
        </Modal>
      )}
      {isDetailsShown && <DetailsModal onConfirm={toggleDetailsModal} />}
      {isAllCommentsModalShown && (
        <AllCommentsModal
          leadId={lead?.id}
          onAdd={handleAddCommentButtonClick}
          onClose={handleCloseModal}
          onSubmit={handleCloseModal}
        />
      )}
      {isNewCommentsModalShown && (
        <NewCommentModal
          onBack={handleBackButtonClick}
          onClose={handleCloseModal}
          onSubmit={handleSubmitNewComment}
        />
      )}
    </PageWrapContent>
  );
};

export default FillingFeedbackState;

const SubmitWrap = styled.div`
  display: flex;
  margin: 1rem 0;
  justify-content: flex-end;
`;

const LeadInfoWrap = styled.div`
  display: flex;
  flex-direction: row;
  flex-wrap: wrap;
  border-bottom: 1px solid ${({ theme }) => theme.colors.border.primary};
  padding-bottom: 1rem;
`;

const ConfigButtonsWrap = styled.div`
  display: flex;
  margin-left: auto;
  flex-direction: row;
  gap: 1rem;
  flex: 1;
  align-items: flex-end;
  justify-content: flex-end;
  padding: 1rem 0 0;
`;

const LeftElementsWrap = styled.div`
  flex: 1;
  display: flex;
  flex-direction: row;
`;
const CenterElementsWrap = styled.div`
  display: flex;
  flex: 2 30%;
  align-items: flex-end;
  justify-content: space-evenly;
  gap: 1rem;
`;

const Field = styled.div`
  width: 260px;
`;

const StyledSelect = styled(MultiSelect)`
  width: 265px;
  margin: 0 15px 0 0;
`;

const ModalText = styled.div`
  padding: 6px 0 12px;
  ${({ theme }) => theme.typography.body1};
`;

const StyledDatePicker = styled(DatePicker)`
  margin: 10px 0 0;
`;
