import { useEffect } from "react";
import Router from "router";
import { BrowserRouter } from "react-router-dom";
import { ToastContainer } from "react-toastify";
import styled, { ThemeProvider } from "styled-components";

import theme from "globalStyles/theme";
import AuthProvider from "data/user/AuthContext";
import { getRoutes } from "router/routerChecker";
import { timerService } from "services/timerService/TimerService";
import "react-toastify/dist/ReactToastify.css";

function App() {
  useEffect(() => {
    const timer = timerService();
    timer.startTimerService();
    return () => {
      timer.stopTimerService();
    };
  }, []);

  return (
    <ThemeProvider theme={theme}>
      <BrowserRouter>
        <StyledContainer />
        <AuthProvider>
          <Router routes={getRoutes()} />
        </AuthProvider>
      </BrowserRouter>
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
