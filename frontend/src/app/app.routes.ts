import { Routes } from '@angular/router';

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
        loadComponent: () =>
          import(
            './features/dashboard/pages/dashboard-page/dashboard-page'
          ).then((component) => component.DashboardPage)
      },
      {
        path: 'customers',
        loadComponent: () =>
          import(
            './features/customers/pages/customer-list/customer-list'
          ).then((component) => component.CustomerList)
      },
      {
        path: 'products',
        loadComponent: () =>
          import(
            './features/products/pages/product-list/product-list'
          ).then((component) => component.ProductList)
      },
      {
        path: 'orders',
        loadComponent: () =>
          import(
            './features/orders/pages/order-list/order-list'
          ).then((component) => component.OrderList)
      },
      {
        path: 'orders/create',
        loadComponent: () =>
          import(
            './features/orders/pages/order-create/order-create'
          ).then((component) => component.OrderCreate)
      }
    ]
  },
  {
    path: '**',
    redirectTo: 'dashboard'
  }
];