import React from 'react';
import { Box, Typography, Paper, Chip } from '@mui/material';
import GraphQLProductList from './GraphQLProductList';

const GraphQLPage: React.FC = () => {
  return (
    <Box>
      {/* Hero Section */}
      <Paper 
        sx={{ 
          p: 4, 
          mb: 4, 
          background: 'linear-gradient(135deg, #1976d2 0%, #42a5f5 50%, #90caf9 100%)',
          color: 'white'
        }}
      >
        <Box textAlign="center">
          <Typography variant="h3" gutterBottom sx={{ fontWeight: 'bold' }}>
            🚀 GraphQL Demo
          </Typography>
          <Typography variant="h6" sx={{ mb: 3, opacity: 0.9 }}>
            Esta página demonstra o uso do GraphQL em nossa aplicação. 
            Aqui você pode visualizar produtos, criar novos produtos e ver 
            como o GraphQL oferece uma abordagem mais eficiente para consultas.
          </Typography>
          <Box display="flex" justifyContent="center" gap={2} flexWrap="wrap">
            <Chip 
              label="Endpoint: /graphql" 
              color="primary" 
              variant="outlined"
              sx={{ color: 'white', borderColor: 'white' }}
            />
            <Chip 
              label="Status: Funcionando" 
              color="success" 
              variant="outlined"
              sx={{ color: 'white', borderColor: 'white' }}
            />
          </Box>
        </Box>
      </Paper>

      {/* Main Content */}
      <GraphQLProductList />

      {/* Features Section */}
      <Paper sx={{ p: 4, mt: 4 }}>
        <Typography variant="h4" gutterBottom textAlign="center" sx={{ mb: 4 }}>
          Vantagens do GraphQL
        </Typography>
        <Typography variant="body1" textAlign="center" color="textSecondary" sx={{ mb: 4 }}>
          Descubra como o GraphQL revoluciona a forma como consumimos APIs
        </Typography>
        
        <Box display="flex" flexWrap="wrap" gap={3} justifyContent="center">
          <Paper sx={{ p: 3, height: '100%', textAlign: 'center', minWidth: 300, flex: '1 1 300px' }}>
            <Typography variant="h3" color="primary" sx={{ mb: 2 }}>🎯</Typography>
            <Typography variant="h6" gutterBottom>Consultas Precisas</Typography>
            <Typography variant="body2" color="textSecondary">
              Busque apenas os dados que você precisa, reduzindo o tráfego de rede e melhorando a performance.
            </Typography>
          </Paper>

          <Paper sx={{ p: 3, height: '100%', textAlign: 'center', minWidth: 300, flex: '1 1 300px' }}>
            <Typography variant="h3" color="success.main" sx={{ mb: 2 }}>⚡</Typography>
            <Typography variant="h6" gutterBottom>Performance Otimizada</Typography>
            <Typography variant="body2" color="textSecondary">
              Uma única requisição pode buscar múltiplos recursos relacionados, eliminando o over-fetching.
            </Typography>
          </Paper>

          <Paper sx={{ p: 3, height: '100%', textAlign: 'center', minWidth: 300, flex: '1 1 300px' }}>
            <Typography variant="h3" color="secondary" sx={{ mb: 2 }}>🔍</Typography>
            <Typography variant="h6" gutterBottom>Tipagem Forte</Typography>
            <Typography variant="body2" color="textSecondary">
              Schema bem definido com validação automática de tipos e IntelliSense completo.
            </Typography>
          </Paper>

          <Paper sx={{ p: 3, height: '100%', textAlign: 'center', minWidth: 300, flex: '1 1 300px' }}>
            <Typography variant="h3" color="warning.main" sx={{ mb: 2 }}>📚</Typography>
            <Typography variant="h6" gutterBottom>Documentação Automática</Typography>
            <Typography variant="body2" color="textSecondary">
              O schema serve como documentação sempre atualizada da API, facilitando o desenvolvimento.
            </Typography>
          </Paper>

          <Paper sx={{ p: 3, height: '100%', textAlign: 'center', minWidth: 300, flex: '1 1 300px' }}>
            <Typography variant="h3" color="error" sx={{ mb: 2 }}>🔄</Typography>
            <Typography variant="h6" gutterBottom>Evolução da API</Typography>
            <Typography variant="body2" color="textSecondary">
              Adicione novos campos sem quebrar consultas existentes, mantendo compatibilidade.
            </Typography>
          </Paper>

          <Paper sx={{ p: 3, height: '100%', textAlign: 'center', minWidth: 300, flex: '1 1 300px' }}>
            <Typography variant="h3" color="info.main" sx={{ mb: 2 }}>🛠️</Typography>
            <Typography variant="h6" gutterBottom>Ferramentas Avançadas</Typography>
            <Typography variant="body2" color="textSecondary">
              GraphQL Playground, Apollo DevTools e outras ferramentas para melhor DX.
            </Typography>
          </Paper>
        </Box>
      </Paper>
    </Box>
  );
};

export default GraphQLPage; 