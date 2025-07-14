// src/app/store.ts
import { configureStore } from "@reduxjs/toolkit";
import { useDispatch, useSelector } from "react-redux";
import { catalogApi } from "../../features/catalog/catalogApi";
import { errorApi } from "../../features/about/errorApi";
import { UiSlice } from "../layout/UiSlice";
import { basketApi } from "../../features/basket/basketApi";
import { catalogSlice } from "../../features/catalog/catalogSlice";
import { accountApi } from "../../features/account/accountApi";
import { checkoutApi } from "../../features/checkout/checkoutApi";
import { orderApi } from "../../features/orders/orderApi";
import { adminApi } from "../../features/admin/adminApi";

export const store = configureStore({
  reducer: {
    [catalogApi.reducerPath]: catalogApi.reducer,
    [errorApi.reducerPath]: errorApi.reducer,
    [basketApi.reducerPath]:  basketApi.reducer,
    [accountApi.reducerPath]: accountApi.reducer,
    [checkoutApi.reducerPath]: checkoutApi.reducer,
    [orderApi.reducerPath]: orderApi.reducer,
    [adminApi.reducerPath]: adminApi.reducer,
    ui: UiSlice.reducer,
    catalog: catalogSlice.reducer,
  },
  middleware: (getDefaultMiddleware) =>
    getDefaultMiddleware().concat(
        catalogApi.middleware, 
        errorApi.middleware,
        basketApi.middleware,
        accountApi.middleware,
        checkoutApi.middleware,
        orderApi.middleware,
        adminApi.middleware
    ),
});

export type RootState = ReturnType<typeof store.getState>;
export type AppDispatch = typeof store.dispatch;

export const useAppDispatch = useDispatch.withTypes<AppDispatch>(); //envia comandos para alterar o estado global (Você dá ordens para mudar algo)
export const useAppSelector = useSelector.withTypes<RootState>();   //lê informações do estado e retorna o tipo do estado (Você observa o que está acontecendo)
