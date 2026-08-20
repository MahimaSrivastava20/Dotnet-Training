import { Routes } from '@angular/router';
import { authGuard, adminGuard } from './core/guards/auth.guard';
// import { Testing } from './testing/testing';

export const routes: Routes = [
  // Public routes (no layout)
{
  path: 'testing',
  loadComponent: () => import('./pages/testing/testing').then(m => m.Testing)
},
  { path: '', loadComponent: () => import('./pages/welcome/welcome').then(m => m.Welcome) },
  { path: 'login', loadComponent: () => import('./pages/login/login').then(m => m.Login) },
  { path: 'register', loadComponent: () => import('./pages/register/register').then(m => m.Register) },
  { path: 'verify-email', loadComponent: () => import('./pages/verify-email/verify-email').then(m => m.VerifyEmail) },

  // Protected routes (wrapped in shared layout with sidebar)
  {
    path: '',
    loadComponent: () => import('./layout/app-layout/app-layout').then(m => m.AppLayout),
    canActivate: [authGuard],
    children: [
      {
        path: 'dashboard',
        loadComponent: () => import('./pages/customer-dashboard/customer-dashboard').then(m => m.CustomerDashboard),
      },
      {
        path: 'policies',
        loadComponent: () => import('./pages/policy-management/policy-management').then(m => m.PolicyManagement),
      },
      {
        path: 'payment',
        loadComponent: () => import('./pages/payment/payment').then(m => m.Payment),
      },
      {
        path: 'claims',
        loadComponent: () => import('./pages/claims-review/claims-review').then(m => m.ClaimsReview)
      },
      {
        path: 'support',
        loadComponent: () => import('./pages/support/support').then(m => m.SupportComponent)
      },
      {
        path: 'submit-claim',
        loadComponent: () => import('./pages/submit-claim/submit-claim').then(m => m.SubmitClaim)
      },
      {
        path: 'admin',
        loadComponent: () => import('./pages/admin-dashboard/admin-dashboard').then(m => m.AdminDashboard),
        canActivate: [adminGuard],
      },
    ]
  },

  { path: '**', redirectTo: '' }
];
