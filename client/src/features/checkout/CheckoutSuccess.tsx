import {
  Box,
  Button,
  Container,
  Divider,
  Paper,
  Typography,
  Stack,
  useTheme,
} from "@mui/material";
import { Link, useLocation } from "react-router-dom";
import type { Order } from "../../app/models/order";
import currencyFormat, { formatAddressString, formatPaymentString } from "../../app/lib/util";
import CheckCircleOutlineIcon from "@mui/icons-material/CheckCircleOutline";
import CreditCardIcon from "@mui/icons-material/CreditCard";
import LocalShippingIcon from "@mui/icons-material/LocalShipping";
import CalendarMonthIcon from "@mui/icons-material/CalendarMonth";
import AttachMoneyIcon from "@mui/icons-material/AttachMoney";
import { useEffect } from "react";

// Confetti effect (simple, lightweight)
function ConfettiCelebration() {
  useEffect(() => {
    import("canvas-confetti").then((confetti) => {
      confetti.default({
        particleCount: 120,
        spread: 80,
        origin: { y: 0.6 },
      });
    });
  }, []);
  return null;
}

export default function CheckoutSuccess() {
  const { state } = useLocation();
  const order = state?.data as Order;
  const theme = useTheme();

  if (!order) return <Typography>Problem accessing the order</Typography>;

  return (
    <Box
      sx={{
        bgcolor: "linear-gradient(135deg, #1e3c72 0%, #2a5298 100%)",
        display: "flex",
        alignItems: "center",
        justifyContent: "center",
      }}
    >
      <ConfettiCelebration />
      <Container maxWidth="sm" sx={{ p: 0 }}>
        <Paper
          elevation={8}
          sx={{
            p: { xs: 3, sm: 5 },
            borderRadius: 6,
            bgcolor: "rgba(30, 32, 40, 0.98)",
            color: "#fff",
            boxShadow: "0 8px 32px 0 rgba(30,60,114,0.25)",
            textAlign: "center",
            maxWidth: 500,
            mx: "auto",
            animation: "fadeInUp 0.7s cubic-bezier(.39,.575,.56,1.000) both",
            "@keyframes fadeInUp": {
              "0%": { opacity: 0, transform: "translateY(40px)" },
              "100%": { opacity: 1, transform: "translateY(0)" },
            },
          }}
        >
          <CheckCircleOutlineIcon
            sx={{ fontSize: 80, color: "success.main", mb: 1, mt: -2 }}
          />
          <Typography variant="h4" fontWeight="bold" gutterBottom>
            Pedido Confirmado!
          </Typography>
          <Typography variant="subtitle1" color="grey.300" gutterBottom>
            Obrigado por sua compra. Seu pedido <b>#{order.id}</b> foi
            processado com sucesso.
          </Typography>

          <Divider sx={{ my: 3, bgcolor: "grey.800" }} />

          <Stack spacing={2} sx={{ textAlign: "left", mb: 2 }}>
            <Box
              display="flex"
              alignItems="center"
              justifyContent="space-between"
            >
              <Box display="flex" alignItems="center" gap={1}>
                <CalendarMonthIcon fontSize="small" />
                <Typography variant="body2" color="grey.400">
                  Data do Pedido
                </Typography>
              </Box>
              <Typography variant="body2" fontWeight="bold">
                {new Date(order.orderDate).toLocaleString("pt-BR")}
              </Typography>
            </Box>
            <Box
              display="flex"
              alignItems="center"
              justifyContent="space-between"
            >
              <Box display="flex" alignItems="center" gap={1}>
                <CreditCardIcon fontSize="small" />
                <Typography variant="body2" color="grey.400">
                  Pagamento
                </Typography>
              </Box>
              <Typography variant="body2" fontWeight="bold">
                {formatPaymentString(order.paymentSummary)}
              </Typography>
            </Box>
            <Box
              display="flex"
              alignItems="center"
              justifyContent="space-between"
            >
              <Box display="flex" alignItems="center" gap={1}>
                <LocalShippingIcon fontSize="small" />
                <Typography variant="body2" color="grey.400">
                  Entrega
                </Typography>
              </Box>
              <Typography
                variant="body2"
                fontWeight="bold"
                sx={{ textAlign: "right" }}
              >
                {formatAddressString(order.shippingAddress)}
              </Typography>
            </Box>
            <Box
              display="flex"
              alignItems="center"
              justifyContent="space-between"
            >
              <Box display="flex" alignItems="center" gap={1}>
                <AttachMoneyIcon fontSize="small" />
                <Typography variant="body2" color="grey.400">
                  Total
                </Typography>
              </Box>
              <Typography
                variant="body2"
                fontWeight="bold"
                color="success.light"
              >
                {currencyFormat(order.total)}
              </Typography>
            </Box>
          </Stack>

          <Divider sx={{ my: 3, bgcolor: "grey.800" }} />

          <Stack
            direction={{ xs: "column", sm: "row" }}
            spacing={2}
            justifyContent="center"
          >
            <Button
              variant="contained"
              color="success"
              component={Link}
              to={`/orders/${order.id}`}
              sx={{
                fontWeight: "bold",
                minWidth: 180,
                boxShadow: "0 2px 8px 0 rgba(76,175,80,0.15)",
                transition: "background 0.2s",
                "&:hover": {
                  background: theme.palette.success.dark,
                  color: "#fff",
                },
              }}
            >
              Ver detalhes do pedido
            </Button>
            <Button
              component={Link}
              to="/catalog"
              variant="outlined"
              color="inherit"
              sx={{
                fontWeight: "bold",
                minWidth: 180,
                borderColor: "grey.700",
                background: "rgba(255,255,255,0.02)",
                transition: "background 0.2s, color 0.2s",
                "&:hover": {
                  background: "rgba(255,255,255,0.08)",
                  color: theme.palette.success.main,
                  borderColor: theme.palette.success.main,
                },
              }}
            >
              Continuar comprando
            </Button>
          </Stack>
        </Paper>
      </Container>
    </Box>
  );
}
