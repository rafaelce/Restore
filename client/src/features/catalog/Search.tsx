import { debounce, TextField } from "@mui/material";
import { useAppDispatch, useAppSelector } from "../../app/store/store";
import { setSearchTerm } from "./catalogSlice";
import { useEffect, useState } from "react";

export default function Search() {
  
  // a função useAppSelector acessar partes do estado global da sua aplicação
  const { searchTerm } = useAppSelector((state) => state.catalog);
  
  // a função useAppDispatch dar acesso à função dispatch que enviar actions 
  // para o Redux alterar o estado global da aplicação
  const dispatch = useAppDispatch();

  const [term, setTerm] = useState(searchTerm);

  useEffect(() => {
    setTerm(searchTerm);
  }, [searchTerm]);

  // debounce é uma função que limita a taxa de execução de uma função
  // para evitar que ela seja chamada muitas vezes em um curto período de tempo
  const debouncedSearch = debounce((event) => {
    dispatch(setSearchTerm(event.target.value));
  }, 500);

  return (
    <TextField
      label="Search products"
      variant="outlined"
      fullWidth
      type="search"
      value={term}
      onChange={(e) => {
        setTerm(e.target.value);
        debouncedSearch(e);
      }}
    />
  );
}
