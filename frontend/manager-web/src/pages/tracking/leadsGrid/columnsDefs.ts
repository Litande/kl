import { ColDef } from "ag-grid-community";
import AmountCellRenderer from "./cells/AmountCellRenderer";

const columnDefs: ColDef[] = [
  {
    headerName: "Country",
    field: "country",
    sortable: true,
    flex: 1,
    minWidth: 120,
  },
  {
    headerName: "Amount",
    field: "amount",
    sortable: true,
    flex: 1,
    minWidth: 110,
    cellRenderer: AmountCellRenderer,
  },
  {
    headerName: "Max time",
    field: "maxTime",
    sortable: true,
    minWidth: 120,
    flex: 1,
    valueFormatter: params => {
      if (!params.value) return;

      const [hh, mm, ss] = params.value.split(":");

      return `${hh}:${mm}:${ss.substring(0, 2)}`;
    },
  },
  {
    headerName: "Avg time",
    field: "avgTime",
    sortable: true,
    minWidth: 120,
    flex: 1,
    valueFormatter: params => {
      if (!params.value) return;

      const [hh, mm, ss] = params.value.split(":");

      return `${hh}:${mm}:${ss.substring(0, 2)}`;
    },
  },
];

export default columnDefs;
