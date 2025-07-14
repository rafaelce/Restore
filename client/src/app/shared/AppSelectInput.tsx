import {FormControl, FormHelperText, InputLabel, MenuItem, Select} from "@mui/material";
import {
  type FieldValues,
  type UseControllerProps,
  useController,
} from "react-hook-form";

type Props<T extends FieldValues> = {
  label: string;
  name: keyof T;
  items: string[];
} & UseControllerProps<T>;

export default function AppSelectInput<T extends FieldValues>(props: Props<T>) {
  const { fieldState, field } = useController({ ...props });

  return (
    <FormControl fullWidth error={!!fieldState.error}>
      <InputLabel>{props.label}</InputLabel>
      <Select
        value={field.value || ""}
        label={props.label}
        onChange={field.onChange}
      >
        {props.items.map((item: string, index: number) => (
          <MenuItem value={item} key={index}>
            {item}
          </MenuItem>
        ))}
      </Select>
      <FormHelperText>{fieldState.error?.message}</FormHelperText>
    </FormControl>
  );
}

