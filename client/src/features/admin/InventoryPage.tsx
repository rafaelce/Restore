import { 
  Container, 
  Box, 
  Table, 
  TableBody, 
  TableCell, 
  TableHead, 
  TableRow, 
  Typography,
  IconButton,
  Tooltip,
  Fade,
  Slide,
  Chip
} from "@mui/material";
import { useAppDispatch, useAppSelector } from "../../app/store/store";
import { useFetchProductsQuery } from "../catalog/catalogApi";
import currencyFormat from "../../app/lib/util";
import { Delete, Edit, Inventory, Add, ArrowUpward, ArrowDownward } from "@mui/icons-material";
import AppPagination from "../../app/shared/AppPagination";
import { setPageNumber, setOrderBy, setOrderDirection } from "../catalog/catalogSlice";
import { useState, useEffect } from "react";
import AppLoading from "../../app/shared/AppLoading";
import ProductForm from "./ProductForm";
import type { Product } from "../../app/models/product";
import { useDeleteProductMutation } from "./adminApi";

// Componente para cabeçalho clicável
const SortableHeader = ({ 
  label, 
  field, 
  currentOrderBy, 
  currentOrderDirection, 
  onSort 
}: {
  label: string;
  field: string;
  currentOrderBy: string;
  currentOrderDirection: "asc" | "desc";
  onSort: (field: string) => void;
}) => {
  const isActive = currentOrderBy === field;

  return (
    <TableCell 
      align="center"
      sx={{ 
        fontWeight: 700, 
        fontSize: '1.1rem',
        color: '#495057',
        borderBottom: '2px solid #dee2e6',
        cursor: 'pointer',
        userSelect: 'none',
        transition: 'all 0.3s ease',
        '&:hover': {
          background: '#e3f2fd',
          color: '#1976d2'
        }
      }}
      onClick={() => onSort(field)}
    >
      <Box sx={{ display: 'flex', alignItems: 'center', justifyContent: 'center', gap: 1 }}>
        {label}
        {isActive && (
          currentOrderDirection === "asc" ? 
            <ArrowUpward sx={{ fontSize: 16, color: '#1976d2' }} /> : 
            <ArrowDownward sx={{ fontSize: 16, color: '#1976d2' }} />
        )}
      </Box>
    </TableCell>
  );
};

