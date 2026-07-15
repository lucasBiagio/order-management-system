export interface Product {
  id: string;
  name: string;
  price: number;
  currentStock: number;
}

export interface CreateProductRequest {
  name: string;
  price: number;
  currentStock: number;
}

export interface UpdateProductRequest {
  name: string;
  price: number;
  currentStock: number;
}