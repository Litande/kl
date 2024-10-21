import { ContentWrap, PageWrapContent } from "components/layout/AgentLayout";
import React, { useEffect, useMemo, useState } from "react";
import styled from "styled-components";

import Label from "components/label/Label";
import { useAgent } from "data/user/useAgent";
import Button from "components/button/Button";
import { convertSecondsToTimeString } from "utils/timerUtils";
import CNVLeadInfo from "components/board/trader/CNVLeadInfo";
import PageTitle from "components/layout/PageTitle";
import useMute from "hooks/useMute";
import useAgentStatus from "hooks/useAgentStatus";
import useCallingStateSettings from "components/board/states/hooks/useCallingStateSettings";
import useCommentsModal from "components/board/states/hooks/useCommentsModal";
import CallService from "services/callService/CallService";
import GlobalLoader from "components/loader/GlobalLoader";
import AllCommentsModal from "components/allCommentsModal/AllCommentsModal";
import NewCommentModal from "components/newCommentModal/NewCommentModal";

const CallingState = () => {
  const { agent } = useAgent();
  const { updater } = useAgentStatus();
  const lead = useMemo(() => {
    return agent.lead;
  }, [agent, updater]);

  const [isHold, setIsHold] = useState<boolean>(lead?.isHold());
  const { isMute, toggleMute } = useMute();

  const endCallByAgent = () => {
    agent.endCallByAgent();
  };

  const hold = () => {
    agent.hold();
    setIsHold(agent.lead?.isHold());
  };

  const setVoiceMallStatus = () => {
    agent.voiceMail();
  };

  const onNoAnswer = () => {
    agent.noAnswer();
  };

  const [callTime, setCallTime] = useState(0);
  useEffect(() => {
    const subscribe = lead?.callTime?.subscribe(value => {
      setCallTime(value);
    });
    return () => {
      subscribe?.unsubscribe();
    };
  }, [lead]);

  const [isLoading, setIsLoading] = useState(CallService.getInstance().isLoading.getValue());
  useEffect(() => {
    const subscribe = CallService.getInstance().isLoading.subscribe(value => {
      setIsLoading(value);
    });
    return () => {
      subscribe.unsubscribe();
    };
  });

  const { isEndCallButtonEnable, isVoicemailButtonEnable } = useCallingStateSettings(
    agent,
    callTime
  );

  const isShowLoader = useMemo(() => {
    return isLoading || lead === undefined;
  }, [isLoading, lead]);

  const {
    isAllCommentsModalShown,
    isNewCommentsModalShown,
    handleAddCommentButtonClick,
    handleBackButtonClick,
    toggleIsAllCommentsModalShown,
    handleCloseModal,
  } = useCommentsModal();

  return (
    <PageWrapContent>
      <PageTitle label={"Dialing"} />
      <ContentWrap>
        {isShowLoader && <GlobalLoader />}
        {!isShowLoader && (
          <LeadInfoWrap>
            <LeftElementsWrap>
              <Label label={lead?.name} header={"Lead"} />
            </LeftElementsWrap>
            <CenterElementsWrap>
              <Label
                label={convertSecondsToTimeString(callTime)}
                header={"Call Time"}
                styles={{ minWidth: "140px" }}
              />
              <Button
                style={{ display: "none" }}
                variant={isHold ? "primary" : "secondary"}
                onClick={hold}
              >
                {isHold ? "UnHold" : "Hold"}
              </Button>
              <Button variant={isMute ? "primary" : "secondary"} onClick={toggleMute}>
                {isMute ? "Unmute" : "Mute"}
              </Button>
            </CenterElementsWrap>
            <ConfigButtonsWrap>
              <Button onClick={toggleIsAllCommentsModalShown}>Comments</Button>
              <Button disabled={!isVoicemailButtonEnable} onClick={onNoAnswer}>
                NA
              </Button>
              <Button disabled={!isVoicemailButtonEnable} onClick={setVoiceMallStatus}>
                Voicemail
              </Button>
              <Button
                disabled={!isEndCallButtonEnable}
                onClick={endCallByAgent}
                style={{ width: "108px" }}
                data-testid="end-call-button"
              >
                End Call
              </Button>
            </ConfigButtonsWrap>
          </LeadInfoWrap>
        )}
        {!isShowLoader && <CNVLeadInfo />}
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
            onSubmit={handleCloseModal}
          />
        )}
      </ContentWrap>
    </PageWrapContent>
  );
};

export default CallingState;

const LeadInfoWrap = styled.div`
  display: flex;
  flex-direction: row;
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
  justify-content: center;
  gap: 1rem;
`;
