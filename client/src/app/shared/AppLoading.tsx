import { Backdrop, Typography } from "@mui/material";
import { ScaleLoader } from "react-spinners";

export default function AppLoading({ text = "" }) {
  return (
    <Backdrop
      open={true}
      sx={{
        zIndex: (theme) => theme.zIndex.drawer + 1,
        backgroundColor: "rgba(0, 0, 0, 0.25)",
        backdropFilter: "blur(3px)",
        flexDirection: "column",
        color: "text.secondary",
      }}
    >
      {text && (
        <Typography variant="h6" sx={{ mb: 2 }}>
          {text}
        </Typography>
      )}
      <ScaleLoader
        color="#1976d2"
        height={80}
        width={8}
        radius={4}
        margin={4}
      />
    </Backdrop>
  );
}
