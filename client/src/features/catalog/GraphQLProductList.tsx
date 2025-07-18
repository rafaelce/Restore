import React, { useState } from 'react';
import { useQuery } from '@apollo/client';
import { GET_PRODUCTS, GET_PRODUCT_COUNT } from './graphqlQueries';
import { 
  Box, 
  Button, 
  Card, 
  CardActions, 
  CardContent, 
  CardMedia, 
  Typography, 
  Grid, 
  Paper,
  Alert,
  Chip,
  Pagination
} from '@mui/material';
import AddIcon from '@mui/icons-material/Add';
import { Link } from 'react-router-dom';
import { useAddBasketItemMutation } from '../basket/basketApi';
import { useUserInfoQuery } from '../account/accountApi';
import currencyFormat from '../../app/lib/util';
import ProductForm from '../admin/ProductForm';

interface Product {
  id: number;
  name: string;
  price: number;
  brand: string;
  type: string;
  pictureUrl: string;
  description: string;
  quantityInStock: number;
}



const GraphQLProductList: React.FC = () => {
  const [showCreateForm, setShowCreateForm] = useState(false);
  const [currentPage, setCurrentPage] = useState(1);
  const [pageSize] = useState(10); // 10 produtos por página (padrão HotChocolate)
  const [cursors, setCursors] = useState<string[]>([]);
  
  // Verificar se o usuário é admin
  const { data: user } = useUserInfoQuery();
  const isAdmin = user?.roles?.includes('Admin') || false;

  // Query para produtos
  const { data, loading, error, refetch, fetchMore } = useQuery(GET_PRODUCTS, {
    variables: { first: pageSize, after: currentPage > 1 ? cursors[currentPage - 2] : null },
    fetchPolicy: 'cache-and-network'
  });

  // Query para contagem
  const { data: countData } = useQuery(GET_PRODUCT_COUNT);



  if (loading) return <Box textAlign="center" p={4}>Carregando produtos...</Box>;
  if (error) return <Alert severity="error" sx={{ m: 2 }}>Erro: {error.message}</Alert>;

  const products = data?.products?.nodes || [];
  
  // Extrair número da string "Total products: 20" ou usar fallback
  const extractCount = (countData: any) => {
    if (typeof countData === 'number') return countData;
    if (typeof countData === 'string') {
      const match = countData.match(/Total products: (\d+)/);
      return match ? parseInt(match[1]) : 0;
    }
    return 0;
  };
  
  const totalCount = extractCount(countData?.productCount) || products.length;
  const totalPages = totalCount > 0 ? Math.ceil(totalCount / pageSize) : 1;
  


  const handlePageChange = async (event: React.ChangeEvent<unknown>, page: number) => {
    if (page === currentPage) return;
    
    if (page === 1) {
      // Primeira página
      setCurrentPage(1);
      await refetch({ first: pageSize, after: null });
    } else if (page > currentPage) {
      // Próxima página
      if (data?.products?.pageInfo?.hasNextPage) {
        const endCursor = data.products.pageInfo.endCursor;
        const newCursors = [...cursors, endCursor];
        setCursors(newCursors);
        setCurrentPage(page);
        await fetchMore({
          variables: { first: pageSize, after: endCursor }
        });
      }
    } else {
      // Página anterior
      const targetCursor = cursors[page - 2] || null;
      setCurrentPage(page);
      await refetch({ first: pageSize, after: targetCursor });
    }
  };

  return (
    <Box>
      {/* Header */}
      <Paper sx={{ p: 3, mb: 3, background: 'linear-gradient(45deg, #1976d2 30%, #42a5f5 90%)' }}>
        <Box display="flex" justifyContent="space-between" alignItems="center">
          <Box>
            <Typography variant="h4" color="white" gutterBottom>
              Produtos via GraphQL
            </Typography>
            <Typography variant="body1" color="white" sx={{ opacity: 0.8 }}>
              Gerenciamento de produtos usando GraphQL
            </Typography>
          </Box>
          <Box display="flex" alignItems="center" gap={2}>
            <Chip 
              label={`Total: ${totalCount} produtos`} 
              color="primary" 
              variant="outlined"
              sx={{ color: 'white', borderColor: 'white' }}
            />
            {isAdmin && (
              <Button
                variant="contained"
                startIcon={<AddIcon />}
                onClick={() => setShowCreateForm(true)}
                sx={{ 
                  bgcolor: 'white', 
                  color: 'primary.main',
                  '&:hover': { bgcolor: 'grey.100' }
                }}
              >
                Adicionar Produto
              </Button>
            )}
          </Box>
        </Box>
      </Paper>

      {/* Lista de Produtos */}
      <Grid container spacing={3} justifyContent="center">
        {products.map((product: Product) => (
          <Grid key={product.id} size={{ xs: 12, sm: 6, md: 4, lg: 3 }}>
            <ProductCardGraphQL product={product} />
          </Grid>
        ))}
      </Grid>

      {products.length === 0 && (
        <Box textAlign="center" py={8}>
          <Typography variant="h6" color="textSecondary">
            Nenhum produto encontrado.
          </Typography>
        </Box>
      )}

                        {/* Paginação */}
                  <Box display="flex" justifyContent="center" mt={4}>
                    <Pagination
                      count={totalPages}
                      page={currentPage}
                      onChange={handlePageChange}
                      color="primary"
                      size="large"
                      showFirstButton
                      showLastButton
                    />
                  </Box>

      {/* Modal para criar produto usando ProductForm */}
      {showCreateForm && (
        <Box
          onClick={() => setShowCreateForm(false)}
          sx={{
            position: 'fixed',
            top: 0,
            left: 0,
            right: 0,
            bottom: 0,
            bgcolor: 'rgba(0, 0, 0, 0.5)',
            zIndex: 1300,
            display: 'flex',
            alignItems: 'center',
            justifyContent: 'center',
            p: 2
          }}
        >
          <Box
            onClick={(e) => e.stopPropagation()}
            sx={{
              bgcolor: 'background.paper',
              borderRadius: 2,
              maxWidth: 'lg',
              width: '100%',
              maxHeight: '90vh',
              overflow: 'auto'
            }}
          >
            <ProductForm
              setEditMode={setShowCreateForm}
              product={null}
              refetch={refetch}
              setSelectedProduct={() => {}}
            />
          </Box>
        </Box>
      )}

      {/* Informações do GraphQL */}
      <Paper sx={{ p: 3, mt: 4 }}>
        <Typography variant="h6" gutterBottom>
          Informações do GraphQL
        </Typography>
        <Box display="flex" flexWrap="wrap" gap={2} justifyContent="center">
          <Paper elevation={3} sx={{ p: 3, textAlign: 'center', bgcolor: 'primary.main', color: 'white', minWidth: 200, flex: '1 1 200px' }}>
            <Typography variant="h4" fontWeight="bold">
              {totalCount}
            </Typography>
            <Typography variant="body2" sx={{ opacity: 0.9 }}>
              Total de Produtos
            </Typography>
          </Paper>
          <Paper elevation={3} sx={{ p: 3, textAlign: 'center', bgcolor: 'success.main', color: 'white', minWidth: 200, flex: '1 1 200px' }}>
            <Typography variant="h4" fontWeight="bold">
              {products.length}
            </Typography>
            <Typography variant="body2" sx={{ opacity: 0.9 }}>
              Produtos na Página
            </Typography>
          </Paper>
          <Paper elevation={3} sx={{ p: 3, textAlign: 'center', bgcolor: 'secondary.main', color: 'white', minWidth: 200, flex: '1 1 200px' }}>
            <Typography variant="h6" fontWeight="bold">
              GET_PRODUCTS
            </Typography>
            <Typography variant="body2" sx={{ opacity: 0.9 }}>
              Query Executada
            </Typography>
          </Paper>
          <Paper elevation={3} sx={{ p: 3, textAlign: 'center', bgcolor: 'info.main', color: 'white', minWidth: 200, flex: '1 1 200px' }}>
            <Typography variant="h4" fontWeight="bold">
              {totalPages}
            </Typography>
            <Typography variant="body2" sx={{ opacity: 0.9 }}>
              Total de Páginas
            </Typography>
          </Paper>
        </Box>
      </Paper>
    </Box>
  );
};

