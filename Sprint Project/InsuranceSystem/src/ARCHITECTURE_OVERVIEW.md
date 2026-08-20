# Insurance Portal - Architecture Overview

## System Architecture

```
┌─────────────────────────────────────────────────────────────────┐
│                         Frontend (Angular)                       │
│                      http://localhost:4200                       │
│                                                                   │
│  ┌──────────────┐  ┌──────────────┐  ┌──────────────┐          │
│  │   Welcome    │  │   Register   │  │    Login     │          │
│  │     Page     │  │     Page     │  │     Page     │          │
│  └──────────────┘  └──────────────┘  └──────────────┘          │
│                                                                   │
│  ┌──────────────┐  ┌──────────────┐  ┌──────────────┐          │
│  │   Customer   │  │   Policy     │  │    Admin     │          │
│  │  Dashboard   │  │  Management  │  │  Dashboard   │          │
│  └──────────────┘  └──────────────┘  └──────────────┘          │
│                                                                   │
│  ┌──────────────┐  ┌──────────────┐                             │
│  │   Claims     │  │ Verify Email │                             │
│  │   Review     │  │     Page     │                             │
│  └──────────────┘  └──────────────┘                             │
│                                                                   │
│  ┌─────────────────────────────────────────────────────────┐   │
│  │              Core Services Layer                         │   │
│  │  • AuthService      • PolicyService                      │   │
│  │  • TicketService    • PaymentService                     │   │
│  │  • NotificationService  • AdminService                   │   │
│  └─────────────────────────────────────────────────────────┘   │
│                                                                   │
│  ┌─────────────────────────────────────────────────────────┐   │
│  │         HTTP Interceptor (JWT Token Injection)           │   │
│  └─────────────────────────────────────────────────────────┘   │
└───────────────────────────┬─────────────────────────────────────┘
                            │
                            │ HTTP/REST API
                            │ Authorization: Bearer {token}
                            ▼
┌─────────────────────────────────────────────────────────────────┐
│                    API Gateway (Ocelot)                          │
│                   http://localhost:5000                          │
│                                                                   │
│  ┌─────────────────────────────────────────────────────────┐   │
│  │              Route Configuration                         │   │
│  │  /api/auth/*          → IdentityService:5001            │   │
│  │  /api/identity/admin/* → IdentityService:5001           │   │
│  │  /api/policies/*      → PolicyService:5003              │   │
│  │  /api/tickets/*       → TicketService:5002              │   │
│  │  /api/payments/*      → PaymentService:5004             │   │
│  │  /api/notifications/* → NotificationService:5005        │   │
│  │  /api/reporting/*     → AdminService:5006               │   │
│  └─────────────────────────────────────────────────────────┘   │
│                                                                   │
│  ┌─────────────────────────────────────────────────────────┐   │
│  │         JWT Authentication Middleware                    │   │
│  └─────────────────────────────────────────────────────────┘   │
└───────────────────────────┬─────────────────────────────────────┘
                            │
                            │ Routes to Microservices
                            ▼
┌─────────────────────────────────────────────────────────────────┐
│                      Microservices Layer                         │
│                                                                   │
│  ┌──────────────────┐  ┌──────────────────┐                    │
│  │ IdentityService  │  │  PolicyService   │                    │
│  │   Port: 5001     │  │   Port: 5003     │                    │
│  │                  │  │                  │                    │
│  │ • Register       │  │ • Get Policies   │                    │
│  │ • Login          │  │ • Create Policy  │                    │
│  │ • Verify Email   │  │ • Update Policy  │                    │
│  │ • User Mgmt      │  │ • Delete Policy  │                    │
│  │ • Create Staff   │  │ • Purchase       │                    │
│  └────────┬─────────┘  └────────┬─────────┘                    │
│           │                     │                               │
│  ┌────────┴─────────┐  ┌───────┴──────────┐                   │
│  │  TicketService   │  │  PaymentService  │                    │
│  │   Port: 5002     │  │   Port: 5004     │                    │
│  │                  │  │                  │                    │
│  │ • Create Ticket  │  │ • Process Pay    │                    │
│  │ • Get Tickets    │  │ • Get Payments   │                    │
│  │ • Approve Claim  │  │ • Payment Status │                    │
│  │ • Reject Claim   │  │                  │                    │
│  │ • Comments       │  │                  │                    │
│  └────────┬─────────┘  └────────┬─────────┘                    │
│           │                     │                               │
│  ┌────────┴──────────┐  ┌──────┴──────────┐                   │
│  │NotificationService│  │  AdminService    │                   │
│  │   Port: 5005      │  │   Port: 5006     │                   │
│  │                   │  │                  │                    │
│  │ • Get Notifs      │  │ • Dashboard      │                   │
│  │ • Mark Read       │  │ • Reports        │                   │
│  │ • Create Notif    │  │ • Analytics      │                   │
│  └───────────────────┘  └──────────────────┘                   │
│                                                                   │
└───────────────────────────┬─────────────────────────────────────┘
                            │
                            │ Database Connections
                            ▼
┌─────────────────────────────────────────────────────────────────┐
│                      Database Layer (SQL Server)                 │
│                                                                   │
│  ┌──────────────┐  ┌──────────────┐  ┌──────────────┐          │
│  │  IdentityDB  │  │  PolicyDB    │  │  TicketDB    │          │
│  │              │  │              │  │              │          │
│  │ • Users      │  │ • Policies   │  │ • Tickets    │          │
│  │              │  │ • Customer   │  │ • Claims     │          │
│  │              │  │   Policies   │  │ • Comments   │          │
│  └──────────────┘  └──────────────┘  └──────────────┘          │
│                                                                   │
│  ┌──────────────┐  ┌──────────────┐                             │
│  │  PaymentDB   │  │NotificationDB│                             │
│  │              │  │              │                             │
│  │ • Payments   │  │ • Notifs     │                             │
│  └──────────────┘  └──────────────┘                             │
└─────────────────────────────────────────────────────────────────┘

┌─────────────────────────────────────────────────────────────────┐
│                  Message Queue (RabbitMQ - Optional)             │
│                                                                   │
│  Event Bus for Asynchronous Communication:                       │
│  • user.registered      → NotificationService                    │
│  • policy.purchased     → NotificationService, PaymentService    │
│  • payment.completed    → PolicyService, NotificationService     │
│  • ticket.created       → NotificationService                    │
│  • claim.approved       → NotificationService, PaymentService    │
│  • claim.rejected       → NotificationService                    │
└─────────────────────────────────────────────────────────────────┘
```

