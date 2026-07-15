import { Component, inject, signal } from '@angular/core';
import {
  FormBuilder,
  ReactiveFormsModule,
  Validators
} from '@angular/forms';

import { MatButtonModule } from '@angular/material/button';
import {
  MAT_DIALOG_DATA,
  MatDialogModule,
  MatDialogRef
} from '@angular/material/dialog';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';

import { Customer } from '../../../../core/models/customer';
import { CustomerService } from '../../../../core/services/customer.service';

export interface CustomerFormData {
  customer?: Customer;
}

@Component({
  selector: 'app-customer-form',
  standalone: true,
  imports: [
    ReactiveFormsModule,
    MatButtonModule,
    MatDialogModule,
    MatFormFieldModule,
    MatInputModule
  ],
  templateUrl: './customer-form.html',
  styleUrl: './customer-form.scss'
})
export class CustomerForm {
  private readonly formBuilder = inject(FormBuilder);
  private readonly customerService = inject(CustomerService);
  private readonly dialogRef = inject(MatDialogRef<CustomerForm>);

  readonly data = inject<CustomerFormData | null>(
    MAT_DIALOG_DATA,
    { optional: true }
  );

  readonly isSaving = signal(false);
  readonly isEditing = Boolean(this.data?.customer);

  readonly form = this.formBuilder.nonNullable.group({
    name: [
      this.data?.customer?.name ?? '',
      [
        Validators.required,
        Validators.minLength(2),
        Validators.maxLength(120)
      ]
    ],
    email: [
      this.data?.customer?.email ?? '',
      [
        Validators.required,
        Validators.email,
        Validators.maxLength(160)
      ]
    ]
  });

  submit(): void {
    if (this.form.invalid || this.isSaving()) {
      this.form.markAllAsTouched();
      return;
    }

    this.isSaving.set(true);

    const request = this.form.getRawValue();
    const customer = this.data?.customer;

    if (customer) {
      this.customerService.update(customer.id, request).subscribe({
        next: () => {
          this.dialogRef.close(true);
        },
        error: () => {
          this.isSaving.set(false);
        }
      });

      return;
    }

    this.customerService.create(request).subscribe({
      next: () => {
        this.dialogRef.close(true);
      },
      error: () => {
        this.isSaving.set(false);
      }
    });
  }

  cancel(): void {
    this.dialogRef.close(false);
  }
}