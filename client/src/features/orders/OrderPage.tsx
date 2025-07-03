import { 
  Container, 
  Table, 
  TableBody, 
  TableCell, 
  TableHead, 
  TableRow, 
  Typography, 
  Box,
  Chip,
  IconButton,
  Tooltip,
  Fade,
  Slide
} from "@mui/material";
import { useFetchOrdersQuery } from "./orderApi";
import { useNavigate } from "react-router-dom";
import currencyFormat from "../../app/lib/util";
import { format } from "date-fns";
import { 
  Visibility as VisibilityIcon,
  Receipt as ReceiptIcon,
  TrendingUp as TrendingUpIcon
} from "@mui/icons-material";
import { useState, useEffect } from "react";
import AppLoading from "../../app/shared/AppLoading";
import StatusBadge from '../../app/shared/StatusBadge';

export default function OrdersPage() {
  const { data: orders, isLoading } = useFetchOrdersQuery();
  const navigate = useNavigate();
  const [mounted, setMounted] = useState(false);

  useEffect(() => {
    setMounted(true);
  }, []);

  if (isLoading) return <AppLoading text="Loading orders" />

  if (!orders) return <Typography variant="h5" sx={{ fontWeight: 600 }}> No orders available </Typography>

  const getStatusColor = (status: string) => {
    switch (status.toLowerCase()) {
      case 'pending':
        return { bg: '#fff3e0', color: '#e65100' };
      case 'confirmed':
        return { bg: '#e3f2fd', color: '#1565c0' };
      case 'shipped':
        return { bg: '#fff8e1', color: '#f57c00' };
      case 'delivered':
        return { bg: '#e8f5e8', color: '#2e7d32' };
      default:
        return { bg: '#f3e5f5', color: '#7b1fa2' };
    }
  };

  return (
    <Container maxWidth="lg" sx={{ mt: 4, mb: 4 }}>
      <Slide direction="down" in={mounted} timeout={800}>
        <Box>
          
          <Box
            sx={{
              background: '#ffffff',
              borderRadius: 4,
              overflow: 'hidden',
              boxShadow: '0 4px 12px rgba(0,0,0,0.1)',
              border: '1px solid #e0e0e0'
            }}
          >
            <Box
              sx={{
                background: '#1976d2',
                p: 3,
                display: 'flex',
                alignItems: 'center',
                gap: 2
              }}
            >
              <ReceiptIcon sx={{ color: 'white', fontSize: 32 }} />
              <Typography variant="h6" sx={{ color: 'white', fontWeight: 600 }}>
                Order History
              </Typography>
              <Box sx={{ flexGrow: 1 }} />
              <Chip 
                label={`${orders.length} orders`}
                sx={{ 
                  background: 'rgba(255,255,255,0.2)',
                  color: 'white',
                  fontWeight: 600
                }}
              />
            </Box>

            <Table sx={{ minWidth: 650 }}>
              <TableHead>
                <TableRow sx={{ background: '#f5f5f5' }}>
                  <TableCell 
                    align="center"
                    sx={{ 
                      fontWeight: 700, 
                      fontSize: '1.1rem',
                      color: '#495057',
                      borderBottom: '2px solid #dee2e6'
                    }}
                  >
                    Order ID
                  </TableCell>
                  <TableCell 
                    sx={{ 
                      fontWeight: 700, 
                      fontSize: '1.1rem',
                      color: '#495057',
                      borderBottom: '2px solid #dee2e6'
                    }}
                  >
                    Date
                  </TableCell>
                  <TableCell 
                    sx={{ 
                      fontWeight: 700, 
                      fontSize: '1.1rem',
                      color: '#495057',
                      borderBottom: '2px solid #dee2e6'
                    }}
                  >
                    Total
                  </TableCell>
                  <TableCell 
                    sx={{ 
                      fontWeight: 700, 
                      fontSize: '1.1rem',
                      color: '#495057',
                      borderBottom: '2px solid #dee2e6'
                    }}
                  >
                    Status
                  </TableCell>
                  <TableCell 
                    align="center"
                    sx={{ 
                      fontWeight: 700, 
                      fontSize: '1.1rem',
                      color: '#495057',
                      borderBottom: '2px solid #dee2e6'
                    }}
                                      >
                      Details
                    </TableCell>
                  </TableRow>
                </TableHead>
              <TableBody>
                {orders.map((order, index) => (
                  <Fade in={mounted} timeout={1000 + index * 200} key={order.id}>
                    <TableRow
                      hover
                      sx={{
                        cursor: 'pointer',
                        transition: 'all 0.3s ease',
                        '&:hover': {
                          background: '#f8f9fa',
                          transform: 'translateY(-1px)',
                          boxShadow: '0 2px 8px rgba(0,0,0,0.1)'
                        },
                        '&:nth-of-type(even)': {
                          background: 'rgba(248, 249, 250, 0.5)'
                        }
                      }}
                    >
                      <TableCell 
                        align="center"
                        sx={{ 
                          fontWeight: 600,
                          fontSize: '1.1rem',
                          color: '#495057'
                        }}
                      >
                        <Box
                          sx={{
                            background: '#1976d2',
                            color: 'white',
                            borderRadius: 2,
                            px: 2,
                            py: 1,
                            display: 'inline-block',
                            fontWeight: 700,
                            boxShadow: '0 2px 4px rgba(25, 118, 210, 0.3)'
                          }}
                        >
                          #{order.id}
                        </Box>
                      </TableCell>
                      <TableCell sx={{ fontSize: '1rem', color: '#6c757d' }}>
                        <Box sx={{ display: 'flex', alignItems: 'center', gap: 1 }}>
                          <TrendingUpIcon sx={{ color: '#28a745', fontSize: 20 }} />
                          {format(order.orderDate, "dd MMM yyyy")}
                        </Box>
                      </TableCell>
                      <TableCell sx={{ fontSize: '1.1rem', fontWeight: 600, color: '#28a745' }}>
                        {currencyFormat(order.total)}
                      </TableCell>
                      <TableCell>
                        <StatusBadge status={order.orderStatus} />
                      </TableCell>
                      <TableCell align="center">
                        <Tooltip title="View Order Details" arrow>
                          <IconButton
                            onClick={() => navigate(`/orders/${order.id}`)}
                            sx={{
                              background: '#1976d2',
                              color: 'white',
                              '&:hover': {
                                background: '#1565c0',
                                transform: 'scale(1.05)',
                                boxShadow: '0 2px 8px rgba(25, 118, 210, 0.4)'
                              },
                              transition: 'all 0.3s ease'
                            }}
                          >
                            <VisibilityIcon />
                          </IconButton>
                        </Tooltip>
                      </TableCell>
                    </TableRow>
                  </Fade>
                ))}
              </TableBody>
            </Table>
          </Box>
        </Box>
      </Slide>
    </Container>
  );
}