---

## Data Flow Examples

### 1. User Registration Flow

```
┌─────────┐     ┌─────────┐     ┌──────────┐     ┌──────────┐
│ Browser │────▶│ Angular │────▶│ Gateway  │────▶│ Identity │
│         │     │ Service │     │          │     │ Service  │
└─────────┘     └─────────┘     └──────────┘     └────┬─────┘
                                                       │
                                                       ▼
                                                  ┌──────────┐
                                                  │ Identity │
                                                  │    DB    │
                                                  └────┬─────┘
                                                       │
                                                       ▼
                                                  ┌──────────┐
                                                  │ RabbitMQ │
                                                  │  Event   │
                                                  └────┬─────┘
                                                       │
                                                       ▼
                                                  ┌──────────┐
                                                  │Notification│
                                                  │ Service  │
                                                  └──────────┘
```

**Steps:**
1. User fills registration form
2. Angular AuthService calls POST /api/auth/register
3. API Gateway routes to IdentityService
4. IdentityService creates user in database
5. Publishes "user.registered" event to RabbitMQ
6. NotificationService consumes event and creates welcome notification
7. JWT token returned to frontend
8. Token stored in localStorage

---

### 2. Policy Purchase Flow

```
┌─────────┐     ┌─────────┐     ┌──────────┐     ┌──────────┐
│Customer │────▶│ Policy  │────▶│ Gateway  │────▶│ Policy   │
│  Page   │     │ Service │     │          │     │ Service  │
└─────────┘     └─────────┘     └──────────┘     └────┬─────┘
                                                       │
                                                       ▼
                                                  ┌──────────┐
                                                  │ Policy   │
                                                  │    DB    │
                                                  └────┬─────┘
                                                       │
                                                       ▼
                                                  ┌──────────┐
                                                  │ Payment  │
                                                  │ Service  │
                                                  └────┬─────┘
                                                       │
                                                       ▼
                                                  ┌──────────┐
                                                  │ Payment  │
                                                  │    DB    │
                                                  └────┬─────┘
                                                       │
                                                       ▼
                                                  ┌──────────┐
                                                  │ RabbitMQ │
                                                  │  Event   │
                                                  └────┬─────┘
                                                       │
                                                       ▼
                                                  ┌──────────┐
                                                  │ Policy   │
                                                  │ Activated│
                                                  └──────────┘
```

