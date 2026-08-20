import { Component, signal, OnInit } from '@angular/core';
import { Router, RouterModule } from '@angular/router';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { AuthService } from '../../core/services/auth.service';
import { PolicyService } from '../../core/services/policy.service';
import { Policy } from '../../core/models/api-response.model';

@Component({
  selector: 'app-policy-management',
  imports: [RouterModule, CommonModule, FormsModule],
  templateUrl: './policy-management.html',
  styleUrl: './policy-management.css',
})
export class PolicyManagement implements OnInit {
  policies = signal<Policy[]>([]);
  loading = signal(true);
  error = signal('');
  success = signal('');
  
  // For admin: create/edit policy
  showPolicyForm = signal(false);
  editingPolicy = signal<Policy | null>(null);
  policyForm = {
    name: '',
    type: 'Health',
    premium: 0,
    coverageAmount: 0,
    coverageDetails: '',
    terms: ''
  };

  viewingPolicy = signal<Policy | null>(null);
  
  // For customer: purchasing a policy
  purchasingPolicy = signal<Policy | null>(null);
  purchaseFormData: any = {};

  constructor(
    public authService: AuthService,
    private policyService: PolicyService,
    private router: Router
  ) {}

  ngOnInit() {
    this.loadPolicies();
  }

  loadPolicies() {
    this.loading.set(true);
    this.error.set('');

    this.policyService.getAllPolicies().subscribe({
      next: (response) => {
        if (response.success && response.data) {
          this.policies.set(response.data);
        }
        this.loading.set(false);
      },
      error: (err) => {
        this.error.set('Failed to load policies');
        this.loading.set(false);
      }
    });
  }

  purchasePolicy(policy: Policy) {
    this.purchasingPolicy.set(policy);
    // Initialize form data with basic fields
    this.purchaseFormData = {
      phoneNumber: '',
      age: null,
      healthPlanType: 'Individual'
    };
  }

  proceedToPayment() {
    const policy = this.purchasingPolicy();
    if (!policy) return;

    // Validate generic required fields
    if (!this.purchaseFormData.phoneNumber) {
      this.error.set('Please fill out all required fields.');
      return;
    }

    this.loading.set(true);
    this.error.set('');
    
    // In a real app, this purchaseFormData would be sent to the backend
    this.policyService.purchasePolicy(policy.policyId).subscribe({
      next: (res) => {
        this.loading.set(false);
        if (res.success) {
          this.purchasingPolicy.set(null);
          this.router.navigate(['/payment'], {
            queryParams: {
              policyId: policy.policyId,
              policyName: policy.name,
              policyType: policy.type,
              amount: policy.premium
            }
          });
        } else {
          this.error.set(res.message || 'Failed to initiate purchase');
        }
      },
      error: (err) => {
        this.loading.set(false);
        this.error.set(err.error?.message || 'Failed to initiate purchase');
      }
    });
  }

  openCreateForm() {
    this.editingPolicy.set(null);
    this.policyForm = {
      name: '',
      type: 'Health',
      premium: 0,
      coverageAmount: 0,
      coverageDetails: '',
      terms: ''
    };
    this.showPolicyForm.set(true);
  }

  openEditForm(policy: Policy) {
    this.editingPolicy.set(policy);
    this.policyForm = {
      name: policy.name,
      type: policy.type,
      premium: policy.premium,
      coverageAmount: policy.coverageAmount,
      coverageDetails: policy.coverageDetails,
      terms: policy.terms
    };
    this.showPolicyForm.set(true);
  }

  closeForm() {
    this.showPolicyForm.set(false);
    this.editingPolicy.set(null);
  }

  savePolicy() {
    if (this.policyForm.premium < 0 || this.policyForm.premium > 10000) {
      alert('enter the amount in range (0-10000)');
      return;
    }

    this.loading.set(true);
    this.error.set('');
    this.success.set('');

    const editing = this.editingPolicy();
    const request = editing
      ? this.policyService.updatePolicy(editing.policyId, this.policyForm)
      : this.policyService.createPolicy(this.policyForm);

    request.subscribe({
      next: (response) => {
        if (response.success) {
          this.success.set(editing ? 'Policy updated!' : 'Policy created!');
          this.closeForm();
          this.loadPolicies();
        } else {
          this.error.set(response.message || 'Operation failed');
        }
        this.loading.set(false);
      },
      error: (err) => {
        this.error.set(err.error?.message || 'Operation failed');
        this.loading.set(false);
      }
    });
  }

  deletePolicy(policyId: string) {
    if (!confirm('Are you sure you want to delete this policy?')) {
      return;
    }

    this.loading.set(true);
    this.error.set('');
    this.success.set('');

    this.policyService.deletePolicy(policyId).subscribe({
      next: (response) => {
        if (response.success) {
          this.success.set('Policy deleted!');
          this.loadPolicies();
        } else {
          this.error.set(response.message || 'Delete failed');
        }
        this.loading.set(false);
      },
      error: (err) => {
        this.error.set(err.error?.message || 'Delete failed');
        this.loading.set(false);
      }
    });
  }
}

