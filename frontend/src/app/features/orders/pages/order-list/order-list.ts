import { CommonModule } from '@angular/common';
import {
  Component,
  inject,
  OnInit,
  signal
} from '@angular/core';
import { RouterLink } from '@angular/router';
import { forkJoin } from 'rxjs';

import { MatButtonModule } from '@angular/material/button';
import { MatCardModule } from '@angular/material/card';
import { MatIconModule } from '@angular/material/icon';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { MatSelectModule } from '@angular/material/select';

import { Customer } from '../../../../core/models/customer';
import {
  Order,
  OrderStatus
} from '../../../../core/models/order';
import { CustomerService } from '../../../../core/services/customer.service';
import { OrderService } from '../../../../core/services/order.service';

interface OrderListItem extends Order {
  customerName: string;
  totalItems: number;
  isUpdatingStatus: boolean;
}

@Component({
  selector: 'app-order-list',
  standalone: true,
  imports: [
    CommonModule,
    RouterLink,
    MatButtonModule,
    MatCardModule,
    MatIconModule,
    MatProgressSpinnerModule,
    MatSelectModule
  ],
  templateUrl: './order-list.html',
  styleUrl: './order-list.scss'
})
export class OrderList implements OnInit {
  private readonly orderService = inject(OrderService);
  private readonly customerService = inject(CustomerService);

  readonly orders = signal<OrderListItem[]>([]);
  readonly isLoading = signal(true);

  readonly statusOptions = [
    {
      value: OrderStatus.Pending,
      label: 'Pendente'
    },
    {
      value: OrderStatus.Paid,
      label: 'Pago'
    },
    {
      value: OrderStatus.Cancelled,
      label: 'Cancelado'
    }
  ];

  ngOnInit(): void {
    this.loadOrders();
  }

  loadOrders(): void {
    this.isLoading.set(true);

    forkJoin({
      orders: this.orderService.getAll(),
      customers: this.customerService.getAll()
    }).subscribe({
      next: ({ orders, customers }) => {
        const customerNames = this.buildCustomerNames(customers);

        const items = orders
          .map((order) => ({
            ...order,
            customerName:
              customerNames.get(order.customerId) ??
              'Cliente não encontrado',
            totalItems: order.items.reduce(
              (total, item) => total + item.quantity,
              0
            ),
            isUpdatingStatus: false
          }))
          .sort(
            (first, second) =>
              new Date(second.orderDate).getTime() -
              new Date(first.orderDate).getTime()
          );

        this.orders.set(items);
        this.isLoading.set(false);
      },
      error: () => {
        this.isLoading.set(false);
      }
    });
  }

  updateStatus(
    order: OrderListItem,
    status: OrderStatus
  ): void {
    if (order.status === status || order.isUpdatingStatus) {
      return;
    }

    this.setUpdatingStatus(order.id, true);

    this.orderService
      .updateStatus(order.id, { status })
      .subscribe({
        next: (updatedOrder) => {
          this.orders.update((orders) =>
            orders.map((currentOrder) =>
              currentOrder.id === order.id
                ? {
                    ...currentOrder,
                    status: updatedOrder.status,
                    isUpdatingStatus: false
                  }
                : currentOrder
            )
          );
        },
        error: () => {
          this.setUpdatingStatus(order.id, false);
        }
      });
  }

  getStatusLabel(status: OrderStatus): string {
    switch (status) {
      case OrderStatus.Pending:
        return 'Pendente';

      case OrderStatus.Paid:
        return 'Pago';

      case OrderStatus.Cancelled:
        return 'Cancelado';

      default:
        return 'Desconhecido';
    }
  }

  getStatusClass(status: OrderStatus): string {
    switch (status) {
      case OrderStatus.Pending:
        return 'status-pending';

      case OrderStatus.Paid:
        return 'status-paid';

      case OrderStatus.Cancelled:
        return 'status-cancelled';

      default:
        return 'status-unknown';
    }
  }

  private setUpdatingStatus(
    orderId: string,
    isUpdatingStatus: boolean
  ): void {
    this.orders.update((orders) =>
      orders.map((order) =>
        order.id === orderId
          ? {
              ...order,
              isUpdatingStatus
            }
          : order
      )
    );
  }

  private buildCustomerNames(
    customers: Customer[]
  ): Map<string, string> {
    return new Map(
      customers.map((customer) => [
        customer.id,
        customer.name
      ])
    );
  }
}