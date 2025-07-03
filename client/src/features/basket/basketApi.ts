import { createApi} from "@reduxjs/toolkit/query/react";
import { baseQueryWithErrorHandling } from "../../app/api/baseApi";
import {Item, type Basket } from "../../app/models/basket";
import type { Product } from "../../app/models/product";
import Cookies from "js-cookie";

/*
    > builder.query<param1, param2>: é usado para buscar dados (operações do tipo GET), onde:
        param1: é o objeto retornado pela API. (ex: Basket)
        param2: é o tipo do argumento que a query espera receber (ex: void)

    > builder.mutation: - é usado para alterar dados (operações como POST, PUT, PATCH ou DELETE)
        param1: é o objeto retornado pela API. (ex: Basket)
        param2: é o tipo do argumento que a mutation espera receber (ex: {product: Product, quantity: number})
        
    > onQueryStarted:  é um callback opcional do RTK Query que permite executar lógica personalizada 
    assim que uma query ou mutation é iniciada, antes mesmo de receber a resposta do servidor.
    recebe dois parâmetros fixos: 

    1. arg: o argumento passado para a mutation ou query (pode ser um objeto com várias propriedades
    2. api: um objeto com várias ferramentas úteis, como dispatch, getState, queryFulfilled, entre outros.

    > dispatch: Imagine que você tem um estado global (como uma caixa com informações do seu app), 
    e quer dizer: “Ei, atualiza esse estado com esses novos dados!”. O dispatch faz exatamente 
    isso: ele envia uma ação para a store, pedindo que o estado seja atualizado.

    > queryFulfilled: é uma Promise que resolve quando a requisição termina com sucesso. 
    Você pode usar 'await queryFulfilled' para executar lógica somente após a resposta do servidor.

*/

function isBasketItem(product: Product | Item): product is Item {
  return (product as Item).quantity !== undefined;
}

export const basketApi = createApi({
  reducerPath: "basketApi",
  baseQuery: baseQueryWithErrorHandling,
  tagTypes: ["Basket"],
  endpoints: (builder) => ({
    //busca items
    fetchBasket: builder.query<Basket, void>({
      query: () => "basket",
      providesTags: ["Basket"],
    }),

    //adiciona items
    addBasketItem: builder.mutation<Basket, { product: Product | Item; quantity: number }>({
      query: ({ product, quantity }) => {
        const productId = isBasketItem(product)
          ? product.productId
          : product.id;
        return {
          url: `basket?productId=${productId}&quantity=${quantity}`,
          method: "POST",
        };
      },

      //código adicionado para automatizar o refresh da página de carrinho
      //quando os produtos forem adicionados na página de catálogo.
      onQueryStarted: async (
        { product, quantity },
        { dispatch, queryFulfilled }
      ) => {
        let isNewBasket = false;
        const patchResult = dispatch(
          basketApi.util.updateQueryData("fetchBasket", undefined, (draft) => {
            const productId = isBasketItem(product)
              ? product.productId
              : product.id;

            if (!draft?.basketId) isNewBasket = true;

            if (!isNewBasket) {
              const existingItem = draft.items.find(
                (item) => item.productId === productId
              );

              if (existingItem) existingItem.quantity += quantity;
              else
                draft.items.push(
                  isBasketItem(product)
                    ? product
                    : { ...product, productId: product.id, quantity }
                );
            }
          })
        );

        try {
          await queryFulfilled;
          if (isNewBasket) dispatch(basketApi.util.invalidateTags(["Basket"]));
        } catch (error) {
          console.log(error);
          patchResult.undo();
        }
      },
    }),
    //remove itens
    removeBasketItem: builder.mutation<
      void,
      { productId: number; quantity: number }
    >({
      query: ({ productId, quantity }) => ({
        url: `basket?productId=${productId}&quantity=${quantity}`,
        method: "DELETE",
      }),
      //código adicionado para automatizar o refresh da página de carrinho
      //quando os produtos forem removidos na página de catálogo.
      onQueryStarted: async (
        { productId, quantity },
        { dispatch, queryFulfilled }
      ) => {
        const patchResult = dispatch(
          basketApi.util.updateQueryData("fetchBasket", undefined, (draft) => {
            const itemIndex = draft.items.findIndex(
              (item) => item.productId === productId
            );
            if (itemIndex >= 0) {
              draft.items[itemIndex].quantity -= quantity;
              if (draft.items[itemIndex].quantity <= 0) {
                draft.items.splice(itemIndex, 1);
              }
            }
          })
        );

        try {
          await queryFulfilled;
        } catch (error) {
          console.log(error);
          patchResult.undo();
        }
      },
    }),
    clearBasket: builder.mutation<void, void>({
      queryFn: () => ({ data: undefined }),
      onQueryStarted: async (_, { dispatch }) => {
        dispatch(
          basketApi.util.updateQueryData("fetchBasket", undefined, (draft) => {
            draft.items = [];
            draft.basketId = '';
          })
        );
        Cookies.remove("basketId");
      },
    }),
  }),
});

export const { useFetchBasketQuery, useAddBasketItemMutation, useRemoveBasketItemMutation, useClearBasketMutation } = basketApi;