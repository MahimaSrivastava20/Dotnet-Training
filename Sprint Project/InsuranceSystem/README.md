# Insurance Policy & Claims Support System

A distributed microservices-based insurance system built with **.NET 10**, **Entity Framework Core**, **SQL Server**, **JWT Authentication**, **RabbitMQ**, and **Ocelot API Gateway**.

---

## Architecture

```
┌─────────────────────────────────────────────────────┐
│                   Angular Frontend                   │  :4200
└──────────────────────────┬──────────────────────────┘
                           │
┌──────────────────────────▼──────────────────────────┐
│                  Ocelot API Gateway                  │  :5000
│              JWT Validation · Routing                │
└──┬──────┬──────┬──────┬──────┬──────┬───────────────┘
   │      │      │      │      │      │
  :5001  :5002  :5003  :5004  :5005  :5006
Identity Ticket Policy Payment Notif  Admin
Service Service Service Service ation  CQRS
                                Svc   Svc
   │      │      │      │      │      │
   └──────┴──────┴──────┴──────┴──────┘
                     │
              ┌──────▼──────┐
              │   RabbitMQ  │  :5672 / :15672
              └─────────────┘
```

---

## Prerequisites

| Tool | Version | Notes |
|---|---|---|
| .NET SDK | 10.0+ | ✅ Installed |
| SQL Server / LocalDB | Any | LocalDB used by default |
| RabbitMQ | 3.x+ | See setup below |
| Node.js | 20+ | For Angular frontend |
| Angular CLI | 19+ | `npm i -g @angular/cli` |

---

## Quick Start

### 1. Start RabbitMQ

**Option A — Docker:**
```bash
docker-compose up -d rabbitmq
```

**Option B — Windows Installer:**
Download from https://www.rabbitmq.com/install-windows.html  
RabbitMQ runs on `localhost:5672` by default.

### 2. Run All Services

Open 7 terminal windows and run each:

```powershell
# Terminal 1 - Identity Service
cd src\Services\IdentityService
dotnet run

# Terminal 2 - Ticket Service
cd src\Services\TicketService
dotnet run

# Terminal 3 - Policy Service
cd src\Services\PolicyService
dotnet run

# Terminal 4 - Payment Service
cd src\Services\PaymentService
dotnet run

# Terminal 5 - Notification Service
cd src\Services\NotificationService
dotnet run

# Terminal 6 - Admin Service
cd src\Services\AdminService
dotnet run

# Terminal 7 - API Gateway
cd src\Gateway\ApiGateway
dotnet run
```

Databases are created automatically on first run via `EnsureCreated()`.

---

## Service Ports & URLs

| Service | Port | Base URL |
|---|---|---|
| API Gateway | 5000 | http://localhost:5000 |
| Identity Service | 5001 | http://localhost:5001 |
| Ticket Service | 5002 | http://localhost:5002 |
| Policy Service | 5003 | http://localhost:5003 |
| Payment Service | 5004 | http://localhost:5004 |
| Notification Service | 5005 | http://localhost:5005 |
| Admin Service | 5006 | http://localhost:5006 |
| RabbitMQ Management | 15672 | http://localhost:15672 |

---

## Default Admin Credentials

```
Email:    admin@insurance.com
Password: Admin@123
```

---

## API Endpoints (via Gateway at :5000)

### Auth (No JWT required)
```
POST /api/auth/register       → Customer self-registration
POST /api/auth/login          → Returns JWT token
```

### Identity Admin (Admin JWT required)
```
POST /api/identity/admin/create-claims-specialist
POST /api/identity/admin/create-support-specialist
GET  /api/identity/admin/users
PUT  /api/identity/admin/users/{id}/toggle-status
```

### Tickets (JWT required)
```
POST /api/tickets                        → Create ticket [Customer]
GET  /api/tickets                        → List tickets (role-filtered)
GET  /api/tickets/{id}                   → Get ticket detail
PUT  /api/tickets/{id}/status            → Update status [Specialist/Admin]
POST /api/tickets/{id}/assign            → Assign ticket [Admin]
POST /api/tickets/{id}/comments          → Add comment
GET  /api/tickets/{id}/comments          → View thread
POST /api/tickets/{id}/approve           → Approve claim [ClaimsSpecialist/Admin]
POST /api/tickets/{id}/reject            → Reject claim [ClaimsSpecialist/Admin]
```

### Policies (JWT required)
```
GET  /api/policies                       → Browse all policies
GET  /api/policies/{id}                  → Get policy detail
POST /api/policies                       → Create policy [Admin]
POST /api/policies/purchase              → Purchase policy [Customer]
POST /api/policies/renew/{id}            → Renew policy [Customer]
GET  /api/policies/my-policies           → My policies [Customer]
```

