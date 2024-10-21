import { ColDef } from "ag-grid-community";
import moment from "moment";

import AgentsNameCellRenderer from "components/allCommentsModal/AgentsNameCellRenderer";

const columnDefs: ColDef[] = [
  {
    headerName: "Agent's name",
    field: "agentFullName",
    sortable: true,
    flex: 1,
    minWidth: 100,
    cellRenderer: AgentsNameCellRenderer,
  },
  {
    headerName: "Sales status",
    field: "leadStatus",
    sortable: true,
    flex: 1,
    minWidth: 100,
  },
  {
    headerName: "Comments",
    field: "comment",
    sortable: true,
    flex: 1,
    minWidth: 100,
  },
  {
    headerName: "Date",
    field: "createdAt",
    sortable: true,
    flex: 1,
    minWidth: 100,
    valueFormatter: params => params.value && moment(params.value).format("DD/MM/yy HH:mm:ss"),
  },
];

export default columnDefs;
