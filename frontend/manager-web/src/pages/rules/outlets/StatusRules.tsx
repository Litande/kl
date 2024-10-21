import { useCallback, useMemo, useRef, useState } from "react";
import { useForm } from "react-hook-form";
import styled from "styled-components";
import { ColDef, GetRowIdFunc, GetRowIdParams } from "ag-grid-community";
import "ag-grid-community/styles/ag-grid.css";
import "ag-grid-community/styles/ag-theme-alpine.css";

import apiService from "services/api/apiService";
import Checkbox from "components/checkbox";
import Grid from "components/grid/Grid";
import { useSelector } from "react-redux";
import { isLoadingSelector } from "components/ruleEngine/groupSelector";
import LoadingOverlay from "components/loadingOverlay/LoadingOverlay";

const { fetchApi, putApi } = apiService();

type Statuses = Array<{ status: string; availableStatuses: Array<string> }>;

const StatusRules = () => {
  const isLoading = useSelector(isLoadingSelector);
  const { register } = useForm();
  const formWatchRef = useRef(null);
  const getRowId = useMemo<GetRowIdFunc>(
    () =>
      ({ data }: GetRowIdParams) =>
        data.value,
    []
  );
  // eslint-disable-next-line @typescript-eslint/no-explicit-any
  const [rowData, setRowData] = useState<any[]>(null);
  const [columnDefs, setColumnDefs] = useState<ColDef[]>([
    {
      headerName: "",
      field: "label",
      width: 170,
      pinned: "left",
      wrapText: true,
      lockPinned: true,
      cellStyle: {
        background: "#F4F9FF",
        lineHeight: "15px",
        fontSize: "12px",
        wordBreak: "break-word",
        display: "flex",
        alignItems: "center",
      },
    },
  ]);
  const defaultColDef = useMemo<ColDef>(() => {
    return {
      initialWidth: 75,
      height: 109,
      wrapHeaderText: true,
      autoHeaderHeight: true,
      suppressMovable: true,
    };
  }, []);

  const setData = (data, cellRendererParams) => {
    if (
      formWatchRef.current[data.value] &&
      Array.isArray(formWatchRef.current[data.value]) &&
      formWatchRef.current[data.value].length
    ) {
      if (formWatchRef.current[data.value].includes(cellRendererParams.value)) {
        formWatchRef.current[data.value] = formWatchRef.current[data.value].filter(
          opt => opt !== cellRendererParams.value
        );
      } else {
        formWatchRef.current[data.value].push(cellRendererParams.value);
      }
    } else {
      formWatchRef.current[data.value] = formWatchRef.current[data.value]
        ? [...formWatchRef.current[data.value], cellRendererParams.value]
        : [cellRendererParams.value];
    }
    const modifiedData = Object.entries(formWatchRef.current).reduce((acc, [key, value]) => {
      if (Array.isArray(value) && value.length) {
        acc.push({ status: key, availableStatuses: value });
      }
      return acc;
    }, []);
    putApi("/rules/status", modifiedData);
  };

  const onGridReady = useCallback(() => {
    Promise.all([fetchApi("/commons/leads/statuses"), fetchApi("/rules/status")])
      .then(([{ data }, activeStatuses]) => {
        const savedStatuses: Statuses = activeStatuses.data.reduce((acc, opt) => {
          acc[opt.status] = opt.availableStatuses;
          return acc;
        }, {});
        formWatchRef.current = savedStatuses;

        setRowData(data.map(el => ({ ...el, [el.label]: el.value })));
        setColumnDefs(prev => [
          ...prev,
          ...data.map(el => ({
            field: el.label,
            headerComponentParams: el,
            headerComponent: ({ label }) => <CustomHeader>{label}</CustomHeader>,
            cellRenderer: ({ register, data, colDef: { cellRendererParams } }) => {
              const { ref, name, onChange, ...restFormProps } = register(data.value);

              const isChecked =
                Array.isArray(formWatchRef.current[data.value]) &&
                formWatchRef.current[data.value].includes(cellRendererParams.value);
              const handleChange = e => {
                onChange(e);
                setData(data, cellRendererParams);
              };

              return (
                <Checkbox
                  inputRef={ref}
                  checked={isChecked}
                  value={cellRendererParams.value}
                  name={name}
                  onChange={handleChange}
                  {...restFormProps}
                />
              );
            },
            cellRendererParams: { register, ...el },
          })),
        ]);
      })
      .catch(e => console.log(e));
    // eslint-disable-next-line react-hooks/exhaustive-deps
  }, []);

  return (
    <Container>
      <Wrapper>
        <AgThemeAlpine>
          <StyledGrid
            rowHeight={50}
            rowData={rowData}
            columnDefs={columnDefs}
            defaultColDef={defaultColDef}
            onGridReady={onGridReady}
            getRowId={getRowId}
          ></StyledGrid>
        </AgThemeAlpine>
      </Wrapper>
      {isLoading && <LoadingOverlay />}
    </Container>
  );
};

export default StatusRules;

const Container = styled.div`
  position: relative;
  width: 100%;
  height: calc(70vh - 2.4rem);
`;

const CustomHeader = styled.div`
  ${({ theme }) => theme.typography.smallText3};
  color: rgba(0, 0, 0, 0.6);
  text-align: center;
`;

const AgThemeAlpine = styled.div`
  height: 100%;
  width: 100%;
  --ag-row-hover-color: #fff;
  --ag-odd-row-background-color: #fff;
  --ag-font-size: 12px;
  --ag-font-family: "Inter regular";
  --ag-range-selection-border-color: rgba(0, 0, 0, 0.12);
  --ag-range-selection-border-style: none;
  --ag-header-column-separator-display: block;
  --ag-header-column-separator-height: 100%;
  --ag-header-column-separator-width: 1px;
  --ag-header-column-separator-color: rgba(0, 0, 0, 0.12);
  --ag-header-background-color: #f3f8ff;
  --ag-cell-horizontal-border: 1px solid rgba(0, 0, 0, 0.12);

  --ag-secondary-border-row: 1px solid;
  --ag-borders-critical: 1px solid;
`;

const Wrapper = styled.div`
  display: flex;
  flex-direction: column;
  height: 100%;

  & .ag-header-cell {
    background: #f3f8ff;
    padding: 10px;
    text-align: center;
    border-right: 1px solid rgba(0, 0, 0, 0.12);
  }
`;

const StyledGrid = styled(Grid)`
  .ag-center-cols-container .ag-row .ag-cell {
    display: flex;
    justify-content: center;
    align-items: center;
    border-right: 1px solid rgba(0, 0, 0, 0.12);
  }

  .ag-pinned-left-header .ag-header-cell {
    background: #fff;
  }
`;
