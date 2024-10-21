import { ColDef } from "ag-grid-community";
import ScoreCellRenderer from "./cells/ScoreCellRenderer";
import LeadCellRenderer from "./cells/LeadCellRenderer";

const columnDefs: ColDef[] = [
  {
    headerName: "Leads ID",
    field: "leadId",
    sortable: true,
    flex: 1,
    minWidth: 110,
    cellRenderer: LeadCellRenderer,
  },
  {
    headerName: "Score",
    field: "leadScore",
    sortable: true,
    flex: 1,
    cellRenderer: ScoreCellRenderer,
  },
  {
    headerName: "Status",
    field: "leadStatus",
    sortable: true,
    flex: 1,
    minWidth: 170,
  },
];

export default columnDefs;
