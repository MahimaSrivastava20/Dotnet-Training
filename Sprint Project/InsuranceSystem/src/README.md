# Insurance Portal - Full Stack Application

A comprehensive insurance management system built with Angular frontend and .NET microservices backend.

![Status](https://img.shields.io/badge/status-ready-green)
![Frontend](https://img.shields.io/badge/frontend-Angular%2021-red)
![Backend](https://img.shields.io/badge/backend-.NET%2010-blue)
![Architecture](https://img.shields.io/badge/architecture-microservices-orange)

---

## 🎯 Project Overview

SmartSure is a modern insurance portal that enables customers to browse and purchase insurance policies, submit claims, and manage their accounts. Administrators can manage policies, review claims, and access comprehensive reporting dashboards.

### Key Features

✅ **User Management**
- Customer registration and authentication
- Email verification
- Role-based access control (Customer, Admin, Claims Specialist, Support Specialist)

✅ **Policy Management**
- Browse available insurance policies
- Purchase and renew policies
- Admin CRUD operations for policies
- Multiple policy types (Health, Life, Vehicle, Property)

✅ **Claims Processing**
- Submit insurance claims
- Track claim status
- Admin/Specialist approval workflow
- Comment system for communication

✅ **Payment Processing**
- Secure payment handling
- Payment history tracking
- Integration with policy activation

✅ **Notifications**
- Real-time notifications for important events
- Mark as read functionality
- Event-driven notification system

✅ **Admin Dashboard**
- System metrics and analytics
- User management
- Comprehensive reporting (Tickets, Claims, Payments)
- Specialist account creation

---

## 🏗️ Architecture

### Microservices Backend
- **API Gateway** (Port 5000) - Ocelot-based routing and authentication
- **IdentityService** (Port 5001) - User authentication and management
- **TicketService** (Port 5002) - Support tickets and claims
- **PolicyService** (Port 5003) - Insurance policy management
- **PaymentService** (Port 5004) - Payment processing
- **NotificationService** (Port 5005) - User notifications
- **AdminService** (Port 5006) - Admin dashboard and reporting

### Frontend
- **Angular 21** - Modern reactive UI
- **Tailwind CSS** - Utility-first styling
- **Angular Signals** - State management
- **HTTP Interceptors** - Automatic JWT token injection
- **Route Guards** - Role-based navigation protection

### Database
- **SQL Server** - Database per service pattern
- **Entity Framework Core** - ORM
- **Code-First Migrations** - Database versioning

### Messaging
- **RabbitMQ** - Event-driven communication (optional)
- Asynchronous event processing
- Service-to-service notifications

---

## 📁 Project Structure

```
InsuranceSystem/
├── Frontend/                    # Angular application
│   ├── src/
│   │   ├── app/
│   │   │   ├── core/           # Services, guards, interceptors, models
│   │   │   └── pages/          # Page components
│   │   └── environments/       # Environment configuration
│   └── package.json
│
├── Gateway/
│   └── ApiGateway/             # Ocelot API Gateway
│
├── Services/
│   ├── IdentityService/        # Authentication & Users
│   ├── PolicyService/          # Policy Management
│   ├── TicketService/          # Tickets & Claims
│   ├── PaymentService/         # Payment Processing
│   ├── NotificationService/    # Notifications
│   └── AdminService/           # Admin Dashboard & Reports
│
├── Shared/
│   └── SharedLibrary/          # Common DTOs, Events, Messaging
│
└── Documentation/
    ├── API_INTEGRATION_SUMMARY.md
    ├── QUICK_START_GUIDE.md
    ├── TESTING_GUIDE.md
    ├── ARCHITECTURE_OVERVIEW.md
    └── IMPLEMENTATION_CHECKLIST.md
```

---

## 🚀 Quick Start

### Prerequisites
- .NET 10.0 SDK
- Node.js 18+ and npm
- SQL Server
- RabbitMQ (optional)

### 1. Clone Repository
```bash
git clone <repository-url>
cd InsuranceSystem
```

### 2. Setup Backend

**Update Connection Strings:**
Edit `appsettings.json` in each service:
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=localhost;Database=InsuranceDB;Trusted_Connection=true;TrustServerCertificate=true;"
  }
}
```

**Run Migrations:**
```bash
cd Services/IdentityService
dotnet ef database update

cd ../PolicyService
dotnet ef database update

# Repeat for other services...
```

**Start Services:**
```bash
# Option 1: Visual Studio - Set multiple startup projects and press F5

# Option 2: Terminal - Run each in separate terminal
cd Gateway/ApiGateway && dotnet run
cd Services/IdentityService && dotnet run
cd Services/PolicyService && dotnet run
# ... etc
```

### 3. Setup Frontend

```bash
cd Frontend
npm install
npm start
```

Frontend will run on: http://localhost:4200

### 4. Access Application

**Default Admin Account:**
- Email: `admin@insurance.com`
- Password: `Admin@123`

**Create Customer Account:**
- Navigate to http://localhost:4200/register
- Fill in registration form
- Login and explore features

---

## 📚 Documentation

| Document | Description |
|----------|-------------|
| [API Integration Summary](API_INTEGRATION_SUMMARY.md) | Complete API endpoint mapping and integration details |
| [Quick Start Guide](QUICK_START_GUIDE.md) | Step-by-step setup and configuration |
| [Testing Guide](TESTING_GUIDE.md) | Comprehensive testing scenarios and troubleshooting |
| [Architecture Overview](ARCHITECTURE_OVERVIEW.md) | System architecture diagrams and data flows |
| [Implementation Checklist](IMPLEMENTATION_CHECKLIST.md) | Detailed task tracking and completion status |

---

## 🔐 Security Features

- **JWT Authentication** - Secure token-based authentication
- **Password Hashing** - BCrypt for password security
- **Role-Based Authorization** - Fine-grained access control
- **HTTP Interceptors** - Automatic token management
- **Route Guards** - Client-side navigation protection
- **CORS Configuration** - Controlled cross-origin requests

---

## 🎨 User Interface

### Pages

1. **Welcome Page** (`/`) - Landing page with features and navigation
2. **Login** (`/login`) - User authentication
3. **Register** (`/register`) - New user registration
4. **Verify Email** (`/verify-email`) - Email verification
5. **Customer Dashboard** (`/dashboard`) - Customer overview
6. **Policy Management** (`/policies`) - Browse and manage policies
7. **Admin Dashboard** (`/admin`) - Admin control panel
8. **Claims Review** (`/claims`) - Claims management

### Design System

- **Material Design 3** - Modern, accessible UI components
- **Tailwind CSS** - Utility-first styling
- **Responsive Design** - Mobile-friendly layouts
- **Dark Mode Ready** - Theme support built-in

---

## 🧪 Testing

### Manual Testing
Follow the [Testing Guide](TESTING_GUIDE.md) for comprehensive test scenarios.

### API Testing
Use Swagger UI available at:
- IdentityService: http://localhost:5001/swagger
- PolicyService: http://localhost:5003/swagger
- (Other services have Swagger enabled)

### Test Accounts

**Admin:**
- Email: admin@insurance.com
- Password: Admin@123

**Customer:**
- Create via registration page

---

## 📊 API Endpoints

### Authentication
- `POST /api/auth/register` - Register new user
- `POST /api/auth/login` - Login
- `POST /api/auth/verify-email` - Verify email
- `POST /api/auth/resend-verification` - Resend verification code

### Policies
- `GET /api/policies` - Get all policies
- `POST /api/policies` - Create policy (Admin)
- `PUT /api/policies/{id}` - Update policy (Admin)
- `DELETE /api/policies/{id}` - Delete policy (Admin)
- `POST /api/policies/purchase` - Purchase policy
- `GET /api/policies/my-policies` - Get user's policies

### Tickets/Claims
- `POST /api/tickets` - Create ticket
- `GET /api/tickets` - Get all tickets
- `POST /api/tickets/{id}/approve` - Approve claim
- `POST /api/tickets/{id}/reject` - Reject claim
- `POST /api/tickets/{id}/comments` - Add comment

### Admin
- `GET /api/reporting/admin/dashboard` - Dashboard metrics
- `GET /api/reporting/admin/reports/tickets` - Ticket report
- `GET /api/reporting/admin/reports/claims` - Claim report
- `GET /api/identity/admin/users` - Get all users

[See full API documentation](API_INTEGRATION_SUMMARY.md)

---

## 🛠️ Technology Stack

### Frontend
- Angular 21.2.8
- TypeScript 5.x
- Tailwind CSS 3.x
- RxJS 7.x
- Angular Signals

### Backend
- ASP.NET Core 10.0
- C# 13
- Entity Framework Core 10.0
- Ocelot API Gateway
- RabbitMQ Client
- BCrypt.Net
- JWT Bearer Authentication

### Database
- SQL Server
- Database per Service pattern

### DevOps
- Git for version control
- Docker ready (containers can be added)
- CI/CD ready

---

## 🔄 Event-Driven Architecture

The system uses RabbitMQ for asynchronous communication:

**Events Published:**
- `user.registered` - New user registration
- `policy.purchased` - Policy purchase
- `payment.completed` - Payment processed
- `ticket.created` - New ticket/claim
- `claim.approved` - Claim approved
- `claim.rejected` - Claim rejected

**Event Consumers:**
- NotificationService - Creates notifications for all events
- PolicyService - Activates policies on payment completion
- AdminService - Updates dashboard metrics

---

## 📈 Future Enhancements

### Planned Features
- [ ] Real email service integration (SendGrid, AWS SES)
- [ ] Real payment gateway (Stripe, PayPal)
- [ ] File upload for claim documents
- [ ] Advanced search and filtering
- [ ] Export reports to PDF/Excel
- [ ] Real-time updates with SignalR
- [ ] Mobile app (React Native)
- [ ] Two-factor authentication
- [ ] Audit logging
- [ ] Performance monitoring

### Infrastructure
- [ ] Docker containerization
- [ ] Kubernetes orchestration
- [ ] CI/CD pipeline (GitHub Actions, Azure DevOps)
- [ ] Cloud deployment (Azure, AWS)
- [ ] Load balancing
- [ ] Redis caching
- [ ] Elasticsearch for logging

---

## 🤝 Contributing

1. Fork the repository
2. Create a feature branch (`git checkout -b feature/AmazingFeature`)
3. Commit your changes (`git commit -m 'Add some AmazingFeature'`)
4. Push to the branch (`git push origin feature/AmazingFeature`)
5. Open a Pull Request

---

## 📝 License

This project is licensed under the MIT License - see the LICENSE file for details.

---

## 👥 Team

- **Backend Development** - .NET Microservices
- **Frontend Development** - Angular Application
- **Database Design** - SQL Server Schema
- **DevOps** - Infrastructure & Deployment

---

## 📞 Support

For issues, questions, or contributions:
- Create an issue in the repository
- Check the [Testing Guide](TESTING_GUIDE.md) for troubleshooting
- Review the [API Documentation](API_INTEGRATION_SUMMARY.md)

---

## ✨ Acknowledgments

- Material Design 3 for design system
- Tailwind CSS for styling utilities
- Ocelot for API Gateway
- RabbitMQ for messaging
- Entity Framework Core for ORM

---

## 📊 Project Status

**Current Version:** 1.0.0  
**Status:** Production Ready (Demo)  
**Last Updated:** January 2025

### Completion Status
- ✅ Backend: 100% (All endpoints implemented)
- ✅ Frontend: 100% (All pages connected)
- ✅ Integration: 100% (Full API integration)
- ✅ Documentation: 100% (Comprehensive guides)
- ✅ Testing: Ready for QA

---

**Built with ❤️ using Angular and .NET**
