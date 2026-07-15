import { CommonModule, CurrencyPipe } from '@angular/common';
import {
  Component,
  computed,
  inject,
  OnInit,
  signal
} from '@angular/core';
import { forkJoin } from 'rxjs';

import { MatCardModule } from '@angular/material/card';
import { MatIconModule } from '@angular/material/icon';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';

import { CustomerService } from '../../../../core/services/customer.service';
import { ProductService } from '../../../../core/services/product.service';
import { OrderService } from '../../../../core/services/order.service';
import { Order, OrderStatus } from '../../../../core/models/order';

@Component({
  selector: 'app-dashboard-page',
  standalone: true,
  imports: [
    CommonModule,
    CurrencyPipe,
    MatCardModule,
    MatIconModule,
    MatProgressSpinnerModule
  ],
  templateUrl: './dashboard-page.html',
  styleUrl: './dashboard-page.scss'
})
export class DashboardPage implements OnInit {

  private readonly customerService = inject(CustomerService);
  private readonly productService = inject(ProductService);
  private readonly orderService = inject(OrderService);

  readonly isLoading = signal(true);

  readonly customerCount = signal(0);
  readonly productCount = signal(0);
  readonly orders = signal<Order[]>([]);

  readonly orderCount = computed(() => this.orders().length);

  readonly totalRevenue = computed(() =>
    this.orders().reduce(
      (total, order) => total + order.totalAmount,
      0
    )
  );

  readonly pendingOrders = computed(() =>
    this.orders().filter(
      order => order.status === OrderStatus.Pending
    ).length
  );

  readonly paidOrders = computed(() =>
    this.orders().filter(
      order => order.status === OrderStatus.Paid
    ).length
  );

  readonly cancelledOrders = computed(() =>
    this.orders().filter(
      order => order.status === OrderStatus.Cancelled
    ).length
  );

  readonly latestOrders = computed(() =>
    [...this.orders()]
      .sort(
        (a, b) =>
          new Date(b.orderDate).getTime() -
          new Date(a.orderDate).getTime()
      )
      .slice(0, 5)
  );

  ngOnInit(): void {
    this.loadDashboard();
  }

  private loadDashboard(): void {

    this.isLoading.set(true);

    forkJoin({
      customers: this.customerService.getAll(),
      products: this.productService.getAll(),
      orders: this.orderService.getAll()
    }).subscribe({

      next: ({ customers, products, orders }) => {

        this.customerCount.set(customers.length);
        this.productCount.set(products.length);
        this.orders.set(orders);

        this.isLoading.set(false);
      },

      error: () => {

        this.isLoading.set(false);

      }

    });

  }

}