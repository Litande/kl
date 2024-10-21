import { useEffect, useState } from "react";
import styled from "styled-components";
import Grid from "components/grid/Grid";
import ModalV2, { ModalContent } from "components/modal/ModalV2";
import mockData from "./mock";
import columnDefs from "./colsDef";
import Button from "components/button/Button";

type Props = {
  onConfirm: () => void;
};

const DetailsModal = ({ onConfirm }: Props) => {
  const [rowData, setRowData] = useState(mockData);

  useEffect(() => {
    setRowData(mockData);
  }, []);

  return (
    <StyledModal title="View Details" hasCloseIcon onCancel={onConfirm}>
      <GridWrapper columnDefs={columnDefs} rowData={rowData} />
      <StyledButton onClick={onConfirm}>Close</StyledButton>
    </StyledModal>
  );
};

const StyledModal = styled(ModalV2)`
  min-height: 70vh;
  min-width: 70vw;

  ${ModalContent} {
    display: flex;
    height: 100%;
    justify-content: flex-start;
  }
`;

const GridWrapper = styled(Grid)`
  height: calc(100% - 100px);
  padding-bottom: 30px;
  box-sizing: border-box;
`;

const StyledButton = styled(Button)`
  min-width: 120px;
`;

export default DetailsModal;
