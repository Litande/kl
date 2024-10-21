import React, { useState } from "react";
import styled from "styled-components";
import { ICellRendererParams } from "ag-grid-community";

import agentApi from "services/api/agentApi";
import Button from "components/button/Button";
import Player from "components/player/Player";
import LoaderModal from "components/loaderModal/LoaderModal";
import useMakeRequestWithAttempts from "hooks/useMakeRequestWithAttempts";
import RecordUnavailableModal from "components/recordUnavailableModal/RecordUnavailableModal";

import { IRow } from "./types";

const ActionsCellRenderer = (props: ICellRendererParams<IRow>) => {
  const [isPlayerShown, setIsPlayerShown] = useState(false);
  const [audioUrl, setAudioUrl] = useState(null);
  const [isLoaderShown, setIsLoaderShown] = useState(false);
  const [isErrorShown, setIsErrorShown] = useState(false);
  const { makeRequestWithAttempts } = useMakeRequestWithAttempts();
  const { id, firstName, lastName, lastActivity } = props.data;
  const [cellData, setCellData] = useState({
    id,
    firstName,
    lastName,
    lastActivity,
  });

  const handleModalClose = () => {
    setIsPlayerShown(false);
    setIsErrorShown(false);
  };

  const makePlayRequest = async callId =>
    makeRequestWithAttempts({
      onBegin: () => setIsLoaderShown(true),
      request: async () =>
        await agentApi.getRecord({ id: callId }).then(({ data }) => {
          setIsLoaderShown(false);
          const blob = new Blob([data], { type: "audio/ogg" });
          const blobUrl = URL.createObjectURL(blob);
          setAudioUrl(blobUrl);
          setIsPlayerShown(true);
        }),
      onError: () => {
        setIsLoaderShown(false);
        setIsErrorShown(true);
      },
    });

  const handlePlay = async () => {
    await makePlayRequest(cellData.id);
  };

  const getCurrentRowIndex = () => {
    return (
      props?.node?.parent?.allLeafChildren.find(child => parseInt(child.id) === cellData.id)
        ?.rowIndex || 0
    );
  };

  const handlePlayerNext = async () => {
    const nextRow =
      props?.node?.parent?.allLeafChildren[getCurrentRowIndex() + 1] ||
      props?.node?.parent?.allLeafChildren[0];
    if (nextRow) {
      setCellData(nextRow.data);
      await makePlayRequest(nextRow.data?.id);
    }
  };

  const handlePlayerPrev = async () => {
    const prevRow =
      props?.node?.parent?.allLeafChildren[getCurrentRowIndex() - 1] ||
      props?.node?.parent?.allLeafChildren.at(-1);
    if (prevRow) {
      setCellData(prevRow.data);
      await makePlayRequest(prevRow.data?.id);
    }
  };

  return (
    <Container>
      <PlayButton onClick={handlePlay}>
        <PlayIcon className="icon-ic-play" />
        Play
      </PlayButton>
      {isLoaderShown && <LoaderModal id={cellData.id} />}
      {isErrorShown && <RecordUnavailableModal onClose={handleModalClose} />}
      {isPlayerShown && (
        <Player
          id={cellData.id}
          audioUrl={audioUrl}
          clientName={`${cellData.firstName} ${cellData.lastName}`}
          lastCallAt={cellData.lastActivity}
          onClose={handleModalClose}
          onNext={handlePlayerNext}
          onPrev={handlePlayerPrev}
        />
      )}
    </Container>
  );
};

const Container = styled.span`
  display: flex;
  justify-content: center;
  align-items: center;
  height: 100%;
`;

const PlayButton = styled(Button)`
  position: relative;
  width: 140px;
`;
const PlayIcon = styled.i`
  position: absolute;
  left: 12px;
  font-size: 12px;
`;

export default ActionsCellRenderer;
