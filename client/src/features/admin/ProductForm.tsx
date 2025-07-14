import { useForm, type FieldValues } from "react-hook-form";
import { createProductSchema, type CreateProductSchema } from "../../app/lib/schemas/createProductSchema";
import { Box, Button, Grid, Paper, Typography } from "@mui/material";
import AppTextInput from "../../app/shared/AppTextInput";
import { zodResolver } from "@hookform/resolvers/zod";
import { useFetchFiltersQuery } from "../catalog/catalogApi";
import AppSelectInput from "../../app/shared/AppSelectInput";
import AppDropzone from "../../app/shared/AppDropzone";
import type { Product } from "../../app/models/product";
import { useEffect } from "react";
import { useCreateProductMutation, useUpdateProductMutation } from "./adminApi";
import { handleApiError } from "../../app/lib/util";

type Props = {
  setEditMode: (value: boolean) => void;
  product: Product | null;
  refetch: () => void;
  setSelectedProduct: (value: Product | null) => void;
}

export default function ProductForm({ setEditMode, product, refetch, setSelectedProduct }: Props) {
  const {
    control,
    handleSubmit,
    watch,
    reset,
    setError, 
    formState: { isSubmitting },
  } = useForm<CreateProductSchema>({
    mode: "onTouched",
    resolver: zodResolver(createProductSchema),
  });

  const watchFile = watch("file") as File & { preview: string };
  const {data} = useFetchFiltersQuery();
  const [createProduct] = useCreateProductMutation();
  const [updateProduct] = useUpdateProductMutation();

  useEffect(() => {
    if (product)  reset(product);

    return () => {
      if(watchFile) URL.revokeObjectURL(watchFile.preview);
    }
  }, [product, reset, watchFile]);

  const createFormData = (items: FieldValues) => {
    const formData = new FormData();
    for (const key in items) {
      formData.append(key, items[key]);
    }

    return formData;
  };
 
  const onSubmit = async (data: CreateProductSchema) => {
    try {
          const formData = createFormData(data);

          if(watchFile) formData.append("file", watchFile);

          if (product) await updateProduct({ id: product.id, data: formData }).unwrap();
          else await createProduct(formData).unwrap();
          setEditMode(false);
          refetch();
          setSelectedProduct(null);

    } catch (error) {
      console.log(error);
      handleApiError<CreateProductSchema>(error, setError, [
        "brand",
        "description",
        "file",
        "name",
        "pictureUrl",
        "price",
        "quantityInStock",
        "type",
      ]);
    }
  };

  return (
    <Box component={Paper} sx={{ p: 4, maxWidth: "lg", mx: "auto" }}>
      <Typography variant="h4" sx={{ mb: 4 }}>
        Product details
      </Typography>
      <form onSubmit={handleSubmit(onSubmit)}>
        <Grid container spacing={3}>
          <Grid size={12}>
            <AppTextInput control={control} name="name" label="Product Name" />
          </Grid>
          <Grid size={6}>
            {data?.brands && (
              <AppSelectInput
                control={control}
                name="brand"
                label="Brand"
                items={data.brands}
              />
            )}
          </Grid>
          <Grid size={6}>
            {data?.brands && (
              <AppSelectInput
                control={control}
                name="type"
                label="Type"
                items={data.types}
              />
            )}
          </Grid>
          <Grid size={6}>
            <AppTextInput
              type="number"
              control={control}
              name="price"
              label="Price in cents"
            />
          </Grid>
          <Grid size={6}>
            <AppTextInput
              type="number"
              control={control}
              name="quantityInStock"
              label="Quantity in Stock"
            />
          </Grid>
          <Grid size={12}>
            <AppTextInput
              control={control}
              name="description"
              label="Description"
              multiline
              rows={4}
            />
          </Grid>
          <Grid
            size={12}
            display="flex"
            justifyContent="space-between"
            alignItems="center"
          >
            <AppDropzone name="file" control={control} />
            {watchFile?.preview ? (
              <img
                src={watchFile.preview}
                alt="preview of image"
                style={{ maxHeight: 200 }}
              />
            ) : product?.pictureUrl ? (
              <img
                src={product?.pictureUrl}
                alt="preview of image"
                style={{ maxHeight: 200 }}
              />
            ): null}
          </Grid>
        </Grid>
        <Box display="flex" justifyContent="space-between" sx={{ mt: 3 }}>
          <Button onClick={() => setEditMode(false)} variant="contained" color="inherit">
            Cancel
          </Button>
          <Button loading={isSubmitting} variant="contained" color="success" type="submit">
            Submit
          </Button>
        </Box>
      </form>
    </Box>
  );
}
