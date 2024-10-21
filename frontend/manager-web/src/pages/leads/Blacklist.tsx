import styled from "styled-components";
import Modal from "components/modal/Modal";
import Grid from "components/grid/Grid";
import { useEffect, useState } from "react";
import trackingApi from "../../services/api/tracking";
import { GridApi, GridReadyEvent } from "ag-grid-community";

const columnDefs = [
  {
    width: 70,
    checkboxSelection: true,
    headerCheckboxSelection: true,
  },
  {
    headerName: "Cid",
    field: "id",
    sortable: true,
    flex: 1,
  },
  {
    headerName: "Phone",
    field: "phone",
    sortable: true,
    flex: 1,
  },
  {
    headerName: "Country",
    field: "country",
    sortable: true,
    flex: 1,
  },
  {
    headerName: "First name",
    field: "firstName",
    sortable: true,
    flex: 1,
  },
  {
    headerName: "Last name",
    field: "lastName",
    sortable: true,
    flex: 1,
  },
  {
    headerName: "Source",
    field: "source",
    sortable: true,
    flex: 1,
  },
  {
    headerName: "Email",
    field: "email",
    sortable: true,
    flex: 1,
  },
  {
    headerName: "Sales status",
    field: "salesStatus",
    sortable: true,
    flex: 1,
    minWidth: 150,
  },
];

type Props = {
  onClose: () => void;
  onSave: () => void;
};

const Blacklist = ({ onClose, onSave }: Props) => {
  const [isLoading, setIsLoading] = useState(true);
  const [gridApi, setGridApi] = useState<GridApi>(null);

  const onGridReady = (params: GridReadyEvent) => {
    const { api } = params;
    setGridApi(api);
  };

  const onConfirm = () => {
    const selectedRows = gridApi.getSelectedRows();

    trackingApi.removeBlacklist({ ids: selectedRows.map(({ id }) => id) });
    onSave();
  };

  const [rowData, setRowData] = useState(null);

  useEffect(() => {
    trackingApi
      .getBlacklist()
      .then(({ data: { items } }) => {
        setRowData(items);
      })
      .finally(() => {
        setIsLoading(false);
      });
  }, []);

  return (
    <StyledModal
      title="Black list"
      onConfirm={onConfirm}
      onCancel={onClose}
      confirmButtonText="Extract"
      hasCloseIcon
    >
      <StyledGrid
        isLoading={isLoading}
        onGridReady={onGridReady}
        rowData={rowData}
        columnDefs={columnDefs}
        rowSelection="multiple"
        className="blacklist"
        rowMultiSelectWithClick={true}
      />
    </StyledModal>
  );
};

const StyledModal = styled(Modal)`
  height: 75vh;
  width: 70vw;
  max-width: 1440px;
`;

const StyledGrid = styled(Grid)``;

export default Blacklist;
