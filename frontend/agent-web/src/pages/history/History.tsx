import { useCallback, useEffect, useState } from "react";
import { PageWrap } from "components/layout/AgentLayout";
import styled from "styled-components";
import PageTitle from "components/layout/PageTitle";
import Grid from "components/grid/Grid";
import columnDefs from "./ColsDef";
import agentApi from "services/api/agentApi";
import { IPagination } from "components/grid/Pagination";

const History = () => {
  const [rowData, setRowData] = useState([]);
  const [pagination, setPagination] = useState<IPagination>({ page: 1 });
  const updateData = page => {
    agentApi.getHistory({ page }).then(({ data }) => {
      setRowData(data.items);
      setPagination({
        page: data.page,
        pageSize: data.pageSize,
        totalCount: data.totalCount,
      });
    });
  };

  useEffect(() => {
    updateData(pagination.page);
  }, []);

  const onPageChange = useCallback(page => {
    updateData(page);
  }, []);

  return (
    <PageWrap>
      <PageTitle label="History" />
      <GridWrapper
        columnDefs={columnDefs}
        rowData={rowData}
        pagination
        paginationData={pagination}
        onPageChange={onPageChange}
      />
    </PageWrap>
  );
};

const GridWrapper = styled(Grid)`
  height: calc(100% - 100px);
`;

export default History;
