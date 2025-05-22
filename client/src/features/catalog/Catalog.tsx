import { useEffect, useState } from "react";
import ProductList from "./ProductList";
import type { Product } from "../../app/models/product";

export default function Catalog() {
  const [products, setProducts] = useState<Product[]>([]);

  useEffect(() => {
    fetch('https://localhost:5001/api/products')
    .then(response => response.json())
    .then(data => setProducts(data))
    .catch();
  }, []);
  
  return (
    <>
      <ProductList products={products} />
    </>
  );
}