import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';

import {
  CreateProductRequest,
  Product,
  UpdateProductRequest
} from '../models/product';
import { BaseService } from './base.service';

@Injectable({
  providedIn: 'root'
})
export class ProductService extends BaseService {
  private readonly endpoint = `${this.apiUrl}/products`;

  getAll(): Observable<Product[]> {
    return this.http.get<Product[]>(this.endpoint);
  }

  getById(id: string): Observable<Product> {
    return this.http.get<Product>(`${this.endpoint}/${id}`);
  }

  create(request: CreateProductRequest): Observable<Product> {
    return this.http.post<Product>(this.endpoint, request);
  }

  update(id: string, request: UpdateProductRequest): Observable<void> {
    return this.http.put<void>(`${this.endpoint}/${id}`, request);
  }

  delete(id: string): Observable<void> {
    return this.http.delete<void>(`${this.endpoint}/${id}`);
  }
}