import { Box, CircularProgress, Typography } from "@mui/material";
import HourglassBottomIcon from "@mui/icons-material/HourglassBottom";

export default function AppLoading() {
  return (
    <Box
      display="flex"
      flexDirection="column"
      alignItems="center"
      justifyContent="center"
      height="60vh"
      textAlign="center"
      sx={{
        color: "text.secondary",
      }}
    >
      <HourglassBottomIcon
        sx={{ fontSize: 50, mb: 2, color: "text.disabled" }}
      />
      <Typography variant="h6" gutterBottom>
        Carregando dados...
      </Typography>
      <CircularProgress color="secondary" />
    </Box>
  );
}
