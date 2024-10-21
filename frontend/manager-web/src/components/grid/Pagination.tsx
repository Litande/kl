import React, { useEffect } from "react";
import { GridApi } from "ag-grid-community";

import styled from "styled-components";

export type IPagination = {
  page?: number;
  pageSize?: number;
  totalCount?: number;
};

type Props = {
  gridApi: GridApi;
  paginationData: IPagination;
  onPageChange?: (page: number) => void;
};

const Pagination = ({ gridApi, paginationData, onPageChange }: Props) => {
  const { pageSize, totalCount, page } = paginationData;
  const setPage = page => onPageChange && onPageChange(page);
  const showNext = () => onPageChange && onPageChange(page + 1);
  const showPrev = () => onPageChange && onPageChange(page - 1);

  const totalPages = totalCount ? Math.ceil(totalCount / pageSize) : 1;

  useEffect(() => {
    gridApi?.paginationGoToPage(page - 1);
  }, [page, gridApi]);

  if (totalPages === 1) {
    return (
      <PaginationContainer>
        <Item isActive>{page}</Item>
      </PaginationContainer>
    );
  }

  return (
    <PaginationContainer>
      {page !== 1 && <Button onClick={showPrev}>Previous</Button>}
      {page > totalPages + 2 && <Item onClick={() => setPage(page - 3)}>{page - 3}</Item>}
      {page > totalPages + 1 && <Item onClick={() => setPage(page - 2)}>{page - 2}</Item>}
      {page !== 1 && <Item onClick={() => setPage(page - 1)}>{page - 1}</Item>}
      <Item isActive>{page}</Item>
      {page !== totalPages && (
        <Item isHidden={page === totalPages} onClick={() => setPage(page + 1)}>
          {page + 1}
        </Item>
      )}
      {page + 2 <= totalPages && <Item onClick={() => setPage(page + 2)}>{page + 2}</Item>}
      {page + 2 < totalPages && <Dots>...</Dots>}
      {page + 2 < totalPages && <Item onClick={() => setPage(totalPages)}>{totalPages}</Item>}
      <Button isHidden={page === totalPages} onClick={showNext}>
        Next
      </Button>
    </PaginationContainer>
  );
};

export default Pagination;

export const PaginationContainer = styled.div`
  display: flex;
  flex-direction: row;
  justify-content: center;
  align-items: center;
  height: 60px;
  gap: 8px;
`;

export const Item = styled.div<{ isActive?: boolean; isHidden?: boolean }>`
  height: 32px;
  min-width: 32px;
  text-align: center;
  background: ${({ theme, isActive }) =>
    isActive ? theme.colors.btn.secondary : theme.colors.bg.ternary};
  color: ${({ theme, isActive }) =>
    isActive ? theme.colors.btn.primary : theme.colors.fg.secondary};
  border: 1px solid rgba(0, 0, 0, 0.12);
  border-radius: 4px;
  ${({ theme }) => theme.typography.buttonsText};
  visibility: ${({ isHidden }) => (isHidden ? "hidden" : "visible")};
  line-height: 32px;
  cursor: pointer;
`;

const Button = styled(Item)`
  width: 82px;
`;

export const Dots = styled.div`
  line-height: 32px;
  text-align: center;
`;
