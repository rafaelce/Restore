import { fetchBaseQuery, type BaseQueryApi, type FetchArgs } from "@reduxjs/toolkit/query";
import { startLoading, stopLoading } from "../layout/UiSlice";

const customBaseQuery = fetchBaseQuery({
  baseUrl: import.meta.env.VITE_API_URL,
});

const sleep = () => new Promise((resolve) => setTimeout(resolve, 1000));

export const baseQueryWithErrorHandling = async (
  args: string | FetchArgs,
  api: BaseQueryApi,
  extraOptions: object
) => {

  api.dispatch(startLoading());
  
  await sleep();
  const result = await customBaseQuery(args, api, extraOptions);
  
  api.dispatch(stopLoading());

  if (result.error) {
    const { status, data } = result.error;
    console.log({ status, data });
  }

  return result;
};


