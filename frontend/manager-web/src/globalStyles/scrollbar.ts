import { css } from "styled-components";
import { colors } from "./theme/palette";

const scrollbar = css`
  /* width */
  ::-webkit-scrollbar {
    width: 24px;
    height: 24px;
  }

  /* Track */
  ::-webkit-scrollbar-track {
    background: ${colors.bg.ternary};
  }

  /* Handle */
  ::-webkit-scrollbar-thumb {
    border: 8px solid transparent;
    border-radius: 28px;
    background-color: rgba(0, 0, 0, 0.12);
    background-clip: content-box;
  }

  /* Handle on hover */
  ::-webkit-scrollbar-thumb:hover {
    background-color: rgba(0, 0, 0, 0.3);
  }

  /* Buttons */
  ::-webkit-scrollbar-button {
    display: block;
    background-color: ${colors.bg.ternary};
    background-repeat: no-repeat;
    background-size: 50%;
    background-position: center;
  }

  ::-webkit-scrollbar-button:vertical:start:increment,
  ::-webkit-scrollbar-button:horizontal:start:increment {
    display: none;
  }

  ::-webkit-scrollbar-button:vertical:start:decrement {
    background-image: url('data:image/svg+xml, <svg width="12" height="8" viewBox="0 0 12 8" fill="none" xmlns="http://www.w3.org/2000/svg"><path d="M6 3.05176e-05L12 7.99988L0 8.00003L6 3.05176e-05Z" fill="black" fill-opacity="0.12"/></svg>');
    &:hover {
      background-image: url('data:image/svg+xml, <svg width="12" height="8" viewBox="0 0 12 8" fill="none" xmlns="http://www.w3.org/2000/svg"><path d="M6 3.05176e-05L12 7.99988L0 8.00003L6 3.05176e-05Z" fill="black" fill-opacity="0.3"/></svg>');
    }
  }

  ::-webkit-scrollbar-button:horizontal:start:decrement {
    background-image: url('data:image/svg+xml, <svg width="8" height="13" viewBox="0 0 8 13" fill="none" xmlns="http://www.w3.org/2000/svg"><path d="M0 6.00391L7.99985 0.00390625L8 12.0039L0 6.00391Z" fill="black" fill-opacity="0.12"/></svg>');
    &:hover {
      background-image: url('data:image/svg+xml, <svg width="8" height="13" viewBox="0 0 8 13" fill="none" xmlns="http://www.w3.org/2000/svg"><path d="M0 6.00391L7.99985 0.00390625L8 12.0039L0 6.00391Z" fill="black" fill-opacity="0.3"/></svg>');
    }
  }

  ::-webkit-scrollbar-button:vertical:end:increment {
    background-image: url('data:image/svg+xml,<svg width="12" height="8" viewBox="0 0 12 8" fill="none" xmlns="http://www.w3.org/2000/svg"><path d="M6 8L3.56041e-06 -2.95642e-06L12 -9.53674e-07L6 8Z" fill="black" fill-opacity="0.12"/></svg>');
    &:hover {
      background-image: url('data:image/svg+xml,<svg width="12" height="8" viewBox="0 0 12 8" fill="none" xmlns="http://www.w3.org/2000/svg"><path d="M6 8L3.56041e-06 -2.95642e-06L12 -9.53674e-07L6 8Z" fill="black" fill-opacity="0.3"/></svg>');
    }
  }

  ::-webkit-scrollbar-button:horizontal:end:increment {
    background-image: url('data:image/svg+xml,<svg width="8" height="13" viewBox="0 0 8 13" fill="none" xmlns="http://www.w3.org/2000/svg"><path d="M8 6.00391L-2.95642e-06 12.0039L-9.53674e-07 0.00390555L8 6.00391Z" fill="black" fill-opacity="0.12"/></svg>');
    &:hover {
      background-image: url('data:image/svg+xml,<svg width="8" height="13" viewBox="0 0 8 13" fill="none" xmlns="http://www.w3.org/2000/svg"><path d="M8 6.00391L-2.95642e-06 12.0039L-9.53674e-07 0.00390555L8 6.00391Z" fill="black" fill-opacity="0.3"/></svg>');
    }
  }

  ::-webkit-scrollbar-button:vertical:end:decrement,
  ::-webkit-scrollbar-button:horizontal:end:decrement {
    display: none;
  }
`;

export default scrollbar;
