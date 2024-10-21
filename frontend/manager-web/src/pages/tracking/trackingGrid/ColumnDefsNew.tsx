import { IAction } from "./types";
import { ColDef } from "ag-grid-community";
import ActionsCellRenderer from "./cells/ActionsCellRenderer";
import AgentCellRenderer from "./cells/AgentCellRenderer";
import StateCellRenderer from "./cells/StateCellRenderer";
import { ConnectionMode } from "services/callService/types";
import { AgentStatusStr } from "types";
import DurationCellRenderer from "./cells/DurationCellRenderer";

/* Todo: move it to ActionsCellRenderer.tsx component */
const actions: IAction[] = [
  {
    name: "join",
    component: props => <i className="icon-headphone" {...props} />,
    mode: ConnectionMode.ListenOnly,
  },
  {
    name: "call",
    component: props => <i className="icon-call" {...props} />,
    mode: ConnectionMode.BothDirections,
    tooltip: "Attention! \n Do you want to connect to the call?",
  },
  {
    name: "agent",
    component: props => <i className="icon-conect-call" {...props} />,
    mode: ConnectionMode.AgentOnly,
  },
];

const statusWeight = {
  [AgentStatusStr.InTheCall]: 1,
  [AgentStatusStr.FillingFeedback]: 2,
  [AgentStatusStr.WaitingForTheCall]: 3,
  [AgentStatusStr.Dialing]: 4,
  [AgentStatusStr.Busy]: 5,
  [AgentStatusStr.Break]: 6,
  [AgentStatusStr.Offline]: 7,
};

const columnDefs: ColDef[] = [
  {
    field: "name",
    headerName: "Agent",
    sortable: true,
    minWidth: 220,
    flex: 1,
    cellRenderer: AgentCellRenderer,
    // for render update purposes
    valueGetter: gridParams => ({ name: gridParams.data.name, state: gridParams.data.state }),
  },
  {
    field: "state",
    headerName: "",
    minWidth: 135,
    maxWidth: 170,
    flex: 1,
    cellRenderer: ActionsCellRenderer,
    cellRendererParams: {
      actions,
    },
  },
  {
    field: "id",
    headerName: "Id",
    sortable: true,
    minWidth: 75,
    flex: 1,
  },
  {
    field: "leadGroup",
    headerName: "Lead Group",
    sortable: true,
    minWidth: 200,
    flex: 1,
  },
  {
    field: "leadStatus",
    headerName: "Lead Status",
    sortable: true,
    minWidth: 150,
    flex: 1,
  },
  {
    field: "callDur",
    headerName: "Call Duration",
    sortable: true,
    minWidth: 150,
    flex: 1,
    cellRenderer: DurationCellRenderer,
    valueGetter: gridParams => ({ state: gridParams.data.state }),
  },
  {
    field: "state",
    headerName: "State",
    sortable: true,
    minWidth: 200,
    flex: 1,
    cellRenderer: StateCellRenderer,
    sort: "desc",
    comparator: (a, b) => statusWeight[b] - statusWeight[a],
  },
  {
    field: "groups",
    headerName: "Group",
    sortable: true,
    minWidth: 200,
    flex: 1,
  },
  {
    field: "details",
    headerName: "Lead Details",
    sortable: true,
    minWidth: 200,
    flex: 1,
  },
];

export default columnDefs;
