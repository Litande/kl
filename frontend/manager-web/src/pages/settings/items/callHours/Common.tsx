import React from "react";
import styled from "styled-components";

export const save = props => {
  return (
    <svg
      width="24"
      height="24"
      viewBox="0 0 24 24"
      fill="none"
      xmlns="http://www.w3.org/2000/svg"
      {...props}
    >
      <path
        d="M17.67 2H2.8C2.36 2 2 2.36 2 2.8V21.2C2 21.64 2.36 22 2.8 22H21.2C21.64 22 22 21.64 22 21.2V6.33C22 6.12 21.92 5.91 21.77 5.76L18.24 2.23C18.09 2.08 17.89 2 17.67 2ZM16 7H6V3H16V7ZM21 21H3V3H5V7.5C5 7.78 5.22 8 5.5 8H16.5C16.78 8 17 7.78 17 7.5V3H17.59L21 6.41V21Z"
        fill="#5294C3"
      />
      <path
        d="M12 9C9.79 9 8 10.79 8 13C8 15.21 9.79 17 12 17C14.21 17 16 15.21 16 13C16 10.79 14.21 9 12 9ZM12 16C10.34 16 9 14.66 9 13C9 11.34 10.34 10 12 10C13.66 10 15 11.34 15 13C15 14.66 13.66 16 12 16Z"
        fill="#5294C3"
      />
    </svg>
  );
};
export const TableHeader = React.memo(props => (
  <thead {...props}>
    <TRow>
      <THeadData width="30px"></THeadData>
      <THeadData width="30%">Country</THeadData>
      <THeadData width="10%">From</THeadData>
      <THeadData width="12px"></THeadData>
      <THeadData width="10%">Till</THeadData>
      <THeadData width="8%"></THeadData>
      <THeadData width="10%">From</THeadData>
      <THeadData width="12px"></THeadData>
      <THeadData width="10%">Till</THeadData>
      <THeadData width="8%"></THeadData>
      <THeadData width="100px"></THeadData>
    </TRow>
  </thead>
));
TableHeader.displayName = "TableHeader";

export const TRow = styled.tr<{ isExpanded?: boolean; isEditing?: boolean }>`
  background-color: ${({ isExpanded }) => (isExpanded ? "#f3f8ff" : "inherit")};
  border-bottom: ${({ theme, isExpanded, isEditing }) =>
    isExpanded || isEditing ? "none" : `1px solid ${theme.colors.border.primary}`};

  td {
    padding: ${({ isEditing }) => (isEditing ? "16px 8px 8px" : "8px")};
  }
`;

export const THeadData = styled.th<{ width?: string }>`
  text-align: left;
  box-sizing: border-box;
  padding: 0 0.5rem 0.8rem;
  text-transform: uppercase;
  ${({ theme }) => theme.typography.smallText3};
  color: ${({ theme }) => theme.colors.fg.secondary_light};
`;

export const Divider = styled.div`
  width: 12px;
  height: 1px;
  background: black;
`;

export const TData = styled.td<{ isHidden?: boolean }>`
  padding: 8px;
  vertical-align: middle;
  visibility: ${({ isHidden }) => (isHidden ? "collapse" : "visible")};
`;

export const SaveIcon = styled(save)<{ disabled?: boolean }>`
  padding: 5px;
  border: 1px solid ${({ theme }) => theme.colors.border.secondary};
  border-radius: 4px;
  cursor: pointer;
  opacity: ${({ disabled }) => (disabled ? "0.4" : "1")};
`;
