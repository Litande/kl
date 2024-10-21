import { ColDef } from "ag-grid-community";
import AmountCellRenderer from "./cells/AmountCellRenderer";

const columnDefs: ColDef[] = [
  {
    headerName: "Country",
    field: "country",
    sortable: true,
    flex: 1,
    minWidth: 100,
  },
  {
    headerName: "Number of calls",
    field: "amount",
    sortable: true,
    flex: 1,
    minWidth: 150,
    cellRenderer: AmountCellRenderer,
  },
  {
    headerName: "Average rate",
    field: "rate",
    sortable: true,
    minWidth: 150,
    flex: 1,
  },
  {
    headerName: "Average call time",
    field: "avgTime",
    sortable: true,
    minWidth: 180,
    flex: 1,
  },
];

export default columnDefs;
