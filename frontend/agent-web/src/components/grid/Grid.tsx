import React, { useMemo, useState } from "react";
import { AgGridReact } from "ag-grid-react";
import { GetRowIdFunc, GetRowIdParams, GridOptions } from "ag-grid-community";

import styled from "styled-components";

import Pagination, { IPagination } from "./Pagination";
import LoadingOverlay from "./LoadingOverlay";

interface IGridOptionsType extends GridOptions {
  className?: string;
  onPageChange?: (page: number) => void;
  pagination?: boolean;
  paginationData?: IPagination;
}

const ITEMS_PER_PAGE = 20;

const Grid = (props: IGridOptionsType) => {
  const { className, ...gridOptions } = props;
  const [gridApi, setGridApi] = useState(null);

  const getRowId = useMemo<GetRowIdFunc>(() => (params: GetRowIdParams) => params.data.id, []);

  const onGridReady = params => {
    // params.api.showLoadingOverlay();
    setGridApi(params.api);
    props.onGridReady && props.onGridReady(params);
  };

  const icons = {
    sortAscending: '<i class="icon-ic-sortAsc"><i class="path1"/><i class="path2"/></i>',
    sortDescending: '<i class="icon-ic-sortDesc"><i class="path1"/><i class="path2"/></i>',
    sortUnSort: '<i class="icon-ic-sort" ></i>',
  };

  return (
    <GridContainer className={`ag-theme-alpine  ${className}`} hasPagination={props.pagination}>
      <AgGridReact
        {...gridOptions}
        rowData={props.rowData}
        columnDefs={props.columnDefs}
        onGridReady={onGridReady}
        onRowSelected={props.onRowSelected}
        onSelectionChanged={props.onSelectionChanged}
        rowSelection={props.rowSelection}
        embedFullWidthRows={true}
        animateRows
        isFullWidthRow={props.isFullWidthRow}
        getRowId={props.getRowId || getRowId}
        unSortIcon
        icons={icons}
        pagination={props.pagination}
        suppressPaginationPanel
        paginationPageSize={props.paginationPageSize || ITEMS_PER_PAGE}
        overlayLoadingTemplate={`${LoadingOverlay}`}
      />
      {props.pagination && (
        <Pagination
          gridApi={gridApi}
          onPageChange={props.onPageChange}
          paginationData={props.paginationData}
        />
      )}
    </GridContainer>
  );
};

export default Grid;

export const GridContainer = styled.div<{ hasPagination: boolean }>`
  width: 100%;
  height: ${({ hasPagination }) =>
    hasPagination ? "calc(100% - 156px) !important" : "calc(100% - 100x) !important"};
`;
