import { CONNECTION_STATUS } from "../types";
import { FillMapByStatus } from "../consts";

type ComponentProps = {
  status: CONNECTION_STATUS;
};

const IconConnections = ({ status }: ComponentProps) => {
  return (
    <svg width="16" height="16" viewBox="0 0 16 16" fill="none" xmlns="http://www.w3.org/2000/svg">
      <path
        d="M14.6998 4L13.9898 4.71C13.7698 4.46 13.5398 4.23 13.2898 4.01C11.8798 2.76 10.0198 2 7.9998 2C5.9798 2 4.1198 2.76 2.7098 4.01C2.4598 4.23 2.2298 4.46 2.0098 4.71L1.2998 4C1.5198 3.75 1.7498 3.52 1.9998 3.3C3.5898 1.87 5.6998 1 7.9998 1C10.2998 1 12.4098 1.87 13.9998 3.3C14.2498 3.52 14.4798 3.75 14.6998 4Z"
        fill={FillMapByStatus[status].line1.fill}
        fillOpacity={FillMapByStatus[status].line1.opacity}
      />
      <path
        d="M12.5799 6.12L11.8699 6.83C11.6599 6.58 11.4199 6.34 11.1699 6.13C10.3099 5.43 9.19992 5 7.99992 5C6.79992 5 5.68992 5.43 4.82992 6.13C4.57992 6.34 4.33992 6.58 4.12992 6.83L3.41992 6.12C3.63992 5.87 3.86992 5.64 4.11992 5.42C5.15992 4.53 6.51992 4 7.99992 4C9.47992 4 10.8399 4.53 11.8799 5.42C12.1299 5.64 12.3599 5.87 12.5799 6.12Z"
        fill={FillMapByStatus[status].line2.fill}
        fillOpacity={FillMapByStatus[status].line2.opacity}
      />
      <path
        d="M10.4401 8.26L9.72006 8.98C9.55006 8.69 9.31006 8.45 9.02006 8.28C8.72006 8.1 8.37006 8 8.00006 8C7.63006 8 7.28006 8.1 6.98006 8.28C6.69006 8.45 6.45006 8.69 6.28006 8.98L5.56006 8.26C5.75006 7.99 5.99006 7.75 6.26006 7.56C6.75006 7.21 7.35006 7 8.00006 7C8.65006 7 9.25006 7.21 9.74006 7.56C10.0101 7.75 10.2501 7.99 10.4401 8.26Z"
        fill={FillMapByStatus[status].line3.fill}
        fillOpacity={FillMapByStatus[status].line3.opacity}
      />
      {FillMapByStatus[status]?.noConnect && (
        <path
          d="M5.63819 11.7096L9.64041 15.7118L10.3475 15.0047L6.34529 11.0025L5.63819 11.7096Z"
          fill="#C80048"
        />
      )}
      {FillMapByStatus[status]?.noConnect && (
        <path
          d="M9.63992 10.998L5.6377 15.0002L6.3448 15.7074L10.347 11.7051L9.63992 10.998Z"
          fill="#C80048"
        />
      )}
      <path
        d="M10 13V14H11V13H10Z"
        fill={FillMapByStatus[status].dotLine.fill}
        fillOpacity={FillMapByStatus[status].dotLine.opacity}
      />
      <path
        d="M12 13V14H13V13H12Z"
        fill={FillMapByStatus[status].dotLine.fill}
        fillOpacity={FillMapByStatus[status].dotLine.opacity}
      />
      <path
        d="M14 13V14H15V13H14Z"
        fill={FillMapByStatus[status].dotLine.fill}
        fillOpacity={FillMapByStatus[status].dotLine.opacity}
      />
      <path
        d="M3 13V14H4V13H3Z"
        fill={FillMapByStatus[status].dotLine.fill}
        fillOpacity={FillMapByStatus[status].dotLine.opacity}
      />
      <path
        d="M5 13V14H6V13H5Z"
        fill={FillMapByStatus[status].dotLine.fill}
        fillOpacity={FillMapByStatus[status].dotLine.opacity}
      />
      <path
        d="M1 13V14H2V13H1Z"
        fill={FillMapByStatus[status].dotLine.fill}
        fillOpacity={FillMapByStatus[status].dotLine.opacity}
      />
      <path
        d="M8 9C7.44772 9 7 9.44772 7 10C7 10.5523 7.44772 11 8 11C8.55228 11 9 10.5523 9 10C9 9.44772 8.55228 9 8 9Z"
        fill={FillMapByStatus[status].line4.fill}
        fillOpacity={FillMapByStatus[status].line4.opacity}
      />
    </svg>
  );
};

export default IconConnections;