**Steps:**
1. Customer clicks "Purchase" on policy
2. PolicyService creates CustomerPolicy with status "PendingPayment"
3. Customer completes payment via PaymentService
4. PaymentService publishes "payment.completed" event
5. PolicyService consumes event and activates policy
6. NotificationService sends confirmation notification

---

### 3. Claim Approval Flow

```
┌─────────┐     ┌─────────┐     ┌──────────┐     ┌──────────┐
│ Claims  │────▶│ Ticket  │────▶│ Gateway  │────▶│ Ticket   │
│ Review  │     │ Service │     │          │     │ Service  │
└─────────┘     └─────────┘     └──────────┘     └────┬─────┘
                                                       │
                                                       ▼
                                                  ┌──────────┐
                                                  │ Ticket   │
                                                  │    DB    │
                                                  └────┬─────┘
                                                       │
                                                       ▼
                                                  ┌──────────┐
                                                  │ RabbitMQ │
                                                  │  Event   │
                                                  └────┬─────┘
                                                       │
                                    ┌──────────────────┴──────────────────┐
                                    ▼                                     ▼
                              ┌──────────┐                          ┌──────────┐
                              │Notification│                        │ Payment  │
                              │ Service  │                          │ Service  │
                              └──────────┘                          └──────────┘
```

**Steps:**
1. Claims specialist reviews claim
2. Clicks "Approve" button
3. TicketService updates claim status to "Approved"
4. Publishes "claim.approved" event
5. NotificationService notifies customer
6. PaymentService processes claim payment

---

## Technology Stack

### Frontend
- **Framework:** Angular 21.2.8
- **Language:** TypeScript
- **Styling:** Tailwind CSS
- **HTTP Client:** Angular HttpClient
- **State Management:** Angular Signals
- **Routing:** Angular Router with Guards

### Backend
- **Framework:** ASP.NET Core 10.0
- **Language:** C# 13
- **API Gateway:** Ocelot
- **Authentication:** JWT Bearer Tokens
- **ORM:** Entity Framework Core
- **Database:** SQL Server
- **Message Queue:** RabbitMQ
- **Patterns:** CQRS (AdminService), Repository Pattern

### Infrastructure
- **Architecture:** Microservices
- **Communication:** REST APIs, Event-Driven Messaging
- **Database Strategy:** Database per Service
- **API Gateway:** Centralized routing and authentication

---

## Security Architecture

```
┌─────────────────────────────────────────────────────────────────┐
│                        Security Layers                           │
│                                                                   │
│  1. Frontend Layer                                               │
│     • Route Guards (authGuard, adminGuard, customerGuard)       │
│     • Token stored in localStorage                               │
│     • HTTP Interceptor adds Authorization header                 │
│                                                                   │
│  2. API Gateway Layer                                            │
│     • JWT Token Validation                                       │
│     • CORS Configuration                                         │
│     • Rate Limiting (configurable)                               │
│                                                                   │
│  3. Service Layer                                                │
│     • [Authorize] attributes on controllers                      │
│     • Role-based authorization (Admin, Customer, Specialist)     │
│     • Claims-based identity                                      │
│                                                                   │
│  4. Data Layer                                                   │
│     • Password hashing (BCrypt)                                  │
│     • SQL injection prevention (EF Core parameterized queries)   │
│     • Soft deletes for data integrity                            │
└─────────────────────────────────────────────────────────────────┘
```

