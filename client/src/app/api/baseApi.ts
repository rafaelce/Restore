import {
  fetchBaseQuery,
  type BaseQueryApi,
  type FetchArgs,
} from "@reduxjs/toolkit/query";
import { startLoading, stopLoading } from "../layout/UiSlice";
import { toast } from "react-toastify";
import { getNavigate } from "../routes/router";

const customBaseQuery = fetchBaseQuery({
  baseUrl: import.meta.env.VITE_API_URL,
  credentials: "include",
});

type ErrorResponse =
  | string
  | { title: string }
  | { errors: Record<string, string[]> };

const sleep = () => new Promise((resolve) => setTimeout(resolve, 1000));

const isObjectWithTitle = (data: unknown): data is { title: string } =>
  typeof data === "object" && data !== null && "title" in data;

const isObjectWithErrors = (
  data: unknown
): data is { errors: Record<string, string[]> } =>
  typeof data === "object" && data !== null && "errors" in data;


/*
  Essa função faz uma requisição de dados na API
  e gerencia erros que podem ocorrer durante o processo.
*/

export const baseQueryWithErrorHandling = async (
  args: string | FetchArgs,
  api: BaseQueryApi,
  extraOptions: object
) => {

  // pega o navigate do React Router armazenado globalmente
  const router = getNavigate(); 

  api.dispatch(startLoading());
  await sleep();
  const result = await customBaseQuery(args, api, extraOptions);
  api.dispatch(stopLoading());

  if (result.error) {
    console.log(result.error);

    const originalStatus =
      result.error.status === "PARSING_ERROR" && result.error.originalStatus
        ? result.error.originalStatus
        : result.error.status;

    const responseData = result.error.data as ErrorResponse;

    switch (originalStatus) {
      case 400:
        if (typeof responseData === "string") {
          toast.error(responseData);
        } else if (isObjectWithErrors(responseData)) {
          throw Object.values(responseData.errors).flat().join(", ");
        } else if (isObjectWithTitle(responseData)) {
          toast.error(responseData.title);
        }
        break;

      case 401:
        if (isObjectWithTitle(responseData)) {
          toast.error(responseData.title);
        }
        break;

      case 404:
        if (isObjectWithTitle(responseData) && router) {
          router("/not-found");
        }
        break;

      case 500:
        if (typeof responseData === "object" && router) {
          router("/server-error", { state: { error: responseData } });
        }
        break;

      default:
        toast.error("Erro desconhecido");
        break;
    }
  }

  return result;
};
