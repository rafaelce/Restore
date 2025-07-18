import { gql } from '@apollo/client';

// Query para listar produtos com paginação
export const GET_PRODUCTS = gql`
  query GetProducts($first: Int, $after: String) {
    products(first: $first, after: $after) {
      nodes {
        id
        name
        price
        brand
        type
        pictureUrl
        description
        quantityInStock
      }
      pageInfo {
        hasNextPage
        hasPreviousPage
        startCursor
        endCursor
      }
    }
  }
`;

// Query para produto específico
export const GET_PRODUCT = gql`
  query GetProduct($id: Int!) {
    product(id: $id) {
      id
      name
      price
      brand
      type
      description
      pictureUrl
      quantityInStock
    }
  }
`;

// Mutation para criar produto
export const CREATE_PRODUCT = gql`
  mutation CreateProduct($input: CreateProductGraphQLDto!) {
    createProduct(input: $input) {
      id
      name
      price
      brand
      type
      description
      pictureUrl
      quantityInStock
    }
  }
`;

// Query para contar produtos
export const GET_PRODUCT_COUNT = gql`
  query GetProductCount {
    productCount
  }
`; 