### Payments (JWT required)
```
POST /api/payments                       → Process payment [Customer]
GET  /api/payments/my                    → My payments [Customer]
GET  /api/payments/{id}                  → Get payment
```

### Notifications (JWT required)
```
GET  /api/notifications/my              → My notifications
PUT  /api/notifications/{id}/read       → Mark as read
PUT  /api/notifications/read-all        → Mark all as read
```

### Admin Reporting (Admin JWT required)
```
GET  /api/reporting/admin/dashboard
GET  /api/reporting/admin/reports/tickets
GET  /api/reporting/admin/reports/claims
GET  /api/reporting/admin/reports/payments
```

---

## Policy Purchase Saga Flow

```
Customer → POST /api/policies/purchase
    → PolicyService creates CustomerPolicy (Status: PendingPayment)
    
Customer → POST /api/payments (with PolicyId)
    → PaymentService processes payment
    → Publishes: PaymentCompletedEvent → RabbitMQ
    
PolicyService (consumer) receives PaymentCompletedEvent
    → Activates CustomerPolicy (Status: Active)
    → Publishes: PolicyPurchasedEvent → RabbitMQ
    
NotificationService receives PolicyPurchasedEvent
    → Creates notification for customer
```

---

## Claim Processing Flow

```
1. Customer → POST /api/tickets  (Type: "Claim", with ClaimAmount + Documents)
2. Admin assigns ticket to ClaimsSpecialist
3. ClaimsSpecialist reviews → POST /api/tickets/{id}/approve  OR  /reject
4. ClaimApproved/ClaimRejected event published → RabbitMQ
5. NotificationService → Customer receives notification
```

---

## Role-Based Access

| Action | Customer | SupportSpecialist | ClaimsSpecialist | Admin |
|---|:---:|:---:|:---:|:---:|
| Self-register | ✅ | ❌ | ❌ | ❌ |
| Create ticket | ✅ | ❌ | ❌ | ❌ |
| Handle Support tickets | ❌ | ✅ | ❌ | ✅ |
| Handle Claim tickets | ❌ | ❌ | ✅ | ✅ |
| Approve/Reject claims | ❌ | ❌ | ✅ | ✅ |
| Create policies | ❌ | ❌ | ❌ | ✅ |
| Purchase policies | ✅ | ❌ | ❌ | ❌ |
| Create specialists | ❌ | ❌ | ❌ | ✅ |
| View admin dashboard | ❌ | ❌ | ❌ | ✅ |

---

## RabbitMQ Events

| Event | Publisher | Consumers |
|---|---|---|
| `user.registered` | IdentityService | NotificationService |
| `ticket.created` | TicketService | NotificationService |
| `ticket.assigned` | TicketService | NotificationService |
| `ticket.updated` | TicketService | NotificationService |
| `claim.approved` | TicketService | NotificationService |
| `claim.rejected` | TicketService | NotificationService |
| `payment.completed` | PaymentService | PolicyService, NotificationService |
| `policy.purchased` | PolicyService | NotificationService |

---

## Running EF Migrations (optional — EnsureCreated is used by default)

```powershell
# Install EF tools if not installed
dotnet tool install --global dotnet-ef

# Identity
cd src\Services\IdentityService
dotnet ef migrations add Initial
dotnet ef database update

# Ticket
cd src\Services\TicketService
dotnet ef migrations add Initial
dotnet ef database update

# Policy
cd src\Services\PolicyService
dotnet ef migrations add Initial
dotnet ef database update

# Payment
cd src\Services\PaymentService
dotnet ef migrations add Initial
dotnet ef database update

# Notification
cd src\Services\NotificationService
dotnet ef migrations add Initial
dotnet ef database update

# Admin
cd src\Services\AdminService
dotnet ef migrations add Initial
dotnet ef database update
```

---

## Project Structure

```
InsuranceSystem/
├── InsuranceSystem.slnx
├── docker-compose.yml
├── README.md
└── src/
    ├── Shared/
    │   └── SharedLibrary/
    │       ├── Events/          ← All RabbitMQ event classes
    │       ├── Messaging/       ← Publisher + ConsumerBase
    │       └── DTOs/            ← ApiResponse<T>
    ├── Services/
    │   ├── IdentityService/     ← JWT auth, user management, admin seeding
    │   ├── TicketService/       ← Tickets, claims, comments
    │   ├── PolicyService/       ← Policies, purchase saga
    │   ├── PaymentService/      ← Payments, publishes PaymentCompleted
    │   ├── NotificationService/ ← Listens to all events
    │   └── AdminService/        ← CQRS dashboard & reports
    └── Gateway/
        └── ApiGateway/          ← Ocelot routing + JWT validation
```
