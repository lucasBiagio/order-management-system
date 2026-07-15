import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';

import { BaseService } from './base.service';
import { Order } from '../models/order';

@Injectable({
    providedIn: 'root'
})
export class OrderService extends BaseService {

    getAll(): Observable<Order[]> {

        return this.http.get<Order[]>(`${this.apiUrl}/orders`);

    }

    getById(id: number): Observable<Order> {

        return this.http.get<Order>(`${this.apiUrl}/orders/${id}`);
    }

    create(order: Order): Observable<Order> {

        return this.http.post<Order>(`${this.apiUrl}/orders`, order);
    }

    updateStatus(id: number, order: Order): Observable<Order> {

        return this.http.put<Order>(`${this.apiUrl}/orders/${id}`, order);
    }   

}