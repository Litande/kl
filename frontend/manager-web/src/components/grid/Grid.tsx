import React, { useMemo, useState, useEffect } from "react";
import { AgGridReact } from "ag-grid-react";
import { GetRowIdFunc, GetRowIdParams, GridOptions } from "ag-grid-community";
import Pagination, { IPagination } from "./Pagination";

import { LoadingOverlay } from "./LoadingOverlay";
import styled from "styled-components";

interface IGridOptionsType extends GridOptions {
  className?: string;
  isLoading?: boolean;
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
    sortAscending: '<i class="icon-sorting-up"><i class="path1"/><i class="path2"/></i>',
    sortDescending: '<i class="icon-sorting-down"><i class="path1"/><i class="path2"/></i>',
    sortUnSort: '<i class="icon-sorting" ></i>',
  };

  useEffect(() => {
    if (props.isLoading) {
      gridApi?.showLoadingOverlay();
    } else {
      gridApi?.hideOverlay();
      if (!props.rowData?.length) {
        gridApi?.showNoRowsOverlay();
      }
    }
  }, [props.isLoading, gridApi]);

  return (
    <GridContainer
      hasPagination={props.pagination}
      className={`ag-theme-alpine ${
        props.isFullWidthRow ? "fullRowSelectionMode" : ""
      }  ${className}`}
    >
      <AgGridReact
        {...gridOptions}
        rowData={props.rowData}
        columnDefs={props.columnDefs}
        onGridReady={onGridReady}
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
  height: 100%;

  > div:first-child {
    height: ${({ hasPagination }) => (hasPagination ? "calc(100% - 60px) !important" : "auto")};
  }
`;
