import { Box, Button, Typography } from "@mui/material";
import { Link } from "react-router-dom";

export default function HomePage() {
  return (
    <Box
      sx={{
        maxWidth: "100%",
        mx: "auto",
        px: 2,
        position: "relative",
      }}
    >
      <img
        src="/images/hero1.jpg"
        alt="ski resort image"
        style={{
          position: "absolute",
          inset: 0,
          width: "100%",
          height: "100%",
          objectFit: "cover",
          borderRadius: "16px",
          zIndex: 0,
        }}
      />
      <Box
        sx={{
          display: "flex",
          flexDirection: "column",
          padding: 8,
          alignItems: "center",
          position: "relative",
          borderRadius: 4,
        }}
      >
        <Typography
          variant="h1"
          color="white"
          fontWeight="bold"
          textAlign="center"
          sx={{ my: 3 }}
        >
          Welcome to Restore!
        </Typography>
        <Button
          variant="contained"
          size="large"
          component={Link}
          to="/catalog"
          sx={{
            mt: 8,
            backgroundImage: "linear-gradient(to right, #2563eb, #06b6d4)",
            fontWeight: "bold",
            color: "white",
            borderRadius: "16px",
            px: 8,
            py: 2,
            border: "2px solid transparent",
          }}
        >
         Go to shop now
        </Button>
      </Box>
    </Box>
  );
}
