# Insurance Portal - API Integration Summary

## Overview
This document summarizes all API endpoints connected between the frontend and backend, including newly implemented features.

---

## Backend Enhancements

### 1. **Email Verification (IdentityService)**
**New Endpoints Added:**
- `POST /api/auth/verify-email` - Verify user email with code
- `POST /api/auth/resend-verification` - Resend verification code

**DTOs Added:**
- `VerifyEmailDto` - Email and verification code
- `ResendVerificationDto` - Email for resending code

### 2. **Policy Management (PolicyService)**
**New Endpoints Added:**
- `PUT /api/policies/{id}` - Update policy (Admin only)
- `DELETE /api/policies/{id}` - Delete policy (Admin only)

**Service Methods Added:**
- `UpdateAsync()` - Update existing policy
- `DeleteAsync()` - Soft delete policy

---

## Frontend Implementation

### Core Infrastructure

#### **1. Services Created**
Located in `Frontend/src/app/core/services/`:

- **auth.service.ts** - Authentication & user management
  - register(), login(), verifyEmail(), resendVerification()
  - Token storage and user state management
  - Role-based access checks

- **policy.service.ts** - Policy operations
  - getAllPolicies(), getPolicyById()
  - createPolicy(), updatePolicy(), deletePolicy()
  - purchasePolicy(), renewPolicy(), getMyPolicies()

- **ticket.service.ts** - Ticket/Claims management
  - createTicket(), getAllTickets(), getTicketById()
  - updateTicketStatus(), assignTicket()
  - addComment(), getComments()
  - approveClaim(), rejectClaim()

- **payment.service.ts** - Payment processing
  - processPayment(), getMyPayments(), getPaymentById()

- **notification.service.ts** - Notifications
  - getMyNotifications(), markAsRead(), markAllAsRead()

- **admin.service.ts** - Admin operations
  - getDashboard(), getTicketReport(), getClaimReport(), getPaymentReport()
  - getAllUsers(), toggleUserStatus()
  - createClaimsSpecialist(), createSupportSpecialist()

#### **2. Models Created**
Located in `Frontend/src/app/core/models/api-response.model.ts`:

- ApiResponse<T> - Generic API response wrapper
- AuthResponse - Authentication response with token
- User, Policy, CustomerPolicy
- Ticket, ClaimDetails, Comment
- Payment, Notification
- DashboardMetrics

#### **3. HTTP Interceptor**
Located in `Frontend/src/app/core/interceptors/auth.interceptor.ts`:
- Automatically adds JWT Bearer token to all HTTP requests
- Configured in app.config.ts

#### **4. Route Guards**
Located in `Frontend/src/app/core/guards/auth.guard.ts`:
- **authGuard** - Requires authentication
- **adminGuard** - Requires Admin role
- **customerGuard** - Requires Customer role

---

## Page Implementations

### **1. Authentication Pages**

#### Login (`/login`)
- Email/password form
- Calls `authService.login()`
- Redirects to `/admin` for Admin, `/dashboard` for Customer
- Error handling and loading states

#### Register (`/register`)
- Name, email, password, confirm password form
- Calls `authService.register()`
- Redirects to `/verify-email` with email parameter
- Password validation (min 6 chars, match confirmation)

#### Verify Email (`/verify-email`)
- Email and 6-digit code form
- Calls `authService.verifyEmail()`
- Resend code functionality
- Success message and redirect to login

### **2. Customer Pages**

#### Customer Dashboard (`/dashboard`)
**Protected by:** customerGuard

**Features:**
- Displays user's policies (via `policyService.getMyPolicies()`)
- Shows payment history (via `paymentService.getMyPayments()`)
- Lists notifications (via `notificationService.getMyNotifications()`)
- Mark notifications as read
- Logout functionality

#### Policy Management (`/policies`)
**Protected by:** authGuard

**Customer Features:**
- View all available policies
- Purchase policy button (calls `policyService.purchasePolicy()`)

**Admin Features:**
- Create new policy (form with name, type, premium, coverage, terms)
- Edit existing policy
- Delete policy (soft delete)
- All CRUD operations via PolicyService

### **3. Admin Pages**

#### Admin Dashboard (`/admin`)
**Protected by:** adminGuard

