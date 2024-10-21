import { useEffect } from "react";
import moment from "moment";
import { ICellRendererParams } from "ag-grid-community";
import useTimer from "hooks/useTimer";
import { IRow } from "pages/tracking/trackingGrid/types";
import { AgentStatusStr } from "types";

export default function DurationCellRenderer(props: ICellRendererParams<IRow>) {
  const leadAnsweredDate = props.data.leadAnsweredAt ? moment(props.data.leadAnsweredAt) : moment();
  const callFinishedDate = props.data.callFinishedAt ? moment(props.data.callFinishedAt) : moment();
  const { start, pause, reset, total } = useTimer({
    timestampStart: leadAnsweredDate.valueOf(),
    timestampFinish: callFinishedDate.valueOf(),
  });

  useEffect(() => {
    if (AgentStatusStr.Dialing === props.data.state) {
      reset(0, false);
      return;
    }
    if (AgentStatusStr.InTheCall === props.data.state) {
      start();
      return;
    }
    pause();
    // eslint-disable-next-line react-hooks/exhaustive-deps
  }, [props.data.state]);

  return <div>{total}</div>;
}
