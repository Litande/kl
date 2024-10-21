import { ColDef } from "ag-grid-community";
import TagsCellRenderer from "./TagsCellRenderer";

const columnDefs: ColDef[] = [
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
    headerName: "Team",
    field: "teamName",
    sortable: true,
    flex: 1,
    minWidth: 100,
  },
  {
    headerName: "Score",
    field: "score",
    sortable: true,
    flex: 1,
    minWidth: 100,
  },
  {
    headerName: "Tags",
    field: "tags",
    sortable: true,
    flex: 5,
    autoHeight: true,
    cellRenderer: ({ data, value, rowIndex }) => (
      <TagsCellRenderer rowIndex={rowIndex} data={data} value={value} />
    ),
    cellStyle: () => ({
      display: "flex",
      alignItems: "center",
      justifyContent: "space-between",
    }),
  },
];

export default columnDefs;
