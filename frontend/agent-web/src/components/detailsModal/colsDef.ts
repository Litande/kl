import { ColDef } from "ag-grid-community";
import moment from "moment";

const columnDefs: ColDef[] = [
  {
    headerName: "Agent's name",
    field: "firstName",
    sortable: true,
    flex: 1,
    minWidth: 100,
    valueFormatter: params => `${params.value} ${params.data.lastName}`,
  },
  {
    headerName: "Phone",
    field: "phone",
    sortable: true,
    flex: 1,
    minWidth: 100,
  },
  {
    headerName: "Call Duration",
    field: "totalCallsSecond",
    sortable: true,
    flex: 1,
    minWidth: 100,
    valueFormatter: params => params.value && moment.utc(params.value * 1000).format("HH:mm:ss"),
  },
  {
    headerName: "Date",
    field: "lastCallAt",
    sortable: true,
    flex: 1,
    minWidth: 100,
    valueFormatter: params => params.value && moment(params.value).format("DD.MM.YY"),
  },
  {
    headerName: "Sales status",
    field: "leadStatus",
    sortable: true,
    flex: 1,
    minWidth: 100,
  },
];

export default columnDefs;
