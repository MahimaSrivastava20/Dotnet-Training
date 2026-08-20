# Quick Start Guide - Insurance Portal

## Prerequisites
- .NET 10.0 SDK
- Node.js 18+ and npm
- SQL Server
- RabbitMQ (optional, for messaging)

---

## Backend Setup

### 1. Update Connection Strings
Update `appsettings.json` in each service with your SQL Server connection string:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=localhost;Database=InsuranceDB;Trusted_Connection=true;TrustServerCertificate=true;"
  }
}
```

### 2. Run Database Migrations
```bash
# IdentityService
cd Services/IdentityService
dotnet ef database update

# PolicyService
cd ../PolicyService
dotnet ef database update

# TicketService
cd ../TicketService
dotnet ef database update

# PaymentService
cd ../PaymentService
dotnet ef database update

# NotificationService
cd ../NotificationService
dotnet ef database update
```

### 3. Start All Services

**Option A: Using Visual Studio**
- Open the solution file
- Set multiple startup projects
- Start all services

**Option B: Using Terminal (separate terminals for each)**
```bash
# Terminal 1 - API Gateway
cd Gateway/ApiGateway
dotnet run

# Terminal 2 - IdentityService
cd Services/IdentityService
dotnet run

# Terminal 3 - PolicyService
cd Services/PolicyService
dotnet run

# Terminal 4 - TicketService
cd Services/TicketService
dotnet run

# Terminal 5 - PaymentService
cd Services/PaymentService
dotnet run

# Terminal 6 - NotificationService
cd Services/NotificationService
dotnet run

