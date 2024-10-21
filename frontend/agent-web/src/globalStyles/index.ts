import { createGlobalStyle } from "styled-components";
import gridStyles from "./grid";
import reset from "./reset";
import fontsStyles from "./theme/fonts";
import themeButtons from "./theme/buttons";

import "icons/icomoon/style.css";

import "react-calendar/dist/Calendar.css";

import "ag-grid-community/styles/ag-grid.css";
import "ag-grid-community/styles/ag-theme-alpine.css";

import themeInputs from "./theme/inputs";

const GlobalStyles = createGlobalStyle`
	${gridStyles}	
	${reset}	
	${fontsStyles}
	${themeButtons}
	${themeInputs}
	
`;

export default GlobalStyles;
