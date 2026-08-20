import { Component, OnInit, signal } from '@angular/core';
import { ActivatedRoute, Router, RouterModule } from '@angular/router';
import { FormsModule } from '@angular/forms';
import { CommonModule } from '@angular/common';
import { TicketService } from '../../core/services/ticket.service';
import { PolicyService } from '../../core/services/policy.service';

@Component({
  selector: 'app-submit-claim',
  imports: [RouterModule, FormsModule, CommonModule],
  templateUrl: './submit-claim.html',
  styleUrl: './submit-claim.css',
})
export class SubmitClaim implements OnInit {
  policyId = '';
  customerPolicyId = '';
  policyName = '';
  policyType = '';
  remainingCoverageAmount = 0;

  title = '';
  description = '';
  amount: number | null = null;
  photoUrl = ''; // We'll just use a direct URL input or mock file base64

  loading = signal(false);
  error = signal('');
  success = signal('');

  constructor(
    private route: ActivatedRoute,
    private router: Router,
    private ticketService: TicketService,
    private policyService: PolicyService
  ) {}

  ngOnInit() {
    this.route.queryParams.subscribe(params => {
      this.policyId = params['policyId'] || '';
      this.customerPolicyId = params['customerPolicyId'] || '';
      this.policyName = decodeURIComponent(params['policyName'] || '');
      this.policyType = decodeURIComponent(params['policyType'] || '');
      this.remainingCoverageAmount = Number(params['remainingCoverageAmount']) || 0;

      if (!this.policyId) {
        this.router.navigate(['/claims']);
      }
    });
  }

  onFileChange(event: any) {
    const file = event.target.files[0];
    if (file) {
      // In a real app we'd upload to cloud storage and get URL.
      // Here we mock with a data URI or just use a placeholder image if it's too big
      const reader = new FileReader();
      reader.onload = (e: any) => {
        // Keep it small or just use a dummy URL to prevent huge payloads
        this.photoUrl = e.target.result;
      };
      reader.readAsDataURL(file);
    }
  }

  submitClaim() {
    if (!this.title || !this.description || !this.amount || this.amount <= 0) {
      this.error.set('Please fill in all required fields.');
      return;
    }

    if (this.remainingCoverageAmount > 0 && this.amount > this.remainingCoverageAmount) {
      this.error.set(`Cannot claim more than your remaining coverage (₹${this.remainingCoverageAmount}).`);
      return;
    }

    this.loading.set(true);
    this.error.set('');

    const payload = {
      title: this.title,
      description: this.description,
      type: 'Claim',
      policyId: this.customerPolicyId,
      claimAmount: this.amount,
      documents: this.photoUrl || 'https://i.imgur.com/3g7nmJC.png'
    };

    this.ticketService.createTicket(payload).subscribe({
      next: (res) => {
        if (res.success) {
          this.success.set('Claim submitted successfully! Redirecting...');
          setTimeout(() => {
            this.router.navigate(['/claims']);
          }, 2000);
        } else {
          this.error.set(res.message || 'Failed to submit claim.');
          this.loading.set(false);
        }
      },
      error: (err) => {
        this.error.set(err.error?.message || 'Failed to submit claim.');
        this.loading.set(false);
      }
    });
  }
}
