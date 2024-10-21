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
    --ag-header-column-separator-display: block;
    --ag-header-column-separator-height: 100%;
    --ag-header-column-separator-width: 1px;
    --ag-header-column-separator-color: ${colors.border.primary};
    --ag-icon-font-code-checkbox-unchecked: "\\e915";
    --ag-icon-font-code-checkbox-checked: "\\e916";
    --ag-icon-font-code-checkbox-indeterminate: "\\e915";
    --ag-checkbox-checked-color: ${colors.grid.checkbox};
    --ag-checkbox-unchecked-color: ${colors.grid.checkbox};
    --ag-checkbox-indeterminate-color: ${colors.grid.checkbox};

    .ag-checkbox-input-wrapper {
      font-family: "icomoon", serif;
      box-shadow: none;
      background: transparent;
      font-size: 24px;
    }

    &.blacklist {
      .ag-cell-focus:not(.ag-cell-range-selected):focus-within {
        outline: none;
        border: none;
      }

      .ag-root-wrapper {
        border: none;
      }

      .ag-center-cols-viewport {
        background: ${colors.bg.light};
      }

      .ag-cell {
        border: none;
        color: ${colors.fg.secondary_light};
      }

      .ag-cell:after {
        display: block;
        content: "";
        position: absolute;
        top: 7px;
        bottom: 7px;
        right: 0;
        width: 1px;
        background: ${colors.border.primary};
      }

      .ag-cell:last-child:after,
      .ag-header-cell:last-child:after {
        display: none;
      }
    }

    .ag-root-wrapper {
      border-radius: 4px;
    }

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

          .icon-sorting {
            font-size: 13px;
          }

          .icon-sorting-up,
          .icon-sorting-down {
            position: relative;
            left: 0.5px;
            top: 0.5px;
            font-size: 14px;
          }

          .icon-sorting-down {
            top: 0;
          }
        }
      }
    }

    .ag-cell {
      font-family: "Inter light", serif;
    }

    .ag-row {
      .icon-show-on-hover {
        opacity: 0;
        transition: opacity 0.5s;
      }

      &.ag-row-hover .icon-show-on-hover {
        opacity: 1;
      }
    }

    .ag-row.ag-full-width-row {
      background-color: ${colors.grid.fullWidthRow};

      &.ag-row-selected:before {
        display: none;
      }
    }

    .ag-grid-loader {
      box-sizing: border-box;
      display: inline-block;
      width: 6rem;
      height: 6rem;
      animation: rotation 2s linear infinite;
      @keyframes rotation {
        0% {
          transform: rotate(0deg);
        }
        100% {
          transform: rotate(360deg);
        }
      }
    }
  }
`;

export default styles;
