import { ColDef } from "ag-grid-community";
import moment from "moment";

const getFormattedDuration = duration => moment.utc(duration * 1000).format("HH:mm:ss");

const columnDefs: ColDef[] = [
  {
    headerName: "CID",
    field: "callId",
    sortable: true,
    flex: 0.7,
    minWidth: 67,
  },
  {
    headerName: "Call Type",
    field: "callType",
    sortable: true,
    flex: 1,
    minWidth: 90,
  },
  {
    headerName: "Start At",
    field: "startedAt",
    sortable: true,
    flex: 1,
    minWidth: 140,
    valueFormatter: params => {
      if (!params?.value) return;
      return moment(params.value).format("DD/MM/YY HH:mm:ss");
    },
  },
  {
    headerName: "Client Name",
    field: "leadName",
    sortable: true,
    flex: 1,
    minWidth: 144,
  },
  {
    headerName: "Brand",
    field: "brand",
    sortable: true,
    flex: 1,
    minWidth: 122,
  },
  {
    headerName: "Group \n(online/offline)",
    field: "groupName",
    sortable: true,
    flex: 1.2,
    minWidth: 140,
  },
  {
    headerName: "Sales \nStatus After",
    field: "leadStatusAfter",
    sortable: true,
    flex: 1,
    minWidth: 140,
  },
  {
    headerName: "Caller Agent",
    field: "userName",
    sortable: true,
    flex: 1,
    minWidth: 147,
  },
  {
    headerName: "Duration",
    field: "duration",
    sortable: true,
    flex: 1,
    minWidth: 100,
    valueFormatter: params => {
      if (!params?.value) return;
      return getFormattedDuration(params.value);
    },
  },
  {
    headerName: "Bill Duration",
    field: "billDuration",
    sortable: true,
    flex: 0.8,
    minWidth: 125,
    valueFormatter: params => {
      if (!params?.value) return;
      return getFormattedDuration(params.value);
    },
  },
  {
    headerName: "Hangup",
    field: "hangupStatus",
    sortable: true,
    flex: 1,
    minWidth: 100,
  },
  {
    headerName: "Caller ID",
    field: "leadId",
    sortable: true,
    flex: 0.8,
    minWidth: 100,
  },
  {
    headerName: "To (Phone \nDestination)",
    field: "leadPhone",
    sortable: true,
    flex: 1.2,
    minWidth: 155,
  },
];

export default columnDefs;
