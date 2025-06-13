import { createApi} from "@reduxjs/toolkit/query/react";
import { baseQueryWithErrorHandling } from "../../app/api/baseApi";
import {Item, type Basket } from "../../app/models/basket";
import type { Product } from "../../app/models/product";


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
            query: () => 'basket',
            providesTags: ['Basket'],
        }),
        
        //adiciona items
        addBasketItem: builder.mutation<Basket, {product: Product | Item, quantity: number}>({
            query: ({product, quantity}) => {
                const productId = isBasketItem(product) ? product.productId : product.id;
                return {
                  url: `basket?productId=${productId}&quantity=${quantity}`,
                  method: "POST",
                };
            },
            
            //código adicionado para automatizar o refresh da página de carrinho
            //quando os produtos forem adicionados na página de catálogo.
            onQueryStarted: async ({product, quantity}, { dispatch, queryFulfilled }) => {
                let isNweBasket = false;
                const patchResult = dispatch(
                    basketApi.util.updateQueryData('fetchBasket', undefined, (draft) => {
                        const productId = isBasketItem(product) ? product.productId : product.id;
                        
                        if(!draft?.basketId) isNweBasket =  true;
                        
                        if(!isNweBasket){
                            const existingItem = draft.items.find(item => item.productId === productId)

                            if (existingItem) existingItem.quantity += quantity;
                            else draft.items.push(isBasketItem(product) ? product : {...product, productId: product.id, quantity})
                        }
                    })
                )
                
                try {
                    await queryFulfilled;
                    if(isNweBasket) dispatch(basketApi.util.invalidateTags(['Basket']));
                }catch (error) {
                    console.log(error);
                    patchResult.undo();
                }
            }
        }),
        //remove itens
        removeBasketItem: builder.mutation<void, {productId: number, quantity: number}>({
            query: ({productId, quantity}) => ({
                url: `basket?productId=${productId}&quantity=${quantity}`,
                method: "DELETE",
            }),
            //código adicionado para automatizar o refresh da página de carrinho
            //quando os produtos forem removidos na página de catálogo.
            onQueryStarted: async ({productId, quantity}, {dispatch, queryFulfilled}) => {
                const patchResult = dispatch(
                    basketApi.util.updateQueryData('fetchBasket', undefined, (draft) => {
                        const itemIndex = draft.items.findIndex(item => item.productId === productId);
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
                }catch (error) {
                    console.log(error);
                    patchResult.undo();
                }
            } 
        })
    })
});

export const { useFetchBasketQuery, useAddBasketItemMutation, useRemoveBasketItemMutation } = basketApi;