import { OrderItem } from './order-item';

export interface Order {

    id: string;

    customerId: string;

    customerName: string;

    orderDate: string;

    status: string;

    totalAmount: number;

    items: OrderItem[];

}