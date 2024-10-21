import React, { useEffect, useState } from "react";
import styled from "styled-components";
import { ColDef, GridReadyEvent, ICellRendererParams } from "ag-grid-community";
import { AxiosResponse } from "axios";
import TooltipPopover from "components/tooltipPopover/TooltipPopover";
import Grid from "components/grid/Grid";
import trackingApi from "services/api/tracking";

import { ICountryItem } from "../types";
import LeadCellRenderer from "./LeadCellRenderer";

const columnDefs: ColDef[] = [
  {
    headerName: "Lead Id",
    field: "id",
    sortable: true,
    flex: 1,
    cellRenderer: LeadCellRenderer,
  },
  {
    headerName: "Country",
    field: "country",
    sortable: true,
    flex: 1,
  },
];

const AmountCellRenderer = (props: ICellRendererParams<ICountryItem>) => {
  const [gridApi, setGridApi] = useState(null);
  const [leadData, setLeadData] = useState([]);
  const [isTooltipShown, setIsTooltipShown] = useState(false);

  useEffect(() => {
    if (isTooltipShown) {
      trackingApi.getLeadStatInfo({ id: props.data.id }).then(({ data }: AxiosResponse) => {
        setLeadData(data);
      });
    } else {
      setLeadData([]);
    }
    // eslint-disable-next-line react-hooks/exhaustive-deps
  }, [isTooltipShown, gridApi]);

  const onGridReady = (params: GridReadyEvent) => {
    setGridApi(params.api);
  };

  const renderGrid = () => {
    return (
      <GridContainer
        rowData={leadData}
        columnDefs={columnDefs}
        rowSelection="single"
        onGridReady={onGridReady}
        overlayLoadingTemplate={null}
      />
    );
  };
  return (
    <Container>
      <StyledTooltip
        position="left"
        width={420}
        onToggle={({ isShown }) => setIsTooltipShown(isShown)}
        tooltip={renderGrid()}
      >
        <Amount>{props.value}</Amount>
      </StyledTooltip>
    </Container>
  );
};

export default AmountCellRenderer;

const Container = styled.div`
  display: flex;
  justify-content: space-between;
  align-items: center;
`;
const Amount = styled.div`
  cursor: pointer;
`;

const StyledTooltip = styled(TooltipPopover)`
  box-sizing: border-box;
  width: 420px;
  height: 390px;
  background: ${({ theme }) => theme.colors.bg.ternary};
  box-shadow: 0 2px 8px ${({ theme }) => theme.colors.border.primary};
  border-radius: 4px;
  padding: 15px;
`;

const GridContainer = styled(Grid)`
  width: 100%;
  height: 100%;
  position: relative;
`;
