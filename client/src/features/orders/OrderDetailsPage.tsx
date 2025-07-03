import {
  Container,
  Typography,
  Box,
  Divider,
  Chip,
  IconButton,
  Tooltip,
  Card,
  List,
  ListItem,
  ListItemText,
  ListItemAvatar,
  Avatar,
  Stack
} from "@mui/material";
import { useTheme } from "@mui/material/styles";
import { useParams, useNavigate } from "react-router-dom";
import { useFetchOrderDetailedQuery } from "./orderApi";
import currencyFormat from "../../app/lib/util";
import { format } from "date-fns";
import {
  ArrowBack as ArrowBackIcon,
  LocationOn as LocationIcon,
  Payment as PaymentIcon,
  ShoppingCart as ShoppingCartIcon,
  Receipt as ReceiptIcon,
  LocalShipping,
  Info as InfoIcon
} from "@mui/icons-material";
import AppLoading from "../../app/shared/AppLoading";
import StatusBadge from '../../app/shared/StatusBadge';

export default function OrderDetailsPage() {
  const theme = useTheme();
  const { id } = useParams<{ id: string }>();
  const navigate = useNavigate();

  const { data: order, isLoading, error } = useFetchOrderDetailedQuery(Number(id));

  if (isLoading) return <AppLoading text="Loading order details" />;

  if (error || !order) {
    return (
      <Container maxWidth="md" sx={{ mt: 4 }}>
        <Box
          sx={{
            background: theme.palette.error.main,
            borderRadius: 4,
            p: 4,
            textAlign: 'center',
            color: theme.palette.getContrastText(theme.palette.error.main)
          }}
        >
          <Typography variant="h5" sx={{ fontWeight: 600 }}>
            Order not found
          </Typography>
        </Box>
      </Container>
    );
  }

  const getStatusColor = (status: string) => {
    switch (status.toLowerCase()) {
      case 'pending':
        return { bg: theme.palette.warning.light, color: theme.palette.warning.dark, icon: <InfoIcon sx={{ color: theme.palette.warning.dark, mr: 1 }} /> };
      case 'confirmed':
        return { bg: theme.palette.info.light, color: theme.palette.info.dark, icon: <InfoIcon sx={{ color: theme.palette.info.dark, mr: 1 }} /> };
      case 'shipped':
        return { bg: theme.palette.success.light, color: theme.palette.success.dark, icon: <LocalShipping sx={{ color: theme.palette.success.dark, mr: 1 }} /> };
      case 'delivered':
        return { bg: theme.palette.success.light, color: theme.palette.success.dark, icon: <LocalShipping sx={{ color: theme.palette.success.dark, mr: 1 }} /> };
      default:
        return { bg: theme.palette.secondary.light, color: theme.palette.secondary.dark, icon: <InfoIcon sx={{ color: theme.palette.secondary.dark, mr: 1 }} /> };
    }
  };

  const status = getStatusColor(order.orderStatus);

  return (
    <Container
      maxWidth="md"
      sx={{
        mt: 6,
        mb: 6,
        display: "flex",
        flexDirection: "column",
        alignItems: "center",
      }}
    >
      <Card
        sx={{
          width: "100%",
          maxWidth: 700,
          borderRadius: 5,
          boxShadow: "0 8px 32px rgba(0,0,0,0.10)",
          p: { xs: 1, sm: 2, md: 4 },
          background: theme.palette.background.paper,
          position: "relative",
        }}
      >
        {/* Status e Voltar */}
        <Box
          sx={{
            display: "flex",
            alignItems: "center",
            justifyContent: "space-between",
            mb: 2,
          }}
        >
          <Tooltip title="Back to Orders" arrow>
            <IconButton
              onClick={() => navigate("/orders")}
              sx={{
                background: theme.palette.action.hover,
                color: theme.palette.primary.main,
                "&:hover": {
                  background: theme.palette.action.selected,
                  color: theme.palette.primary.dark,
                },
                mr: 1,
              }}
            >
              <ArrowBackIcon />
            </IconButton>
          </Tooltip>
          <Typography
            variant="h5"
            sx={{
              fontWeight: 700,
              color: 'primary.main',
              flex: 1,
              textAlign: "center",
            }}
          >
            Order #{order.id}
          </Typography>
          <StatusBadge status={order.orderStatus} />
        </Box>

        <Divider sx={{ mb: 3 }} />

        {/* Seções lado a lado em telas médias+ e empilhadas no mobile */}
        <Stack direction={{ xs: 'column', sm: 'row' }} spacing={3}>
          {/* Coluna 1: Resumo e Endereço */}
          <Stack spacing={2} flex={1} minWidth={0}>
            <Box>
              <Box sx={{ display: "flex", alignItems: "center", mb: 1 }}>
                <ReceiptIcon sx={{ color: 'primary.main', mr: 1 }} />
                <Typography
                  variant="subtitle1"
                  sx={{ fontWeight: 600, color: 'primary.main' }}
                >
                  Order Summary
                </Typography>
              </Box>
              <Typography variant="body2" color="text.secondary">
                Order Date
              </Typography>
              <Typography variant="body1" sx={{ fontWeight: 600, color: 'text.primary' }}>
                {format(order.orderDate, "dd MMM yyyy 'at' HH:mm")}
              </Typography>
              <Typography
                variant="body2"
                color="text.secondary"
                sx={{ mt: 1 }}
              >
                Buyer Email
              </Typography>
              <Typography variant="body1" sx={{ fontWeight: 600, color: 'text.primary' }}>
                {order.buyerEmail}
              </Typography>
            </Box>
            <Box>
              <Box sx={{ display: "flex", alignItems: "center", mb: 1 }}>
                <LocationIcon sx={{ color: 'primary.main', mr: 1 }} />
                <Typography
                  variant="subtitle1"
                  sx={{ fontWeight: 600, color: 'primary.main' }}
                >
                  Shipping Address
                </Typography>
              </Box>
              <Typography variant="body1" sx={{ fontWeight: 600, mb: 0.5, color: 'text.primary' }}>
                {order.shippingAddress.name}
              </Typography>
              <Typography variant="body2" color="text.secondary">
                {order.shippingAddress.line1}
              </Typography>
              {order.shippingAddress.line2 && (
                <Typography variant="body2" color="text.secondary">
                  {order.shippingAddress.line2}
                </Typography>
              )}
              <Typography variant="body2" color="text.secondary">
                {order.shippingAddress.city}, {order.shippingAddress.state} {order.shippingAddress.postal_code}
              </Typography>
              <Typography variant="body2" color="text.secondary">
                {order.shippingAddress.country}
              </Typography>
            </Box>
          </Stack>

          {/* Coluna 2: Pagamento e Totais */}
          <Stack spacing={2} flex={1} minWidth={0}>
            <Box>
              <Box sx={{ display: "flex", alignItems: "center", mb: 1 }}>
                <PaymentIcon sx={{ color: 'primary.main', mr: 1 }} />
                <Typography
                  variant="subtitle1"
                  sx={{ fontWeight: 600, color: 'primary.main' }}
                >
                  Payment Method
                </Typography>
              </Box>
              <Typography variant="body1" sx={{ fontWeight: 600, mb: 0.5, color: 'text.primary' }}>
                {order.paymentSummary.brand} •••• {order.paymentSummary.last4}
              </Typography>
              <Typography variant="body2" color="text.secondary">
                Expires {order.paymentSummary.exp_month}/{order.paymentSummary.exp_year}
              </Typography>
            </Box>
            <Box>
              <Box sx={{ display: "flex", alignItems: "center", mb: 1 }}>
                <LocalShipping sx={{ color: 'primary.main', mr: 1 }} />
                <Typography
                  variant="subtitle1"
                  sx={{ fontWeight: 600, color: 'primary.main' }}
                >
                  Order Totals
                </Typography>
              </Box>
              <Box sx={{ display: "flex", justifyContent: "space-between", mb: 0.5 }}>
                <Typography variant="body2" color="text.secondary">
                  Subtotal
                </Typography>
                <Typography variant="body2" sx={{ color: 'text.primary' }}>
                  {currencyFormat(order.subtotal)}
                </Typography>
              </Box>
              <Box sx={{ display: "flex", justifyContent: "space-between", mb: 0.5 }}>
                <Typography variant="body2" color="text.secondary">
                  Delivery Fee
                </Typography>
                <Typography variant="body2" sx={{ color: 'text.primary' }}>
                  {currencyFormat(order.deliveryFee)}
                </Typography>
              </Box>
              {order.discount > 0 && (
                <Box sx={{ display: "flex", justifyContent: "space-between", mb: 0.5 }}>
                  <Typography variant="body2" color="text.secondary">
                    Discount
                  </Typography>
                  <Typography variant="body2" color="error.main">
                    -{currencyFormat(order.discount)}
                  </Typography>
                </Box>
              )}
              <Divider sx={{ my: 1 }} />
              <Box sx={{ display: "flex", justifyContent: "space-between" }}>
                <Typography variant="h6" sx={{ fontWeight: 700, color: 'primary.main' }}>
                  Total
                </Typography>
                <Typography variant="h6" sx={{ fontWeight: 700, color: 'primary.main' }}>
                  {currencyFormat(order.total)}
                </Typography>
              </Box>
            </Box>
          </Stack>
        </Stack>

        {/* Itens do Pedido */}
        <Box sx={{ mt: 4 }}>
          <Box sx={{ display: "flex", alignItems: "center", mb: 1 }}>
            <ShoppingCartIcon sx={{ color: 'primary.main', mr: 1 }} />
            <Typography
              variant="subtitle1"
              sx={{ fontWeight: 600, color: 'primary.main' }}
            >
              Order Items ({order.orderItems.length})
            </Typography>
          </Box>
          <List>
            {order.orderItems.map((item, index) => (
              <ListItem
                key={index}
                sx={{
                  px: 0,
                  mb: 1,
                  borderRadius: 2,
                  boxShadow: "0 1px 4px rgba(25,118,210,0.06)",
                  background: theme.palette.action.hover,
                }}
              >
                <ListItemAvatar>
                  <Avatar
                    src={item.pictureUrl}
                    variant="rounded"
                    sx={{ width: 64, height: 64, mr: 2 }}
                  />
                </ListItemAvatar>
                <ListItemText
                  primary={
                    <Typography variant="body1" sx={{ fontWeight: 600, color: 'text.primary' }}>
                      {item.name}
                    </Typography>
                  }
                  secondary={
                    <Typography variant="body2" color="text.secondary">
                      Quantity: {item.quantity} × {currencyFormat(item.price)}
                    </Typography>
                  }
                />
                <Typography
                  variant="body1"
                  sx={{
                    fontWeight: 700,
                    color: 'primary.main',
                    minWidth: 90,
                    textAlign: "right",
                    mr: 5,
                  }}
                >
                  {currencyFormat(item.price * item.quantity)}
                </Typography>
              </ListItem>
            ))}
          </List>
        </Box>
      </Card>
    </Container>
  );
} 