import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';

import {
  CreateOrderRequest,
  Order,
  UpdateOrderStatusRequest
} from '../models/order';
import { BaseService } from './base.service';

@Injectable({
  providedIn: 'root'
})
export class OrderService extends BaseService {
  private readonly endpoint = `${this.apiUrl}/orders`;

  getAll(): Observable<Order[]> {
    return this.http.get<Order[]>(this.endpoint);
  }

  getById(id: string): Observable<Order> {
    return this.http.get<Order>(`${this.endpoint}/${id}`);
  }

  create(request: CreateOrderRequest): Observable<Order> {
    return this.http.post<Order>(this.endpoint, request);
  }

  updateStatus(
    id: string,
    request: UpdateOrderStatusRequest
  ): Observable<Order> {
    return this.http.patch<Order>(
      `${this.endpoint}/${id}/status`,
      request
    );
  }
}