// Componente ProductCard específico para GraphQL
const ProductCardGraphQL: React.FC<{ product: Product }> = ({ product }) => {
  const [addBasketItem, { isLoading }] = useAddBasketItemMutation();

  return (
    <Card
      elevation={3}
      sx={{
        width: '100%',
        borderRadius: 2,
        display: "flex",
        flexDirection: "column",
        justifyContent: "space-between",
        position: 'relative',
        '&:hover': {
          elevation: 6,
          transform: 'translateY(-4px)',
          transition: 'all 0.3s ease-in-out'
        }
      }}
    >
      <Box sx={{ position: 'relative' }}>
        <CardMedia
          sx={{ height: 240, backgroundSize: "cover" }}
          image={product.pictureUrl}
          title={product.name}
        />
        <Chip
          label="GraphQL"
          color="primary"
          size="small"
          sx={{
            position: 'absolute',
            top: 8,
            right: 8,
            bgcolor: 'primary.main',
            color: 'white'
          }}
        />
      </Box>
      <CardContent>
        <Typography
          gutterBottom
          sx={{ textTransform: "uppercase" }}
          variant="subtitle2"
        >
          {product.name}
        </Typography>
        <Typography variant="h6" sx={{ color: "secondary.main" }}>
          {currencyFormat(product.price)}
        </Typography>
        <Typography variant="body2" color="textSecondary" sx={{ mt: 1 }}>
          Estoque: {product.quantityInStock}
        </Typography>
      </CardContent>
      <CardActions sx={{ justifyContent: "space-between" }}>
        <Button 
          disabled={isLoading}
          onClick={() => addBasketItem({ product, quantity: 1 })}
        >
          Add to cart
        </Button>
        <Button component={Link} to={`/catalog/${product.id}`}>
          View
        </Button>
      </CardActions>
    </Card>
  );
};

export default GraphQLProductList; 