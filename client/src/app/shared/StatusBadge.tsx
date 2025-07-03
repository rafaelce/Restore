import { Box, Typography } from "@mui/material";
import { FaClock, FaCheckCircle, FaTimesCircle } from "react-icons/fa";
import type { ReactNode } from "react";

interface StatusBadgeProps {
  status: string;
}

const statusMap: Record<string, { label: string; color: string; bg: string; icon: ReactNode }> = {
  pending: {
    label: "Pendente",
    color: "#ff9800",
    bg: "#fff4e5",
    icon: <FaClock />,
  },
  paymentreceived: {
    label: "Pago",
    color: "#219150",
    bg: "#c6f3dd",
    icon: <FaCheckCircle />,
  },
  paymentfailed: {
    label: "Falha no Pagamento",
    color: "#e74c3c",
    bg: "#ffeaea",
    icon: <FaTimesCircle />,
  },
  paymentmismatch: {
    label: "Valor Divergente",
    color: "#ba68c8",
    bg: "#f3e5f5",
    icon: <FaTimesCircle />,
  },
};

export default function StatusBadge({ status }: StatusBadgeProps) {
  const key = status.replace(/\s/g, "").toLowerCase();
  const { label, color, bg, icon } = statusMap[key] || {
    label: status,
    color: "#7b1fa2",
    bg: "#ede7f6",
    icon: <FaClock />,
  };
  return (
    <Box
      sx={{
        display: "inline-flex",
        alignItems: "center",
        gap: 1,
        px: 2,
        py: 0.5,
        borderRadius: 999,
        fontWeight: 600,
        fontSize: "1rem",
        background: bg,
        color,
        boxShadow: "0 2px 8px rgba(0,0,0,0.07)",
        letterSpacing: 0.02,
        userSelect: "none",
        minWidth: 0,
      }}
      aria-label={`Status: ${label}`}
    >
      <Box sx={{ display: "flex", alignItems: "center", fontSize: 20 }}>{icon}</Box>
      <Typography component="span" sx={{ fontWeight: 600, fontSize: "1rem", ml: 0.5 }}>
        {label}
      </Typography>
    </Box>
  );
} 