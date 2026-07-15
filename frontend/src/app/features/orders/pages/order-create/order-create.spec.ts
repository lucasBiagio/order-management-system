import { provideRouter } from '@angular/router';
import { ComponentFixture, TestBed } from '@angular/core/testing';
import { of } from 'rxjs';
import { vi } from 'vitest';

import { CustomerService } from '../../../../core/services/customer.service';
import { OrderService } from '../../../../core/services/order.service';
import { ProductService } from '../../../../core/services/product.service';
import { OrderCreate } from './order-create';

describe('OrderCreate', () => {
  let component: OrderCreate;
  let fixture: ComponentFixture<OrderCreate>;

  const customerServiceMock = {
    getAll: vi.fn().mockReturnValue(
      of([
        {
          id: 'customer-1',
          name: 'Lucas',
          email: 'lucas@email.com'
        }
      ])
    )
  };

  const productServiceMock = {
    getAll: vi.fn().mockReturnValue(
      of([
        {
          id: 'product-1',
          name: 'Mouse',
          price: 100,
          currentStock: 5
        }
      ])
    )
  };

  const orderServiceMock = {
    create: vi.fn().mockReturnValue(
      of({
        id: 'order-1',
        customerId: 'customer-1',
        orderDate: new Date().toISOString(),
        status: 1,
        totalAmount: 200,
        items: []
      })
    )
  };

  beforeEach(async () => {
    vi.clearAllMocks();

    await TestBed.configureTestingModule({
      imports: [OrderCreate],
      providers: [
        provideRouter([]),
        {
          provide: CustomerService,
          useValue: customerServiceMock
        },
        {
          provide: ProductService,
          useValue: productServiceMock
        },
        {
          provide: OrderService,
          useValue: orderServiceMock
        }
      ]
    }).compileComponents();

    fixture = TestBed.createComponent(OrderCreate);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create the component', () => {
    expect(component).toBeTruthy();
  });

  it('should load customers and products', () => {
    expect(customerServiceMock.getAll).toHaveBeenCalledOnce();
    expect(productServiceMock.getAll).toHaveBeenCalledOnce();

    expect(component.customers()).toHaveLength(1);
    expect(component.products()).toHaveLength(1);
    expect(component.isLoading()).toBeFalsy();
  });

  it('should add a product and calculate the total', () => {
    component.itemForm.setValue({
      productId: 'product-1',
      quantity: 2
    });

    component.addItem();

    expect(component.items()).toHaveLength(1);
    expect(component.items()[0].quantity).toBe(2);
    expect(component.totalAmount()).toBe(200);
  });

  it('should not add quantity greater than available stock', () => {
    component.itemForm.setValue({
      productId: 'product-1',
      quantity: 6
    });

    component.addItem();

    expect(component.items()).toHaveLength(0);
  });
});