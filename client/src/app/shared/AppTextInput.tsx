import { type TextFieldProps, TextField } from "@mui/material";
import { type FieldValues, type UseControllerProps, useController } from "react-hook-form";

type Props<T extends FieldValues> = {
  label: string
  name: keyof T
} & UseControllerProps<T> &
  TextFieldProps;

export default function AppTextInput<T extends FieldValues>(props: Props<T>) {
  const { fieldState, field } = useController({ ...props });

  // Handler ajustado para enviar undefined para campos numéricos vazios
  const handleChange = (e: React.ChangeEvent<HTMLInputElement>) => {
    let value: string | number | undefined = e.target.value;
    if (props.type === "number") {
      value = value === "" ? undefined : +value;
    }
    field.onChange(value);
  };

  return (
    <TextField
      {...props}
      {...field}
      onChange={handleChange}
      multiline={props.multiline}
      rows={props.rows}
      type={props.type}
      fullWidth
      value={field.value ?? ""}
      variant="outlined"
      error={!!fieldState.error}
      helperText={fieldState.error?.message}
    />
  );
}
