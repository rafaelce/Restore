import { Backdrop } from "@mui/material";
import { RotatingTriangles } from "react-loader-spinner";

export default function AppLoading() {
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
      <RotatingTriangles
        visible={true}
        height="200"
        width="200"
        ariaLabel="rotating-triangles-loading"
        wrapperStyle={{}}
        wrapperClass=""
        colors={["#00e5ff", "#00bcd4", "#1976d2"]}
      />
    </Backdrop>
  );
}
