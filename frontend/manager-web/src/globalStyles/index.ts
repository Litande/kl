import { createGlobalStyle } from "styled-components";
import gridStyles from "./grid";
import reset from "./reset";
import fontsStyles from "./theme/fonts";
import themeButtons from "./theme/buttons";
import scrollbar from "./scrollbar";

import "icons/icomoon/style.css";

import "ag-grid-community/styles/ag-grid.css";
import "ag-grid-community/styles/ag-theme-alpine.css";

import "react-tooltip/dist/react-tooltip.css";
import "react-calendar/dist/Calendar.css";

import themeInputs from "./theme/inputs";

const GlobalStyles = createGlobalStyle`
  body {
    overflow-y: hidden;
  }
	
	${gridStyles}	
	${reset}	
	${fontsStyles}
	${themeButtons}
	${themeInputs}
	${scrollbar}
`;

export default GlobalStyles;
