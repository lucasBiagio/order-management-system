import { Routes } from '@angular/router';

import { DashboardPage } from './features/dashboard/pages/dashboard-page/dashboard-page';
import { CustomerList } from './features/customers/pages/customer-list/customer-list';
import { ProductList } from './features/products/pages/product-list/product-list';
import { OrderList } from './features/orders/pages/order-list/order-list';
import { OrderCreate } from './features/orders/pages/order-create/order-create';
import { ShellComponent } from './layout/shell/shell';

export const routes: Routes = [
  {
    path: '',
    component: ShellComponent,
    children: [
      {
        path: '',
        redirectTo: 'dashboard',
        pathMatch: 'full'
      },
      {
        path: 'dashboard',
        component: DashboardPage
      },
      {
        path: 'customers',
        component: CustomerList
      },
      {
        path: 'products',
        component: ProductList
      },
      {
        path: 'orders',
        component: OrderList
      },
      {
        path: 'orders/create',
        component: OrderCreate
      }
    ]
  },
  {
    path: '**',
    redirectTo: 'dashboard'
  }
];