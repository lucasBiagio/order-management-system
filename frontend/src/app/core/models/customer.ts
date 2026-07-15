export interface Customer {
  id: string;
  name: string;
  email: string;
}

export interface CreateCustomerRequest {
  name: string;
  email: string;
}

export interface UpdateCustomerRequest {
  name: string;
  email: string;
}