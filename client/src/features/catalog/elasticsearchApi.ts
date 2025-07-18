import { createApi } from "@reduxjs/toolkit/query/react";
import { baseQueryWithErrorHandling } from "../../app/api/baseApi";
import type { SearchRequestDto, SearchResponseDto } from "../../app/models/elasticsearch";

export const elasticsearchApi = createApi({
  reducerPath: "elasticsearchApi",
  baseQuery: baseQueryWithErrorHandling,
  endpoints: (builder) => ({
    searchProducts: builder.query<SearchResponseDto, SearchRequestDto>({
      query: (params) => ({
        url: "/elasticsearch/search",
        method: "POST",
        body: params,
      }),
    }),
    healthCheck: builder.query<boolean, void>({
      query: () => "/elasticsearch/health",
    }),
    indexAllProducts: builder.mutation<string, void>({
      query: () => ({
        url: "/elasticsearch/index-all-products",
        method: "POST",
      }),
    }),
    syncProduct: builder.mutation<string, number>({
      query: (id) => ({
        url: `/elasticsearch/sync-product/${id}`,
        method: "POST",
      }),
    }),
    removeProduct: builder.mutation<string, number>({
      query: (id) => ({
        url: `/elasticsearch/remove-product/${id}`,
        method: "DELETE",
      }),
    }),
  }),
});

export const {
  useSearchProductsQuery,
  useHealthCheckQuery,
  useIndexAllProductsMutation,
  useSyncProductMutation,
  useRemoveProductMutation,
} = elasticsearchApi;