# Terminal 7 - AdminService
cd Services/AdminService
dotnet run
```

**Service Ports:**
- API Gateway: http://localhost:5000
- IdentityService: http://localhost:5001
- TicketService: http://localhost:5002
- PolicyService: http://localhost:5003
- PaymentService: http://localhost:5004
- NotificationService: http://localhost:5005
- AdminService: http://localhost:5006

---

## Frontend Setup

### 1. Install Dependencies
```bash
cd Frontend
npm install
```

### 2. Start Development Server
```bash
npm start
# or
ng serve
```

Frontend will run on: http://localhost:4200

---

## Default Admin Account

After running migrations, a default admin account is created:

**Email:** admin@insurance.com  
**Password:** Admin@123

---

## Testing the Application

### 1. Register a New Customer
1. Navigate to http://localhost:4200
2. Click "Register"
3. Fill in the form:
   - Name: John Doe
   - Email: john@example.com
   - Password: Password123
4. Click "Register"
5. (Optional) Verify email with any 6-digit code
6. Login with credentials

### 2. Admin Login
1. Navigate to http://localhost:4200/login
2. Login with admin credentials
3. You'll be redirected to /admin dashboard

### 3. Create a Policy (Admin)
1. Login as admin
2. Navigate to "Policies" page
3. Click "Create Policy"
4. Fill in policy details:
   - Name: Health Insurance Premium
   - Type: Health
   - Premium: 500
   - Coverage Details: Full medical coverage
   - Terms: 1 year term
5. Click "Save"

### 4. Purchase a Policy (Customer)
1. Login as customer
2. Navigate to "Policies" page
3. Click "Purchase" on a policy
4. Complete payment (mock payment for demo)
5. View purchased policy in dashboard

### 5. Create a Claim (Customer)
1. Login as customer
2. Navigate to dashboard
3. Create a support ticket with type "Claim"
4. Fill in claim details:
   - Title: Medical Claim
   - Description: Hospital visit
   - Claim Amount: 1000
   - Documents: (optional)
5. Submit claim

### 6. Review Claims (Admin/Claims Specialist)
1. Login as admin
2. Navigate to "Claims Review" page
3. Select a claim to review
4. Approve or reject with reason
5. Add comments if needed

---

## API Endpoints Quick Reference

### Authentication
- POST /api/auth/register - Register new user
- POST /api/auth/login - Login
- POST /api/auth/verify-email - Verify email
- POST /api/auth/resend-verification - Resend verification code

### Policies
- GET /api/policies - Get all policies
- GET /api/policies/{id} - Get policy by ID
- POST /api/policies - Create policy (Admin)
- PUT /api/policies/{id} - Update policy (Admin)
- DELETE /api/policies/{id} - Delete policy (Admin)
- POST /api/policies/purchase - Purchase policy (Customer)
- GET /api/policies/my-policies - Get my policies

### Tickets/Claims
- POST /api/tickets - Create ticket (Customer)
- GET /api/tickets - Get all tickets
- GET /api/tickets/{id} - Get ticket by ID
- PUT /api/tickets/{id}/status - Update status
- POST /api/tickets/{id}/assign - Assign ticket (Admin)
- POST /api/tickets/{id}/comments - Add comment
- POST /api/tickets/{id}/approve - Approve claim
- POST /api/tickets/{id}/reject - Reject claim

### Payments
- POST /api/payments - Process payment (Customer)
- GET /api/payments/my - Get my payments
- GET /api/payments/{id} - Get payment by ID

### Notifications
- GET /api/notifications/my - Get my notifications
- PUT /api/notifications/{id}/read - Mark as read
- PUT /api/notifications/read-all - Mark all as read

### Admin
- GET /api/reporting/admin/dashboard - Dashboard metrics
- GET /api/reporting/admin/reports/tickets - Ticket report
- GET /api/reporting/admin/reports/claims - Claim report
- GET /api/reporting/admin/reports/payments - Payment report
- GET /api/identity/admin/users - Get all users
- PUT /api/identity/admin/users/{id}/toggle-status - Toggle user status
- POST /api/identity/admin/create-claims-specialist - Create claims specialist
- POST /api/identity/admin/create-support-specialist - Create support specialist

---

## Troubleshooting

### Backend Issues

**Database Connection Failed:**
- Check SQL Server is running
- Verify connection string in appsettings.json
- Ensure database exists (run migrations)

**Port Already in Use:**
- Change port in launchSettings.json
- Update ocelot.json in API Gateway
- Update environment.ts in Frontend

**RabbitMQ Connection Failed:**
- RabbitMQ is optional for demo
- Services will continue without messaging
- Install RabbitMQ if you want event-driven features

### Frontend Issues

**Cannot Connect to API:**
- Verify API Gateway is running on port 5000
- Check environment.ts has correct apiUrl
- Check browser console for CORS errors

**Authentication Not Working:**
- Clear browser localStorage
- Check JWT token in Network tab
- Verify backend JWT configuration matches

**Module Not Found:**
- Run `npm install` again
- Delete node_modules and package-lock.json, then reinstall
- Check Angular version compatibility

---

## Project Structure

```
InsuranceSystem/
├── Gateway/
│   └── ApiGateway/          # API Gateway (Ocelot)
├── Services/
│   ├── IdentityService/     # Authentication & Users
│   ├── PolicyService/       # Policy Management
│   ├── TicketService/       # Tickets & Claims
│   ├── PaymentService/      # Payment Processing
│   ├── NotificationService/ # Notifications
│   └── AdminService/        # Admin Dashboard & Reports
├── Shared/
│   └── SharedLibrary/       # Common DTOs, Events, Messaging
└── Frontend/
    └── src/
        ├── app/
        │   ├── core/        # Services, Guards, Interceptors, Models
        │   └── pages/       # Page Components
        └── environments/    # Environment Configuration
```

---

## Development Tips

### Backend Development
- Use Swagger UI for API testing: http://localhost:5001/swagger
- Check logs in console for debugging
- Use SQL Server Management Studio to inspect database
- RabbitMQ Management UI: http://localhost:15672 (guest/guest)

### Frontend Development
- Use Angular DevTools browser extension
- Check Network tab for API calls
- Use Redux DevTools if state management added
- Hot reload enabled by default

### Testing APIs with Postman/Thunder Client
1. Login to get JWT token
2. Add token to Authorization header: `Bearer {token}`
3. Test protected endpoints

---

## Next Steps

1. **Customize Styling:**
   - Update Tailwind configuration
   - Modify CSS files in each component

2. **Add Features:**
   - Implement search and filtering
   - Add file upload for claims
   - Integrate real payment gateway
   - Add email service

3. **Deploy:**
   - Configure production environment
   - Set up CI/CD pipeline
   - Deploy to cloud (Azure, AWS, etc.)

---

## Support

For issues or questions:
1. Check API_INTEGRATION_SUMMARY.md for detailed API documentation
2. Review backend logs for error messages
3. Check browser console for frontend errors
4. Verify all services are running on correct ports

---

## Summary

✅ Backend: 7 microservices with complete API endpoints  
✅ Frontend: Angular application with full integration  
✅ Authentication: JWT-based with role management  
✅ Database: SQL Server with EF Core migrations  
✅ Messaging: RabbitMQ for event-driven architecture (optional)  

**You're ready to start developing!** 🚀
