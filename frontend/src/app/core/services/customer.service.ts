import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';

import {
  CreateCustomerRequest,
  Customer,
  UpdateCustomerRequest
} from '../models/customer';
import { BaseService } from './base.service';

@Injectable({
  providedIn: 'root'
})
export class CustomerService extends BaseService {
  private readonly endpoint = `${this.apiUrl}/customers`;

  getAll(): Observable<Customer[]> {
    return this.http.get<Customer[]>(this.endpoint);
  }

  getById(id: string): Observable<Customer> {
    return this.http.get<Customer>(`${this.endpoint}/${id}`);
  }

  create(request: CreateCustomerRequest): Observable<Customer> {
    return this.http.post<Customer>(this.endpoint, request);
  }

  update(id: string, request: UpdateCustomerRequest): Observable<void> {
    return this.http.put<void>(`${this.endpoint}/${id}`, request);
  }

  delete(id: string): Observable<void> {
    return this.http.delete<void>(`${this.endpoint}/${id}`);
  }
}