import { useEffect } from "react";
import { useNavigate, Outlet, ScrollRestoration } from "react-router-dom";
import {
  Box,
  Container,
  createTheme,
  CssBaseline,
  ThemeProvider,
} from "@mui/material";
import { ApolloProvider } from '@apollo/client';
import { apolloClient } from '../api/apolloClient';
import NavBar from "./NavBar";
import { useAppSelector } from "../store/store";
import { setNavigate } from "../routes/router";

function App() {
  const navigate = useNavigate();

  useEffect(() => {
    setNavigate(navigate);
  }, [navigate]);

  const { darkMode } = useAppSelector((state) => state.ui);
  const palleteType = darkMode ? "dark" : "light";

  const theme = createTheme({
    palette: {
      mode: palleteType,
      background: {
        default: palleteType === "light" ? "#eaeaea" : "#121212",
      },
    },
  });

  return (
    <ApolloProvider client={apolloClient}>
      <ThemeProvider theme={theme}>
        <ScrollRestoration />
        <CssBaseline />
        <NavBar />
        <Box
          sx={{
            minHeight: "100vh",
            background: darkMode
              ? "radial-gradient(circle, #1e3aBa, #111B27)"
              : "radial-gradient(circle, #baecf9, #f0f9ff)",
            py: 6,
          }}
        >
          <Container maxWidth="xl" sx={{ marginTop: 8 }}>
            <Outlet />
          </Container>
        </Box>
      </ThemeProvider>
    </ApolloProvider>
  );
}

export default App;
