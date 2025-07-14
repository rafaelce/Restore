export type ProductParams = {
  orderBy: string;
  orderDirection: "asc" | "desc";
  searchTerm?: string;
  types: string[];
  brands: string[];
  pageNumber: number;
  pageSize: number;
};
