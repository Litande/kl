import React, { useEffect, useMemo, useState } from "react";
import styled from "styled-components";
import { useSelector } from "react-redux";

import Grid from "components/grid/Grid";
import useBehavior from "hooks/useBehavior";
import behavior from "pages/tags/behavior";
import { filteredAgentsSelector, isLoadingSelector } from "pages/tags/selector";
import { GetRowIdFunc, GetRowIdParams } from "ag-grid-community";

import columnDefs from "./ColumnDefs";

export const DEFAULT_ROW_HEIGHT = 58;

const TagsGrid = () => {
  const { getAllAgents } = useBehavior(behavior);
  const rowData = useSelector(filteredAgentsSelector);
  const isLoading = useSelector(isLoadingSelector);
  const [gridApi, setGridApi] = useState(null);
  const [pagination, setPagination] = useState({
    page: 1,
    pageSize: 20,
    totalCount: null,
  });

  useEffect(() => {
    setPagination({
      ...pagination,
      page: 1,
      totalCount: rowData.length,
    });
    // eslint-disable-next-line react-hooks/exhaustive-deps
  }, [rowData, setPagination]);

  const onGridReady = ({ api }) => {
    setGridApi(api);
  };
  const onPageChange = page => {
    setPagination({
      ...pagination,
      page,
    });
    gridApi?.paginationGoToPage(page - 1);
  };

  const getRowId = useMemo<GetRowIdFunc>(
    () =>
      ({ data }: GetRowIdParams) =>
        `${data.id === undefined ? data.agentId : data.id}${data.teamIds[0]}`,
    []
  );

  useEffect(() => {
    getAllAgents();
    // eslint-disable-next-line react-hooks/exhaustive-deps
  }, []);

  return (
    <Container>
      <StyledGrid
        isLoading={isLoading}
        defaultColDef={{
          cellStyle: () => ({
            display: "flex",
            alignItems: "center",
            justifyContent: "flex-start",
          }),
        }}
        rowHeight={DEFAULT_ROW_HEIGHT}
        rowData={rowData}
        columnDefs={columnDefs}
        getRowId={getRowId}
        onGridReady={onGridReady}
        paginationData={pagination}
        onPageChange={onPageChange}
        pagination
      />
    </Container>
  );
};

export default TagsGrid;

const Container = styled.div`
  height: 100%;
`;

const StyledGrid = styled(Grid)`
  .ag-cell-wrapper {
    width: 100%;
  }
`;
