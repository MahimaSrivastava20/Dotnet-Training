import { Component, signal, OnInit } from '@angular/core';
import { ActivatedRoute, Router, RouterModule } from '@angular/router';
import { FormsModule } from '@angular/forms';
import { CommonModule } from '@angular/common';
import { PaymentService } from '../../core/services/payment.service';
import { HttpClient } from '@angular/common/http';
import { environment } from '../../../environments/environment';

// Declare Razorpay as global (loaded via script tag in index.html)
declare var Razorpay: any;

@Component({
  selector: 'app-payment',
  imports: [RouterModule, FormsModule, CommonModule],
  templateUrl: './payment.html',
  styleUrl: './payment.css',
})
export class Payment implements OnInit {
  policyId = '';
  policyName = '';
  policyType = '';
  amount = 0;

  step = signal<'init' | 'loading' | 'success' | 'error'>('init');
  error = signal('');
  transactionRef = signal('');
  totalAmount = signal(0);

  constructor(
    private route: ActivatedRoute,
    private router: Router,
    private paymentService: PaymentService,
    private http: HttpClient
  ) {}

  ngOnInit() {
    this.route.queryParams.subscribe(params => {
      this.policyId = params['policyId'] || '';
      this.policyName = decodeURIComponent(params['policyName'] || 'Insurance Policy');
      this.policyType = decodeURIComponent(params['policyType'] || '');
      this.amount = Number(params['amount']) || 0;
      this.totalAmount.set(Math.round(this.amount * 1.18 * 100) / 100);

      if (!this.policyId) {
        this.router.navigate(['/policies']);
      }
    });

    // Dynamically load Razorpay checkout.js if not already loaded
    if (typeof Razorpay === 'undefined') {
      const script = document.createElement('script');
      script.src = 'https://checkout.razorpay.com/v1/checkout.js';
      script.async = true;
      document.body.appendChild(script);
    }
  }

  openRazorpay() {
    this.step.set('loading');
    this.error.set('');

    // Step 1: Create Razorpay order on backend
    this.http.post<any>(`${environment.apiUrl}/payments/create-order`, {
      policyId: this.policyId,
      amount: this.amount,
      policyName: this.policyName
    }).subscribe({
      next: (res) => {
        if (res.success && res.data) {
          this.launchRazorpayModal(res.data);
        } else {
          this.error.set(res.message || 'Failed to create payment order');
          this.step.set('error');
        }
      },
      error: (err) => {
        this.error.set(err.error?.message || 'Failed to create payment order. Please ensure backend is running.');
        this.step.set('error');
      }
    });
  }

  private launchRazorpayModal(orderData: any) {
    const options = {
      key: orderData.keyId,
      amount: orderData.amount,
      currency: orderData.currency || 'INR',
      name: 'SafeHaven',
      description: `${this.policyName} — ${this.policyType} Insurance`,
      order_id: orderData.orderId,
      image: 'https://i.imgur.com/3g7nmJC.png',
      theme: { color: '#005bbf' },
      prefill: {},
      handler: (response: any) => {
        // Step 2: Verify payment signature on backend
        this.verifyPayment(response, orderData);
      },
      modal: {
        ondismiss: () => {
          this.step.set('init');
        }
      }
    };

    this.step.set('init');
    const rzp = new Razorpay(options);
    rzp.open();

    rzp.on('payment.failed', (response: any) => {
      this.error.set(response.error?.description || 'Payment failed in Razorpay');
      this.step.set('error');
    });
  }

  private verifyPayment(razorpayResponse: any, orderData: any) {
    this.step.set('loading');

    this.http.post<any>(`${environment.apiUrl}/payments/verify`, {
      orderId: razorpayResponse.razorpay_order_id,
      paymentId: razorpayResponse.razorpay_payment_id,
      signature: razorpayResponse.razorpay_signature,
      policyId: this.policyId,
      amount: this.amount
    }).subscribe({
      next: (res) => {
        if (res.success) {
          this.transactionRef.set(razorpayResponse.razorpay_payment_id);
          this.step.set('success');
        } else {
          this.error.set(res.message || 'Payment verification failed');
          this.step.set('error');
        }
      },
      error: (err) => {
        this.error.set(err.error?.message || 'Payment verification failed');
        this.step.set('error');
      }
    });
  }

  goToDashboard() {
    this.router.navigate(['/dashboard']);
  }

  retry() {
    this.step.set('init');
    this.error.set('');
  }
}
