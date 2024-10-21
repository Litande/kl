import { css } from "styled-components";
import { colors } from "./theme/palette";

const styles = css`
  .ag-theme-alpine {
    --ag-background-color: white;
    --ag-secondary-foreground-color: ${colors.fg.secondary_light};

    --ag-data-color: ${colors.fg.secondary};
    --ag-header-background-color: ${colors.bg.ternary};
    --ag-header-cell-hover-background-color: ${colors.grid.headerHover};
    --ag-odd-row-background-color: ${colors.bg.secondary};
    --ag-row-hover-color: ${colors.grid.rowHover};
    --ag-border-color: ${colors.border.primary};
    --ag-row-border-color: ${colors.border.primary};
    --ag-selected-row-background-color: ${colors.grid.rowSelected};
    --ag-range-selection-border-color: transparent;

    &.fullRowSelectionMode {
      .ag-row-hover:not(.ag-full-width-row):before {
        background: transparent;
      }

      .ag-row:not(.ag-full-width-row) {
        border-bottom: none;
      }
    }

    .ag-header {
      .ag-header-cell {
        &[aria-sort="descending"],
        &[aria-sort="ascending"] {
          background: ${colors.grid.headerHover};
        }

        .ag-header-cell-label {
          justify-content: space-between;
          font-size: 12px;
          font-weight: 400;
          text-transform: uppercase;

          .icon-ic-sort {
            font-size: 13px;
          }

          .icon-ic-sortDesc,
          .icon-ic-sortAsc {
            position: relative;
            left: 0.5px;
            top: 0.5px;
            font-size: 14px;
          }

          .icon-ic-sortAsc {
            top: 0;
          }
        }
      }
    }

    .ag-cell {
      font-family: "Inter regular", serif;
    }

    .ag-row {
      .icon-ic-show-on-hover {
        opacity: 0;
        transition: opacity 0.5s;
      }

      &.ag-row-hover .icon-ic-show-on-hover {
        opacity: 1;
      }
    }

    .ag-row.ag-full-width-row {
      background-color: ${colors.grid.fullWidthRow};

      &.ag-row-selected:before {
        display: none;
      }
    }
  }
`;

export default styles;