export default function InventoryPage() {
  const productParams = useAppSelector((state) => state.catalog);
  
  // Monta o parâmetro orderBy conforme a direção
  const params = {
    ...productParams,
    orderBy: productParams.orderDirection === 'desc'
      ? `-${productParams.orderBy}`
      : productParams.orderBy
  };
  const { data, refetch, isLoading } = useFetchProductsQuery(params);
  const dispatch = useAppDispatch();
  const [mounted, setMounted] = useState(false);
  const [editMode, setEditMode] = useState(false);

  const [selectedProduct, setSelectedProduct] = useState<Product | null>(null);
  const [deleteProduct] = useDeleteProductMutation();

  const handleSelectProduct = (product: Product) => {
    setSelectedProduct(product);
    setEditMode(true);
  };

  const handleDeleteProduct = async (id: number) => {
    try {
      await deleteProduct(id);
      refetch();
    } catch (error) {
      console.log(error);
    }
  };

  useEffect(() => {
    setMounted(true);
  }, []);

  const handleSort = (field: string) => {
    if (productParams.orderBy === field) {
      // Se já está ordenando por este campo, inverte a direção
      dispatch(setOrderDirection(productParams.orderDirection === "asc" ? "desc" : "asc"));
    } else {
      // Se é um novo campo, define como ascendente
      dispatch(setOrderBy(field));
      dispatch(setOrderDirection("asc"));
    }
  };

  if (isLoading) return <AppLoading text="Loading inventory" />

  if (!data || !data.items) return <Typography variant="h5" sx={{ fontWeight: 600 }}> No products available </Typography>

  if (editMode) return (
    <ProductForm
      setEditMode={setEditMode}
      product={selectedProduct}
      refetch={refetch}
      setSelectedProduct={setSelectedProduct}
    />
  );

  return (
    <Container maxWidth="lg" sx={{ mt: 4, mb: 4 }}>
      <Slide direction="down" in={mounted} timeout={800}>
        <Box>
          <Box
            sx={{
              background: "#ffffff",
              borderRadius: 4,
              overflow: "hidden",
              boxShadow: "0 4px 12px rgba(0,0,0,0.1)",
              border: "1px solid #e0e0e0",
            }}
          >
            <Box
              sx={{
                background: "#1976d2",
                p: 3,
                display: "flex",
                alignItems: "center",
                gap: 2,
              }}
            >
              <Inventory sx={{ color: "white", fontSize: 32 }} />
              <Typography variant="h6" sx={{ color: "white", fontWeight: 600 }}>
                Product Inventory
              </Typography>
              <Box sx={{ flexGrow: 1 }} />
              <Chip
                label={`${
                  data.pagination?.totalCount ?? data.items.length
                } products`}
                sx={{
                  background: "rgba(255,255,255,0.2)",
                  color: "white",
                  fontWeight: 600,
                }}
              />
              <Tooltip title="Add New Product" arrow>
                <IconButton
                  onClick={() => setEditMode(true)}
                  sx={{
                    background: "rgba(255,255,255,0.2)",
                    color: "white",
                    "&:hover": {
                      background: "rgba(255,255,255,0.3)",
                      transform: "scale(1.05)",
                    },
                    transition: "all 0.3s ease",
                  }}
                >
                  <Add />
                </IconButton>
              </Tooltip>
            </Box>

            <Table sx={{ minWidth: 650 }}>
              <TableHead>
                <TableRow sx={{ background: "#f5f5f5" }}>
                  <TableCell
                    align="center"
                    sx={{
                      fontWeight: 700,
                      fontSize: "1.1rem",
                      color: "#495057",
                      borderBottom: "2px solid #dee2e6",
                    }}
                  >
                    ID
                  </TableCell>
                  <TableCell
                    sx={{
                      fontWeight: 700,
                      fontSize: "1.1rem",
                      color: "#495057",
                      borderBottom: "2px solid #dee2e6",
                    }}
                  >
                    Product
                  </TableCell>
                  <SortableHeader
                    label="Price"
                    field="price"
                    currentOrderBy={productParams.orderBy}
                    currentOrderDirection={productParams.orderDirection}
                    onSort={handleSort}
                  />
                  <SortableHeader
                    label="Type"
                    field="type"
                    currentOrderBy={productParams.orderBy}
                    currentOrderDirection={productParams.orderDirection}
                    onSort={handleSort}
                  />
                  <SortableHeader
                    label="Brand"
                    field="brand"
                    currentOrderBy={productParams.orderBy}
                    currentOrderDirection={productParams.orderDirection}
                    onSort={handleSort}
                  />
                  <SortableHeader
                    label="Stock"
                    field="quantityInStock"
                    currentOrderBy={productParams.orderBy}
                    currentOrderDirection={productParams.orderDirection}
                    onSort={handleSort}
                  />
                  <TableCell
                    align="center"
                    sx={{
                      fontWeight: 700,
                      fontSize: "1.1rem",
                      color: "#495057",
                      borderBottom: "2px solid #dee2e6",
                    }}
                  >
                    Actions
                  </TableCell>
                </TableRow>
              </TableHead>
              <TableBody>
                {data.items.map((product, index) => (
                  <Fade
                    in={mounted}
                    timeout={1000 + index * 200}
                    key={product.id}
                  >
                    <TableRow
                      hover
                      sx={{
                        cursor: "pointer",
                        transition: "all 0.3s ease",
                        "&:hover": {
                          background: "#f8f9fa",
                          transform: "translateY(-1px)",
                          boxShadow: "0 2px 8px rgba(0,0,0,0.1)",
                        },
                        "&:nth-of-type(even)": {
                          background: "rgba(248, 249, 250, 0.5)",
                        },
                      }}
                    >
                      <TableCell
                        align="center"
                        sx={{
                          fontWeight: 600,
                          fontSize: "1.1rem",
                          color: "#495057",
                        }}
                      >
                        <Box
                          sx={{
                            background: "#1976d2",
                            color: "white",
                            borderRadius: 2,
                            px: 2,
                            py: 1,
                            display: "inline-block",
                            fontWeight: 700,
                            boxShadow: "0 2px 4px rgba(25, 118, 210, 0.3)",
                          }}
                        >
                          #{product.id}
                        </Box>
                      </TableCell>
                      <TableCell>
                        <Box display="flex" alignItems="center" gap={2}>
                          <img
                            src={product.pictureUrl}
                            alt={product.name}
                            style={{
                              height: 50,
                              width: 50,
                              borderRadius: 8,
                              objectFit: "cover",
                              boxShadow: "0 2px 4px rgba(0,0,0,0.1)",
                            }}
                          />
                          <Typography
                            sx={{ fontWeight: 600, color: "#495057" }}
                          >
                            {product.name}
                          </Typography>
                        </Box>
                      </TableCell>
                      <TableCell
                        align="right"
                        sx={{
                          fontSize: "1.1rem",
                          fontWeight: 600,
                          color: "#28a745",
                        }}
                      >
                        {currencyFormat(product.price)}
                      </TableCell>
                      <TableCell align="center">
                        <Chip
                          label={product.type}
                          sx={{
                            background: "#e3f2fd",
                            color: "#1976d2",
                            fontWeight: 600,
                          }}
                        />
                      </TableCell>
                      <TableCell align="center">
                        <Chip
                          label={product.brand}
                          sx={{
                            background: "#f3e5f5",
                            color: "#7b1fa2",
                            fontWeight: 600,
                          }}
                        />
                      </TableCell>
                      <TableCell align="center">
                        <Box
                          sx={{
                            background:
                              product.quantityInStock > 10
                                ? "#d4edda"
                                : product.quantityInStock > 0
                                ? "#fff3cd"
                                : "#f8d7da",
                            color:
                              product.quantityInStock > 10
                                ? "#155724"
                                : product.quantityInStock > 0
                                ? "#856404"
                                : "#721c24",
                            borderRadius: 2,
                            px: 2,
                            py: 1,
                            display: "inline-block",
                            fontWeight: 700,
                            fontSize: "0.9rem",
                          }}
                        >
                          {product.quantityInStock} units
                        </Box>
                      </TableCell>
                      <TableCell align="center">
                        <Box
                          sx={{
                            display: "flex",
                            gap: 1,
                            justifyContent: "center",
                          }}
                        >
                          <Tooltip title="Edit Product" arrow>
                            <IconButton
                              sx={{
                                background: "#ffc107",
                                color: "white",
                                "&:hover": {
                                  background: "#e0a800",
                                  transform: "scale(1.05)",
                                  boxShadow: "0 2px 8px rgba(255, 193, 7, 0.4)",
                                },
                                transition: "all 0.3s ease",
                              }}
                            >
                              <Edit onClick={() => handleSelectProduct(product)}/>
                            </IconButton>
                          </Tooltip>
                          <Tooltip title="Delete Product" arrow>
                            <IconButton
                              sx={{
                                background: "#dc3545",
                                color: "white",
                                "&:hover": {
                                  background: "#c82333",
                                  transform: "scale(1.05)",
                                  boxShadow: "0 2px 8px rgba(220, 53, 69, 0.4)",
                                },
                                transition: "all 0.3s ease",
                              }}
                            >
                              <Delete onClick={() => handleDeleteProduct(product.id)} />
                            </IconButton>
                          </Tooltip>
                        </Box>
                      </TableCell>
                    </TableRow>
                  </Fade>
                ))}
              </TableBody>
            </Table>
          </Box>
          <Box>
            {data.pagination && (
              <AppPagination
                metadata={data.pagination}
                onPageChange={(page: number) => {
                  dispatch(setPageNumber(page));
                  window.scrollTo({ top: 0, behavior: "smooth" });
                }}
              />
            )}
          </Box>
        </Box>
      </Slide>
    </Container>
  );
}
