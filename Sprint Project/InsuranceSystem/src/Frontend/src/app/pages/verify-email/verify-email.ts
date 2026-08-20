import { Component, signal, OnInit } from '@angular/core';
import { Router, RouterModule, ActivatedRoute } from '@angular/router';
import { FormsModule } from '@angular/forms';
import { CommonModule } from '@angular/common';
import { AuthService } from '../../core/services/auth.service';

@Component({
  selector: 'app-verify-email',
  imports: [RouterModule, FormsModule, CommonModule],
  templateUrl: './verify-email.html',
  styleUrl: './verify-email.css',
})
export class VerifyEmail implements OnInit {
  email = '';
  code = '';
  loading = signal(false);
  error = signal('');
  success = signal('');

  constructor(
    private authService: AuthService,
    private router: Router,
    private route: ActivatedRoute
  ) {}

  ngOnInit() {
    // Get email from query params if available
    this.route.queryParams.subscribe(params => {
      if (params['email']) {
        this.email = params['email'];
      }
    });
  }

  onSubmit() {
    if (!this.email || !this.code) {
      this.error.set('Please fill in all fields');
      return;
    }

    this.loading.set(true);
    this.error.set('');
    this.success.set('');

    this.authService.verifyEmail(this.email, this.code).subscribe({
      next: (response) => {
        if (response.success) {
          this.success.set('Email verified successfully! Redirecting to login...');
          setTimeout(() => {
            this.router.navigate(['/login']);
          }, 2000);
        } else {
          this.error.set(response.message || 'Verification failed');
        }
        this.loading.set(false);
      },
      error: (err) => {
        this.error.set(err.error?.message || 'Verification failed. Please try again.');
        this.loading.set(false);
      }
    });
  }

  resendCode() {
    if (!this.email) {
      this.error.set('Please enter your email');
      return;
    }

    this.loading.set(true);
    this.error.set('');
    this.success.set('');

    this.authService.resendVerification(this.email).subscribe({
      next: (response) => {
        if (response.success) {
          this.success.set('Verification code sent! Check your email.');
        } else {
          this.error.set(response.message || 'Failed to resend code');
        }
        this.loading.set(false);
      },
      error: (err) => {
        this.error.set(err.error?.message || 'Failed to resend code. Please try again.');
        this.loading.set(false);
      }
    });
  }
}

