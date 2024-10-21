import React, { useEffect, useMemo, useState, useCallback } from "react";
import styled from "styled-components";
import SearchForm, { IFormValues } from "./SearchForm";
import Grid from "components/grid/Grid";
import trackingApi from "services/api/tracking";
import columnDefs from "./columnsDefs";
import { GetRowIdFunc, GetRowIdParams, GridReadyEvent } from "ag-grid-community";
import { PageTitle } from "components/layout/Layout";
import { IPagination } from "components/grid/Pagination";
import useOptions from "hooks/useOptions";

const LeadsSearch = () => {
  const [formState, setFormState] = useState({});
  const [isLoading, setIsLoading] = useState(true);
  const [rowData, setRowData] = useState(null);
  const [pagination, setPagination] = useState<IPagination>({ page: 1, pageSize: 10 });

  const options = useOptions({ withCountries: true, withAgents: true, withStatuses: true });

  const fetchLeads = (formData, params) => {
    trackingApi
      .getLeads(formData, { params })
      .then(({ data }) => {
        setRowData([...data.items]);
        setPagination({
          page: data.page,
          pageSize: data.pageSize,
          totalCount: data.totalCount,
        });
      })
      .finally(() => {
        setIsLoading(false);
      });
  };

  const handleFormReset = () => {
    setIsLoading(true);
    setFormState({});
    fetchLeads({}, { page: 1 });
  };

  const handleFormSubmit = (data: IFormValues) => {
    const totalCalls = data.totalCalls.length
      ? {
          operation: "Equal",
          value: parseInt(data.totalCalls),
        }
      : null;

    setIsLoading(true);
    const params = {
      ...data,
      totalCalls,
      brand: "",
      leadStatus: data.leadStatus.map(item => item.value),
      assignedAgent: data.assignedAgent.map(item => item.value),
      country: data.country.map(item => item.value),
    };
    setFormState(params);
    fetchLeads(params, { page: 1 });
  };

  useEffect(() => {
    trackingApi
      .getLeads()
      .then(({ data }) => {
        setRowData(data.items);
        setPagination({
          page: data.page,
          pageSize: data.pageSize,
          totalCount: data.totalCount,
        });
      })
      .finally(() => {
        setIsLoading(false);
      });
  }, []);

  const onGridReady = (params: GridReadyEvent) => {
    params.api.sizeColumnsToFit();
  };

  const onPageChange = useCallback(
    page => {
      fetchLeads(formState, { page });
    },
    [formState]
  );

  const getRowId = useMemo<GetRowIdFunc>(() => (params: GetRowIdParams) => params.data.leadId, []);

  return (
    <Container>
      <PageName>Lead Search</PageName>
      <SearchForm options={options} onSubmit={handleFormSubmit} onReset={handleFormReset} />
      <GridContainer>
        <Grid
          context={{ leadOptions: options }}
          rowData={rowData}
          columnDefs={columnDefs}
          onGridReady={onGridReady}
          getRowId={getRowId}
          isLoading={isLoading}
          pagination
          paginationData={pagination}
          onPageChange={onPageChange}
        ></Grid>
      </GridContainer>
    </Container>
  );
};

export default LeadsSearch;

const Container = styled.div`
  height: 100%;
  display: flex;
  flex-direction: column;
  overflow: hidden;
`;

const GridContainer = styled.div`
  width: 100%;
  height: 100%;
  overflow-y: auto;
`;

const PageName = styled(PageTitle)`
  box-sizing: border-box;
  height: 72px;
  padding: 16px 0;
  margin: 0;
  border-bottom: 1px solid ${({ theme }) => theme.colors.border.primary};
`;
