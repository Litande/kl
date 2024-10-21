import { ColDef } from "ag-grid-community";
import ActionCellRenderer from "./cells/ActionsCellRenderer";

const columnDefs: ColDef[] = [
  {
    headerName: "CID",
    field: "leadId",
    sortable: true,
    flex: 1,
    minWidth: 80,
  },
  {
    headerName: "Phone",
    field: "phoneNumber",
    sortable: true,
    flex: 1,
    minWidth: 100,
  },
  {
    headerName: "Country",
    field: "country",
    sortable: true,
    flex: 1,
    minWidth: 120,
  },
  {
    headerName: "First Name",
    field: "firstName",
    sortable: true,
    flex: 1,
    minWidth: 130,
  },
  {
    headerName: "Last Name",
    field: "lastName",
    sortable: true,
    flex: 1,
    minWidth: 130,
  },
  {
    headerName: "Assigned to Agent",
    field: "assignedAgent",
    sortable: true,
    flex: 1,
    minWidth: 180,
    valueFormatter: params => {
      return `${params.value?.firstName || ""} ${params.value?.lastName || ""}`;
    },
  },
  {
    headerName: "Reg Date",
    field: "registrationTime",
    sortable: true,
    flex: 1,
    minWidth: 120,
  },
  {
    headerName: "Email",
    field: "email",
    sortable: true,
    flex: 1,
    minWidth: 100,
  },
  {
    headerName: "Lead Score",
    field: "leadScore",
    sortable: true,
    flex: 1,
    minWidth: 135,
  },
  {
    headerName: "Total Calls",
    field: "totalCalls",
    sortable: true,
    flex: 1,
    minWidth: 135,
  },
  {
    headerName: "Sales Status",
    field: "leadStatus",
    sortable: true,
    flex: 1,
    minWidth: 150,
  },
  {
    headerName: "Actions",
    field: "",
    sortable: true,
    flex: 1,
    minWidth: 120,
    cellRenderer: ActionCellRenderer,
  },
];

export default columnDefs;
