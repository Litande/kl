import { FC, useEffect, useState } from "react";
import styled from "styled-components";

import ModalV2, { ModalContent } from "components/modal/ModalV2";
import Button from "components/button/Button";
import Grid from "components/grid/Grid";
import leadsApi from "services/api/leads";

import columnDefs from "./colsDef";

type Props = {
  leadId: number;
  onAdd: () => void;
  onClose: () => void;
  onSubmit: () => void;
};

const AllCommentsModal: FC<Props> = ({ leadId, onClose, onAdd, onSubmit }) => {
  const [rowData, setRowData] = useState([]);
  const addNewCommentHandler = () => onAdd();
  const submitHandler = () => onClose();

  useEffect(() => {
    leadsApi
      .getComments(leadId, {
        page: 1,
        pageSize: 9999,
        totalCount: 9999,
      })
      .then(({ data }) => setRowData(data?.items || []));
  }, []);

  return (
    <StyledModal title={`Comments`} onCancel={onClose} hasCloseIcon>
      <Body>
        <GridWrapper columnDefs={columnDefs} rowData={rowData} />
      </Body>
      <Footer>
        <ButtonsContainer>
          <Button onClick={addNewCommentHandler}>Add</Button>
          <Button onClick={submitHandler}>Submit</Button>
        </ButtonsContainer>
      </Footer>
    </StyledModal>
  );
};

export default AllCommentsModal;

const ButtonsContainer = styled.div`
  display: flex;
  align-items: center;
  justify-content: flex-end;
  padding-left: 16px;
  padding-right: 16px;
  width: 100%;
  gap: 16px;
`;

const Body = styled.div`
  height: 81%;
  width: 100%;
`;

const Footer = styled.div`
  display: flex;
  height: 68px;
  width: 100%;
`;

const StyledModal = styled(ModalV2)`
  height: 625px;
  width: 950px;

  ${ModalContent} {
    justify-content: flex-start;
    padding: 0;
    height: 100%;
  }
`;

const GridWrapper = styled(Grid)`
  height: 100%;
  box-sizing: border-box;
`;
