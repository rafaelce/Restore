import { createApi } from "@reduxjs/toolkit/query/react";
import { baseQueryWithErrorHandling } from "../../app/api/baseApi";
import type { CreateOrder, Order } from "../../app/models/order";

export const orderApi = createApi({
  reducerPath: "orderApi",
  baseQuery: baseQueryWithErrorHandling,
  tagTypes: ["Order"],
  endpoints: (builder) => ({
    fetchOrders: builder.query<Order[], void>({
      query: () => "orders",
      providesTags: ["Order"],
    }),
    fetchOrderDetailed: builder.query<Order, number>({
      query: (id) => ({
        url: `orders/${id}`,
      }),
    }),
    createOrder: builder.mutation<Order, CreateOrder>({
      query: (order) => ({
        url: "orders",
        method: "POST",
        body: order,
      }),
      onQueryStarted: async (_, { dispatch, queryFulfilled }) => {
        await queryFulfilled;
        dispatch(orderApi.util.invalidateTags(["Order"]));
      },
    }),
  }),
});

export const { 
    useFetchOrdersQuery, 
    useFetchOrderDetailedQuery, 
    useCreateOrderMutation 
} = orderApi;