---

## Scalability Considerations

### Horizontal Scaling
- Each microservice can be scaled independently
- Stateless services enable easy load balancing
- Database per service allows independent scaling

### Caching Strategy (Future)
- Redis for session management
- Response caching in API Gateway
- Client-side caching with service workers

### Load Balancing (Future)
- Multiple instances of each service
- Load balancer in front of API Gateway
- Database read replicas for reporting

---

## Monitoring & Logging (Future Enhancements)

```
┌─────────────────────────────────────────────────────────────────┐
│                    Observability Stack                           │
│                                                                   │
│  ┌──────────────┐  ┌──────────────┐  ┌──────────────┐          │
│  │   Logging    │  │   Metrics    │  │   Tracing    │          │
│  │              │  │              │  │              │          │
│  │ • Serilog    │  │ • Prometheus │  │ • OpenTelemetry│        │
│  │ • ELK Stack  │  │ • Grafana    │  │ • Jaeger     │          │
│  └──────────────┘  └──────────────┘  └──────────────┘          │
│                                                                   │
│  ┌──────────────────────────────────────────────────────────┐  │
│  │              Health Checks & Alerts                       │  │
│  │  • Service health endpoints                               │  │
│  │  • Database connectivity checks                           │  │
│  │  • RabbitMQ connection monitoring                         │  │
│  └──────────────────────────────────────────────────────────┘  │
└─────────────────────────────────────────────────────────────────┘
```

---

## Deployment Architecture (Production)

```
┌─────────────────────────────────────────────────────────────────┐
│                         Cloud Provider                           │
│                      (Azure / AWS / GCP)                         │
│                                                                   │
│  ┌──────────────────────────────────────────────────────────┐  │
│  │                    Load Balancer                          │  │
│  └────────────────────────┬─────────────────────────────────┘  │
│                            │                                     │
│  ┌─────────────────────────┴──────────────────────────────┐    │
│  │              Container Orchestration                     │    │
│  │              (Kubernetes / Docker Swarm)                 │    │
│  │                                                           │    │
│  │  ┌─────────┐  ┌─────────┐  ┌─────────┐  ┌─────────┐   │    │
│  │  │Frontend │  │ Gateway │  │Identity │  │ Policy  │   │    │
│  │  │  Pod    │  │  Pod    │  │  Pod    │  │  Pod    │   │    │
│  │  └─────────┘  └─────────┘  └─────────┘  └─────────┘   │    │
│  │                                                           │    │
│  │  ┌─────────┐  ┌─────────┐  ┌─────────┐  ┌─────────┐   │    │
│  │  │ Ticket  │  │ Payment │  │  Notif  │  │  Admin  │   │    │
│  │  │  Pod    │  │  Pod    │  │  Pod    │  │  Pod    │   │    │
│  │  └─────────┘  └─────────┘  └─────────┘  └─────────┘   │    │
│  └───────────────────────────────────────────────────────┘    │
│                                                                   │
│  ┌──────────────────────────────────────────────────────────┐  │
│  │              Managed Database Service                     │  │
│  │              (Azure SQL / RDS / Cloud SQL)                │  │
│  └──────────────────────────────────────────────────────────┘  │
│                                                                   │
│  ┌──────────────────────────────────────────────────────────┐  │
│  │              Managed Message Queue                        │  │
│  │              (Azure Service Bus / Amazon MQ)              │  │
│  └──────────────────────────────────────────────────────────┘  │
└─────────────────────────────────────────────────────────────────┘
```

---

## Summary

This insurance portal implements a modern microservices architecture with:

✅ **Separation of Concerns** - Each service has a single responsibility  
✅ **Scalability** - Services can scale independently  
✅ **Resilience** - Failure in one service doesn't affect others  
✅ **Maintainability** - Clear boundaries and well-defined APIs  
✅ **Security** - Multi-layer security with JWT authentication  
✅ **Event-Driven** - Asynchronous communication via message queue  
✅ **Modern Frontend** - Angular with reactive state management  

The architecture is production-ready and can be deployed to any cloud provider with minimal modifications.
