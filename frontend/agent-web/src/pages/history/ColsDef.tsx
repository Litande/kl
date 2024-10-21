import { ColDef } from "ag-grid-community";
import moment from "moment";
import ActionsCellRenderer from "./ActionsCellRenderer";

const columnDefs: ColDef[] = [
  {
    headerName: "id",
    field: "id",
    sortable: true,
    flex: 1,
    minWidth: 70,
  },
  {
    headerName: "First Name",
    field: "firstName",
    sortable: true,
    flex: 1,
    minWidth: 150,
  },
  {
    headerName: "Last Name",
    field: "lastName",
    sortable: true,
    flex: 1,
    minWidth: 150,
  },
  {
    headerName: "Country",
    field: "country",
    sortable: true,
    flex: 1,
    minWidth: 100,
  },
  {
    headerName: "Phone",
    field: "leadPhone",
    sortable: true,
    flex: 1,
    minWidth: 150,
  },
  {
    headerName: "Activity",
    field: "lastActivity",
    sortable: true,
    flex: 1,
    minWidth: 250,
    valueFormatter: params => {
      if (!params.value) return null;
      const date = moment(params.value);
      return `Last call ${date.format("DD/MM/YY HH:mm")} (${date.fromNow()})`;
    },
  },
  {
    headerName: "",
    field: "",
    sortable: true,
    flex: 1,
    minWidth: 170,
    cellRenderer: ActionsCellRenderer,
  },
];

export default columnDefs;
