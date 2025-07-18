export interface ProductSearchDto {
  id: number;
  name: string;
  description: string;
  price: number;
  brand: string;
  type: string;
  quantityInStock: number;
  pictureUrl: string;
  createdAt: string;
  updatedAt: string;
}

export interface SearchRequestDto {
  query: string;
  page?: number;
  pageSize?: number;
  sortBy?: string;
  sortOrder?: string;
  minPrice?: number;
  maxPrice?: number;
  brand?: string;
  type?: string;
}

export interface SearchResponseDto {
  products: ProductSearchDto[];
  totalCount: number;
  page: number;
  pageSize: number;
  totalPages: number;
}
