import { BrowserRouter } from "react-router-dom";
import { ThemeProvider } from "styled-components";
import { Provider } from "react-redux";
import { ToastContainer } from "react-toastify";
import styled from "styled-components";
import "react-toastify/dist/ReactToastify.css";

import theme from "globalStyles/theme";

import Router from "router";
import store from "store";

import { getRoutes } from "router/routerChecker";
import AuthProvider from "data/user/AuthContext";

function App() {
  return (
    <ThemeProvider theme={theme}>
      <Provider store={store}>
        <BrowserRouter>
          <StyledContainer />
          <AuthProvider>
            <Router routes={getRoutes(window.location.hostname)} />
          </AuthProvider>
        </BrowserRouter>
      </Provider>
    </ThemeProvider>
  );
}

export default App;

const StyledContainer = styled(ToastContainer)`
  // https://styled-components.com/docs/faqs#how-can-i-override-styles-with-higher-specificity
  &&&.Toastify__toast-container {
  }
  .Toastify__toast {
    width: 325px;
  }
  .Toastify__toast-body {
    ${theme.typography.body1}
  }
  .Toastify__close-button {
    min-width: min-content;
  }
`;
