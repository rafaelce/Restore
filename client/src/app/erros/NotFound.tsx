import { SearchOff } from "@mui/icons-material";
import { Button, Paper, Typography, Container, Box } from "@mui/material";
import { Link } from "react-router-dom";
import ErrorOutlineIcon from "@mui/icons-material/ErrorOutline";

export default function NotFound() {
  return (
    <Container maxWidth="md">
      <Paper
        elevation={6}
        sx={{
          p: 6,
          display: "flex",
          flexDirection: "column",
          alignItems: "center",
          textAlign: "center",
          gap: 3,
          borderRadius: 3,
        }}
      >
        <SearchOff sx={{ fontSize: 120 }} color="primary" />
        <Typography variant="h3" color="text.primary">
          Page Not Found
        </Typography>
        <Typography variant="subtitle1" color="text.secondary">
          Oops — we couldn’t find what you were looking for.
        </Typography>
        <Box
          sx={{
            mt: 3,
            width: "100%",
            display: "flex",
            justifyContent: "center",
          }}
        >
          <Button
            variant="contained"
            size="large"
            component={Link}
            to="/catalog"
            startIcon={<ErrorOutlineIcon />}
          >
            Back to Shop
          </Button>
        </Box>
      </Paper>
    </Container>
  );
}
