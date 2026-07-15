import { CommonModule } from '@angular/common';
import { Component, inject, OnInit, signal } from '@angular/core';

import { MatButtonModule } from '@angular/material/button';
import { MatCardModule } from '@angular/material/card';
import { MatDialog, MatDialogModule } from '@angular/material/dialog';
import { MatIconModule } from '@angular/material/icon';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { NotificationService } from '../../../../core/services/notification.service';
import { Customer } from '../../../../core/models/customer';
import { CustomerService } from '../../../../core/services/customer.service';
import { ConfirmDialog } from '../../../../shared/components/confirm-dialog/confirm-dialog';
import { CustomerForm } from '../../components/customer-form/customer-form';

@Component({
  selector: 'app-customer-list',
  standalone: true,
  imports: [
    CommonModule,
    MatButtonModule,
    MatCardModule,
    MatDialogModule,
    MatIconModule,
    MatProgressSpinnerModule
  ],
  templateUrl: './customer-list.html',
  styleUrl: './customer-list.scss'
})
export class CustomerList implements OnInit {
  private readonly customerService = inject(CustomerService);
  private readonly dialog = inject(MatDialog);
private readonly notificationService = inject(NotificationService);
  readonly customers = signal<Customer[]>([]);
  readonly isLoading = signal(true);

  ngOnInit(): void {
    this.loadCustomers();
  }

  loadCustomers(): void {
    this.isLoading.set(true);

    this.customerService.getAll().subscribe({
      next: (customers) => {
        this.customers.set(customers);
        this.isLoading.set(false);
      },
      error: () => {
        this.isLoading.set(false);
      }
    });
  }

  openCreateDialog(): void {
    const dialogRef = this.dialog.open(CustomerForm, {
      width: '520px',
      maxWidth: '95vw',
      disableClose: true,
      data: {}
    });

    dialogRef.afterClosed().subscribe((created) => {
      if (created) {
        this.loadCustomers();
      }
    });
  }

  openEditDialog(customer: Customer): void {
    const dialogRef = this.dialog.open(CustomerForm, {
      width: '520px',
      maxWidth: '95vw',
      disableClose: true,
      data: {
        customer
      }
    });

    dialogRef.afterClosed().subscribe((updated) => {
      if (updated) {
        this.loadCustomers();
      }
    });
  }

  deleteCustomer(customer: Customer): void {
    const dialogRef = this.dialog.open(ConfirmDialog, {
      width: '460px',
      maxWidth: '95vw',
      data: {
        title: 'Excluir cliente',
        message: `Deseja realmente excluir o cliente "${customer.name}"?`,
        confirmText: 'Excluir',
        cancelText: 'Cancelar'
      }
    });

    dialogRef.afterClosed().subscribe((confirmed) => {
      if (!confirmed) {
        return;
      }

      this.notificationService.success(
        'Cliente removido com sucesso.'
      );

      this.loadCustomers();
    });
  }
}