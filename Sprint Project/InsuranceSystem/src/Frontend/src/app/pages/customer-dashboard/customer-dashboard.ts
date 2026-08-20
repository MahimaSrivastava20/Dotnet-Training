import { Component, signal, computed, OnInit } from '@angular/core';
import { RouterModule } from '@angular/router';
import { CommonModule } from '@angular/common';
import { AuthService } from '../../core/services/auth.service';
import { PolicyService } from '../../core/services/policy.service';
import { PaymentService } from '../../core/services/payment.service';
import { NotificationService } from '../../core/services/notification.service';
import { CustomerPolicy, Payment, Notification } from '../../core/models/api-response.model';

@Component({
  selector: 'app-customer-dashboard',
  imports: [RouterModule, CommonModule],
  templateUrl: './customer-dashboard.html',
  styleUrl: './customer-dashboard.css',
})
export class CustomerDashboard implements OnInit {
  policies = signal<CustomerPolicy[]>([]);
  payments = signal<Payment[]>([]);
  notifications = signal<Notification[]>([]);
  loading = signal(true);
  error = signal('');

  groupedPolicies = computed(() => {
    const groups: { [key: string]: { policy: CustomerPolicy, count: number } } = {};
    for (const p of this.policies()) {
      if (groups[p.policyId]) {
        groups[p.policyId].count++;
      } else {
        groups[p.policyId] = { policy: p, count: 1 };
      }
    }
    return Object.values(groups);
  });

  constructor(
    public authService: AuthService,
    private policyService: PolicyService,
    private paymentService: PaymentService,
    private notificationService: NotificationService
  ) {}

  ngOnInit() {
    this.loadDashboardData();
  }

  loadDashboardData() {
    this.loading.set(true);
    this.error.set('');

    // Load policies
    this.policyService.getMyPolicies().subscribe({
      next: (response) => {
        if (response.success && response.data) {
          this.policies.set(response.data);
        }
      },
      error: (err) => {
        console.error('Failed to load policies:', err);
      }
    });

    // Load payments
    this.paymentService.getMyPayments().subscribe({
      next: (response) => {
        if (response.success && response.data) {
          this.payments.set(response.data);
        }
      },
      error: (err) => {
        console.error('Failed to load payments:', err);
      }
    });

    // Load notifications
    this.notificationService.getMyNotifications().subscribe({
      next: (response) => {
        if (response.success && response.data) {
          this.notifications.set(response.data);
        }
        this.loading.set(false);
      },
      error: (err) => {
        console.error('Failed to load notifications:', err);
        this.loading.set(false);
      }
    });
  }

  markNotificationAsRead(id: string) {
    this.notificationService.markAsRead(id).subscribe({
      next: () => {
        this.loadDashboardData();
      },
      error: (err) => {
        console.error('Failed to mark notification as read:', err);
      }
    });
  }

  logout() {
    this.authService.logout();
  }
}