**Features:**
- **Overview Tab:**
  - Dashboard metrics (total users, policies, tickets, claims, payments)
  - Calls `adminService.getDashboard()`

- **Users Tab:**
  - List all users
  - Toggle user active/inactive status
  - Create Claims Specialist
  - Create Support Specialist

- **Reports Tab:**
  - Ticket report (with date filtering)
  - Claim report (with date filtering)
  - Payment report (with date filtering)

#### Claims Review (`/claims`)
**Protected by:** authGuard

**Features:**
- List all claim tickets (filtered by type='Claim')
- View claim details including:
  - Claim amount
  - Documents
  - Customer information
  - Comments
- Approve claim (calls `ticketService.approveClaim()`)
- Reject claim with reason (calls `ticketService.rejectClaim()`)
- Add comments to claims

---

## API Endpoint Mapping

### Authentication Endpoints
| Frontend Call | Backend Endpoint | Method | Auth Required |
|--------------|------------------|--------|---------------|
| authService.register() | /api/auth/register | POST | No |
| authService.login() | /api/auth/login | POST | No |
| authService.verifyEmail() | /api/auth/verify-email | POST | No |
| authService.resendVerification() | /api/auth/resend-verification | POST | No |

### Policy Endpoints
| Frontend Call | Backend Endpoint | Method | Auth Required |
|--------------|------------------|--------|---------------|
| policyService.getAllPolicies() | /api/policies | GET | No |
| policyService.getPolicyById() | /api/policies/{id} | GET | No |
| policyService.createPolicy() | /api/policies | POST | Admin |
| policyService.updatePolicy() | /api/policies/{id} | PUT | Admin |
| policyService.deletePolicy() | /api/policies/{id} | DELETE | Admin |
| policyService.purchasePolicy() | /api/policies/purchase | POST | Customer |
| policyService.renewPolicy() | /api/policies/renew/{id} | POST | Customer |
| policyService.getMyPolicies() | /api/policies/my-policies | GET | Yes |

### Ticket/Claims Endpoints
| Frontend Call | Backend Endpoint | Method | Auth Required |
|--------------|------------------|--------|---------------|
| ticketService.createTicket() | /api/tickets | POST | Customer |
| ticketService.getAllTickets() | /api/tickets | GET | Yes |
| ticketService.getTicketById() | /api/tickets/{id} | GET | Yes |
| ticketService.updateTicketStatus() | /api/tickets/{id}/status | PUT | Specialist/Admin |
| ticketService.assignTicket() | /api/tickets/{id}/assign | POST | Admin |
| ticketService.addComment() | /api/tickets/{id}/comments | POST | Yes |
| ticketService.getComments() | /api/tickets/{id}/comments | GET | Yes |
| ticketService.approveClaim() | /api/tickets/{id}/approve | POST | ClaimsSpecialist/Admin |
| ticketService.rejectClaim() | /api/tickets/{id}/reject | POST | ClaimsSpecialist/Admin |

### Payment Endpoints
| Frontend Call | Backend Endpoint | Method | Auth Required |
|--------------|------------------|--------|---------------|
| paymentService.processPayment() | /api/payments | POST | Customer |
| paymentService.getMyPayments() | /api/payments/my | GET | Customer |
| paymentService.getPaymentById() | /api/payments/{id} | GET | Customer/Admin |

### Notification Endpoints
| Frontend Call | Backend Endpoint | Method | Auth Required |
|--------------|------------------|--------|---------------|
| notificationService.getMyNotifications() | /api/notifications/my | GET | Yes |
| notificationService.markAsRead() | /api/notifications/{id}/read | PUT | Yes |
| notificationService.markAllAsRead() | /api/notifications/read-all | PUT | Yes |

### Admin Endpoints
| Frontend Call | Backend Endpoint | Method | Auth Required |
|--------------|------------------|--------|---------------|
| adminService.getDashboard() | /api/reporting/admin/dashboard | GET | Admin |
| adminService.getTicketReport() | /api/reporting/admin/reports/tickets | GET | Admin |
| adminService.getClaimReport() | /api/reporting/admin/reports/claims | GET | Admin |
| adminService.getPaymentReport() | /api/reporting/admin/reports/payments | GET | Admin |
| adminService.getAllUsers() | /api/identity/admin/users | GET | Admin |
| adminService.toggleUserStatus() | /api/identity/admin/users/{id}/toggle-status | PUT | Admin |
| adminService.createClaimsSpecialist() | /api/identity/admin/create-claims-specialist | POST | Admin |
| adminService.createSupportSpecialist() | /api/identity/admin/create-support-specialist | POST | Admin |

