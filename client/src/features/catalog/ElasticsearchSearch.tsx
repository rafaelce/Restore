import { useEffect, useState } from "react";
import AppPagination from "../../app/shared/AppPagination";
import ProductCard from "./ProductCard";
import type { SearchRequestDto } from "../../app/models/elasticsearch";
import { useSearchProductsQuery } from "./elasticsearchApi";
import AppSelectInput from "../../app/shared/AppSelectInput";
import { useForm } from "react-hook-form";
import { Box, Grid, Typography } from "@mui/material";
import AppTextInput from "../../app/shared/AppTextInput";

type SearchFormData = {
  query: string;
  sortBy: string;
  sortOrder: string;
};

export default function ElasticsearchSearch() {
  const [searchParams, setSearchParams] = useState<SearchRequestDto>({
    query: "",
    page: 1,
    pageSize: 10,
    sortBy: "name",
    sortOrder: "asc",
  });

  const { control, watch } = useForm<SearchFormData>({
    defaultValues: {
      query: "",
      sortBy: "name",
      sortOrder: "asc",
    },
  });

  // Só fazer a query se tiver um termo de busca
  const shouldFetch = searchParams.query.trim().length > 0;
  const { data, isLoading, error } = useSearchProductsQuery(searchParams, {
    skip: !shouldFetch,
  });


  // Observar mudanças no formulário
  const watchedQuery = watch("query");
  const watchedSortBy = watch("sortBy");
  const watchedSortOrder = watch("sortOrder");

  // Atualizar searchParams quando o formulário mudar
  useEffect(() => {
    setSearchParams((prev) => ({
      ...prev,
      query: watchedQuery,
      sortBy: watchedSortBy,
      sortOrder: watchedSortOrder,
      page: 1, // Reset para primeira página quando mudar filtros
    }));
  }, [watchedQuery, watchedSortBy, watchedSortOrder]);

  const handlePageChange = (page: number) => {
    setSearchParams((prev) => ({ ...prev, page }));
  };

  if (isLoading) return <div>Carregando...</div>;
  if (error) return <div>Erro ao carregar produtos</div>;

  return (
    <Box sx={{ p: 4, maxWidth: "lg", mx: "auto" }}>
      <Typography variant="h4" sx={{ mb: 4 }}>
        Busca Avançada (Elasticsearch)
      </Typography>

      <Grid container spacing={3} sx={{ mb: 4 }}>
        <Grid size={4}>
          <AppTextInput
            control={control}
            name="query"
            label="Buscar produtos"
          />
        </Grid>

        <Grid size={4}>
          <AppSelectInput
            control={control}
            name="sortBy"
            label="Ordenar por"
            items={["name", "price", "brand", "type"]}
          />
        </Grid>

        <Grid size={4}>
          <AppSelectInput
            control={control}
            name="sortOrder"
            label="Ordem"
            items={["asc", "desc"]}
          />
        </Grid>
      </Grid>

      {!shouldFetch && (
        <Typography variant="body1" sx={{ textAlign: "center", mt: 4 }}>
          Digite um termo de busca para começar
        </Typography>
      )}

      {shouldFetch && data && (
        <>
          <Typography variant="body1" sx={{ mb: 2 }}>
            Encontrados {data.totalCount} produtos
          </Typography>

          <Grid container spacing={3} sx={{ mb: 4 }}>
            {data.products.map((product) => (
              <Grid size={3} key={product.id}>
                <ProductCard product={product} />
              </Grid>
            ))}
          </Grid>

          {data.totalPages > 1 && (
            <AppPagination
              metadata={{
                currentPage: data.page,
                totalPages: data.totalPages,
                pageSize: data.pageSize,
                totalCount: data.totalCount,
              }}
              onPageChange={handlePageChange}
            />
          )}
        </>
      )}

      {shouldFetch && error && (
        <Typography
          variant="body1"
          color="error"
          sx={{ textAlign: "center", mt: 4 }}
        >
          Erro ao carregar produtos
        </Typography>
      )}
    </Box>
  );
}