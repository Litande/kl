import React, { useState } from "react";
import { useSelector } from "react-redux";
import styled from "styled-components";
import { GridApi, GridReadyEvent } from "ag-grid-community";

import Grid from "components/grid/Grid";
import Button from "components/button/Button";
import { callListSelector, isLoadingSelector } from "pages/calls/selector";
import { fetchAudioCall } from "pages/calls/persistance";
import { getBlobUrl } from "pages/calls/utils";
import downloadFile from "utils/downloadFile";
import Player from "components/player/Player";
import LoaderModal from "components/loaderModal/LoaderModal";
import RecordUnavailableModal from "components/recordUnavailableModal/RecordUnavailableModal";
import useMakeRequestWithAttempts from "hooks/useMakeRequestWithAttempts";
import behavior from "pages/calls/behavior";
import useBehavior from "hooks/useBehavior";

import columnDefs from "./columnsDefs";
import { IRow } from "./types";

const CallRecordingsGrid = () => {
  const { items: rowData, ...paginationData } = useSelector(callListSelector);
  const isLoading = useSelector(isLoadingSelector);
  const [gridApi, setGridApi] = useState<GridApi>(null);
  const [selectedRow, setSelectedRow] = useState<IRow>(null);
  const [audioUrl, setAudioUrl] = useState(null);
  const [isPlayerShown, setIsPlayerShown] = useState(false);
  const [isLoaderShown, setIsLoaderShown] = useState(false);
  const [isErrorShown, setIsErrorShown] = useState(false);
  const { makeRequestWithAttempts } = useMakeRequestWithAttempts();
  const { applyPagination, getCalls } = useBehavior(behavior);

  const onGridReady = (params: GridReadyEvent) => {
    const { api } = params;
    setGridApi(api);
  };
  const onSelectionChanged = () => {
    setSelectedRow(gridApi.getSelectedRows()[0]);
  };

  const makePlayRequest = async callId =>
    makeRequestWithAttempts({
      onBegin: () => setIsLoaderShown(true),
      request: async () =>
        await fetchAudioCall(callId).then(audioCall => {
          setIsLoaderShown(false);
          setAudioUrl(getBlobUrl(audioCall));
          setIsPlayerShown(true);
        }),
      onError: () => {
        setIsLoaderShown(false);
        setIsErrorShown(true);
      },
    });

  const handlePlay = async () => {
    const callId = selectedRow?.callId;
    if (callId) {
      await makePlayRequest(callId);
    }
  };

  const handleDownload = async () => {
    const callId = selectedRow?.callId;
    if (callId) {
      await makeRequestWithAttempts({
        onBegin: () => setIsLoaderShown(true),
        request: async () =>
          await fetchAudioCall(callId).then(audioCall => {
            setIsLoaderShown(false);
            downloadFile(getBlobUrl(audioCall), `recording-${callId}.ogg`);
          }),
        onError: () => {
          setIsLoaderShown(false);
          setIsErrorShown(true);
        },
      });
    }
  };

  const onPageChange = page => {
    applyPagination({ page });
    getCalls();
  };

  const handleModalClose = () => {
    setIsPlayerShown(false);
    setIsErrorShown(false);
  };

  const getCurrentRowId = () => {
    const callId = selectedRow?.callId;
    if (callId) {
      return rowData.findIndex((row: IRow) => row.callId === callId);
    }
    return null;
  };

  const handlePlayerNext = async () => {
    const nextRow: IRow = rowData[getCurrentRowId() + 1] || rowData.at(0);
    if (nextRow) {
      setSelectedRow(nextRow);
      await makePlayRequest(nextRow.callId);
    }
  };

  const handlePlayerPrev = async () => {
    const prevRow: IRow = rowData[getCurrentRowId() - 1] || rowData.at(-1);
    if (prevRow) {
      setSelectedRow(prevRow);
      await makePlayRequest(prevRow.callId);
    }
  };

  return (
    <Container>
      <Actions>
        <PlayButton disabled={!selectedRow} type="submit" onClick={handlePlay}>
          <i className="icon-play" />
          Play
        </PlayButton>
        <DownloadButton
          disabled={!selectedRow}
          type="button"
          variant="secondary"
          onClick={handleDownload}
        >
          <i className="icon-download" />
          Download
        </DownloadButton>
        {isLoaderShown && <LoaderModal id={selectedRow?.callId} />}
        {isErrorShown && <RecordUnavailableModal onClose={handleModalClose} />}
        {isPlayerShown && (
          <Player
            id={selectedRow?.callId}
            audioUrl={audioUrl}
            clientName={selectedRow?.leadName}
            agentName={selectedRow?.userName}
            onClose={handleModalClose}
            onNext={handlePlayerNext}
            onPrev={handlePlayerPrev}
          />
        )}
      </Actions>
      <GridContainer>
        <StyledGrid
          isLoading={isLoading}
          onSelectionChanged={onSelectionChanged}
          rowData={rowData}
          columnDefs={columnDefs}
          onGridReady={onGridReady}
          rowSelection="single"
          rowMultiSelectWithClick={true}
          animateRows={true}
          pagination={true}
          onPageChange={onPageChange}
          paginationData={paginationData}
          getRowId={params => params.data.callId}
        />
      </GridContainer>
    </Container>
  );
};

export default CallRecordingsGrid;

const Container = styled.div`
  margin-top: 16px;
  width: 100%;
  height: 100%;
  display: flex;
  flex-direction: column;
  gap: 17px;
`;

const GridContainer = styled.div`
  width: 100%;
  height: 100%;
  overflow-y: auto;
`;

const StyledGrid = styled(Grid)`
  .ag-header-cell-label .ag-header-cell-text {
    white-space: pre-wrap !important;
  }

  .ag-header-cell,
  .ag-cell {
    padding-left: 8px;
    padding-right: 8px;
  }
`;

const Actions = styled.div`
  display: flex;
  gap: 16px;
  width: 100%;

  .icon-download:before {
    color: ${({ theme }) => theme.colors.btn.secondary};
  }
`;

const buttonWidth = 136;

const DownloadButton = styled(Button)`
  width: ${buttonWidth}px;
  margin: 0 16px 0 0;
  display: flex;
  gap: 13px;
`;

const PlayButton = styled(Button)`
  justify-content: flex-start;
  padding: 8px 8px;
  gap: 25px;

  i {
    font-size: 22px;
  }

  width: ${buttonWidth}px;
`;