---

## Configuration

### Environment Configuration
**File:** `Frontend/src/environments/environment.ts`
```typescript
export const environment = {
  production: false,
  apiUrl: 'http://localhost:5000/api'
};
```

### App Configuration
**File:** `Frontend/src/app/app.config.ts`
- Configured HttpClient with auth interceptor
- Router configuration with lazy loading

### API Gateway Routes
**File:** `Gateway/ApiGateway/ocelot.json`
- All routes configured to forward to appropriate microservices
- JWT authentication enabled on protected routes
- Gateway runs on port 5000

---

## Authentication Flow

1. **Registration:**
   - User fills registration form → POST /api/auth/register
   - Backend creates user, returns JWT token
   - Frontend stores token in localStorage
   - Redirects to /verify-email (optional verification)

2. **Login:**
   - User enters credentials → POST /api/auth/login
   - Backend validates and returns JWT token with user info
   - Frontend stores token and user data
   - Redirects based on role (Admin → /admin, Customer → /dashboard)

3. **Authenticated Requests:**
   - Auth interceptor adds `Authorization: Bearer {token}` header
   - Backend validates JWT on protected endpoints
   - Returns 401 if token invalid/expired

4. **Logout:**
   - Frontend clears localStorage
   - Redirects to /login

---

## User Roles & Permissions

### Customer
- View and purchase policies
- Create support tickets and claims
- View own policies and payments
- View notifications

### Admin
- Full system access
- Manage policies (CRUD)
- View dashboard metrics
- Manage users
- Create specialists
- View all reports

### ClaimsSpecialist
- View and manage claims
- Approve/reject claims
- Add comments to tickets

### SupportSpecialist
- View and manage support tickets
- Add comments to tickets

---

## Testing the Integration

### Prerequisites
1. Start all backend services:
   - IdentityService (port 5001)
   - TicketService (port 5002)
   - PolicyService (port 5003)
   - PaymentService (port 5004)
   - NotificationService (port 5005)
   - AdminService (port 5006)
   - API Gateway (port 5000)

2. Start frontend:
   ```bash
   cd Frontend
   npm install
   npm start
   ```

### Test Scenarios

1. **User Registration & Login:**
   - Register new customer
   - Verify email (optional)
   - Login and check dashboard

2. **Policy Management:**
   - Login as admin
   - Create new policy
   - Edit policy
   - Login as customer and purchase policy

3. **Claims Processing:**
   - Login as customer
   - Create claim ticket
   - Login as admin/claims specialist
   - Review and approve/reject claim

4. **Admin Operations:**
   - View dashboard metrics
   - Manage users
   - Create specialists
   - View reports

---

## Error Handling

All services implement consistent error handling:
- Display error messages from API responses
- Show loading states during API calls
- Handle network errors gracefully
- Validate form inputs before submission

---

## Next Steps (Optional Enhancements)

1. **Search & Filtering:**
   - Add search functionality to policies, tickets, users
   - Implement backend search endpoints

2. **Real-time Updates:**
   - Integrate SignalR for real-time notifications
   - Live dashboard updates

3. **File Upload:**
   - Implement document upload for claims
   - Store files in blob storage

4. **Email Service:**
   - Integrate actual email service for verification codes
   - Send notifications via email

5. **Payment Gateway:**
   - Integrate real payment processor (Stripe, PayPal)
   - Handle payment webhooks

6. **Advanced Reporting:**
   - Export reports to PDF/Excel
   - Custom date range filtering
   - Charts and visualizations

---

## Summary

✅ **Backend:** All critical endpoints implemented including email verification and policy CRUD
✅ **Frontend:** Complete service layer with HTTP client and interceptors
✅ **Pages:** All 8 pages fully implemented with API integration
✅ **Authentication:** JWT-based auth with role-based guards
✅ **Error Handling:** Consistent error handling across all components
✅ **Type Safety:** TypeScript models for all API responses

The insurance portal is now fully connected with all API endpoints integrated between frontend and backend!
