import { Box, Grid, Typography } from "@mui/material";
import { useFetchFiltersQuery, useFetchProductsQuery } from "./catalogApi";
import ProductList from "./ProductList";
import Filters from "./Filters";
import { useAppDispatch, useAppSelector } from "../../app/store/store";
import AppPagination from "../../app/shared/AppPagination";
import { setPageNumber } from "./catalogSlice";
import SearchOffIcon from "@mui/icons-material/SearchOff";
import AppLoading from "../../app/shared/AppLoading";


export default function Catalog() {
  const productParams = useAppSelector((state) => state.catalog);
  const { data, isLoading } = useFetchProductsQuery(productParams);
  const { data: filtersData, isLoading: filtersLoading } = useFetchFiltersQuery();
  const dispatch = useAppDispatch();

  if (isLoading || !data || filtersLoading || !filtersData) return <AppLoading text="Loading catalog" />;

  return (
    <Grid container spacing={4}>
      <Grid size={3}>
        <Filters filtersData={filtersData} />
      </Grid>
      <Grid size={9}>
        {data.items && data.items.length > 0 ? (
          <>
            <ProductList products={data.items} />
            <AppPagination
              metadata={data.pagination}
              onPageChange={(page: number) => {
                dispatch(setPageNumber(page));
                window.scrollTo({ top: 0, behavior: "smooth" });
              }}
            />
          </>
        ) : (
          <Box
            display="flex"
            flexDirection="column"
            alignItems="center"
            justifyContent="center"
            height="60vh"
            textAlign="center"
          >
            <SearchOffIcon color="disabled" sx={{ fontSize: 60, mb: 2 }} />
            <Typography variant="h5" color="textSecondary" gutterBottom>
              No products found
            </Typography>
            <Typography variant="body1" color="textSecondary">
              Try adjusting your filters to find what you're looking for.
            </Typography>
          </Box>
        )}
      </Grid>
    </Grid>
  );
}