import { Component, signal, OnInit } from '@angular/core';
import { RouterModule, Router } from '@angular/router';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { AuthService } from '../../core/services/auth.service';
import { TicketService } from '../../core/services/ticket.service';
import { PolicyService } from '../../core/services/policy.service';
import { Ticket, CustomerPolicy } from '../../core/models/api-response.model';

@Component({
  selector: 'app-claims-review',
  imports: [RouterModule, CommonModule, FormsModule],
  templateUrl: './claims-review.html',
  styleUrl: './claims-review.css',
})
export class ClaimsReview implements OnInit {
  // Specialist / admin state
  tickets = signal<Ticket[]>([]);
  selectedTicket = signal<Ticket | null>(null);
  showRejectForm = signal(false);
  rejectionReason = '';

  // Customer state
  myPolicies = signal<CustomerPolicy[]>([]);
  myClaims = signal<Ticket[]>([]);
  activeClaimTab = signal<'policies' | 'claims'>('policies');

  loading = signal(true);
  error = signal('');
  success = signal('');

  constructor(
    public authService: AuthService,
    private ticketService: TicketService,
    private policyService: PolicyService,
    private router: Router
  ) {}

  ngOnInit() {
    if (this.authService.isCustomer()) {
      this.loadCustomerData();
    } else {
      this.loadTickets();
    }
  }

  // ====== Customer methods ======
  loadCustomerData() {
    this.loading.set(true);
    // Load purchased policies
    this.policyService.getMyPolicies().subscribe({
      next: (res) => {
        if (res.success && res.data) this.myPolicies.set(res.data);
        this.loading.set(false);
      },
      error: () => this.loading.set(false)
    });
    // Load their submitted claims
    this.ticketService.getAllTickets().subscribe({
      next: (res) => {
        if (res.success && res.data) {
          this.myClaims.set(res.data.filter(t => t.type === 'Claim'));
        }
      },
      error: () => {}
    });
  }

  fileClaim(policy: CustomerPolicy) {
    this.router.navigate(['/submit-claim'], {
      queryParams: {
        policyId: policy.policyId,
        customerPolicyId: policy.customerPolicyId,
        policyName: policy.policyName,
        policyType: policy.policyType,
        remainingCoverageAmount: policy.remainingCoverageAmount
      }
    });
  }

  // ====== Specialist / Admin methods ======
  loadTickets() {
    this.loading.set(true);
    this.error.set('');
    this.ticketService.getAllTickets().subscribe({
      next: (response) => {
        if (response.success && response.data) {
          const claims = response.data.filter(t => t.type === 'Claim');
          this.tickets.set(claims);
        }
        this.loading.set(false);
      },
      error: () => {
        this.error.set('Failed to load claims');
        this.loading.set(false);
      }
    });
  }

  selectTicket(ticket: Ticket) {
    this.selectedTicket.set(ticket);
    this.ticketService.getTicketById(ticket.ticketId).subscribe({
      next: (res) => { if (res.success && res.data) this.selectedTicket.set(res.data); },
      error: () => {}
    });
  }

  approveClaim(ticketId: string) {
    if (!confirm('Approve this claim?')) return;
    this.loading.set(true);
    this.ticketService.approveClaim(ticketId).subscribe({
      next: (res) => {
        if (res.success) {
          this.success.set('Claim approved successfully');
          this.loadTickets();
          this.selectedTicket.set(null);
        } else {
          this.error.set(res.message || 'Failed to approve');
        }
        this.loading.set(false);
      },
      error: (err) => {
        this.error.set(err.error?.message || 'Failed to approve claim');
        this.loading.set(false);
      }
    });
  }

  openRejectForm(ticketId: string) {
    this.showRejectForm.set(true);
    this.rejectionReason = '';
  }

  closeRejectForm() {
    this.showRejectForm.set(false);
    this.rejectionReason = '';
  }

  rejectClaim() {
    const ticket = this.selectedTicket();
    if (!ticket || !this.rejectionReason) {
      this.error.set('Please provide a rejection reason');
      return;
    }
    this.loading.set(true);
    this.ticketService.rejectClaim(ticket.ticketId, this.rejectionReason).subscribe({
      next: (res) => {
        if (res.success) {
          this.success.set('Claim rejected');
          this.closeRejectForm();
          this.loadTickets();
          this.selectedTicket.set(null);
        } else {
          this.error.set(res.message || 'Failed to reject claim');
        }
        this.loading.set(false);
      },
      error: (err) => {
        this.error.set(err.error?.message || 'Failed to reject claim');
        this.loading.set(false);
      }
    });
  }

  addComment(ticketId: string, message: string) {
    if (!message.trim()) return;
    this.ticketService.addComment(ticketId, message).subscribe({
      next: (res) => { if (res.success) this.selectTicket(this.selectedTicket()!); },
      error: () => {}
    });
  }
}
