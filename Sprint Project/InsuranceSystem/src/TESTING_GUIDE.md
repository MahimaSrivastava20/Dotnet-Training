# Testing Guide - Insurance Portal

## Current Status
✅ Frontend is running on http://localhost:4200  
✅ Welcome page is displaying  
⏳ Backend services need to be started  

---

## Step 1: Start Backend Services

### Option A: Using Visual Studio (Recommended)
1. Open the solution file in Visual Studio
2. Right-click on the solution → Properties
3. Set Multiple Startup Projects
4. Select all services and set Action to "Start"
5. Click OK and press F5

### Option B: Using Terminal (7 separate terminals)

**Terminal 1 - API Gateway:**
```bash
cd Gateway/ApiGateway
dotnet run
```
Wait for: `Now listening on: http://localhost:5000`

**Terminal 2 - IdentityService:**
```bash
cd Services/IdentityService
dotnet run
```
Wait for: `Now listening on: http://localhost:5001`

**Terminal 3 - TicketService:**
```bash
cd Services/TicketService
dotnet run
```
Wait for: `Now listening on: http://localhost:5002`

**Terminal 4 - PolicyService:**
```bash
cd Services/PolicyService
dotnet run
```
Wait for: `Now listening on: http://localhost:5003`

**Terminal 5 - PaymentService:**
```bash
cd Services/PaymentService
dotnet run
```
Wait for: `Now listening on: http://localhost:5004`

**Terminal 6 - NotificationService:**
```bash
cd Services/NotificationService
dotnet run
```
Wait for: `Now listening on: http://localhost:5005`

**Terminal 7 - AdminService:**
```bash
cd Services/AdminService
dotnet run
```
Wait for: `Now listening on: http://localhost:5006`

---

## Step 2: Verify Backend is Running

Open your browser and check these URLs:

1. **API Gateway:** http://localhost:5000  
   Should show: "API Gateway is running"

2. **IdentityService Swagger:** http://localhost:5001/swagger  
   Should show: Swagger UI with auth endpoints

3. **PolicyService Swagger:** http://localhost:5003/swagger  
   Should show: Swagger UI with policy endpoints

---

## Step 3: Test Frontend-Backend Integration

### Test 1: Registration Flow

1. **Navigate to Welcome Page:**
   - Go to http://localhost:4200
   - You should see the SmartSure welcome page with features
   - Click "Get Started" button

2. **Register New User:**
   - Fill in the form:
     - Name: `Test User`
     - Email: `test@example.com`
     - Password: `Test123!`
     - Confirm Password: `Test123!`
   - Click "Register Account"
   
3. **Expected Result:**
   - Loading indicator appears
   - Success: Redirects to `/verify-email?email=test@example.com`
   - Error: Shows error message (e.g., "Email already in use")

4. **Verify Email (Optional):**
   - Enter any 6-digit code (e.g., `123456`)
   - Click "Verify"
   - Should show success message and redirect to login

### Test 2: Login Flow

1. **Navigate to Login:**
   - Go to http://localhost:4200/login
   - Or click "Sign In" from welcome page

2. **Login with Admin:**
   - Email: `admin@insurance.com`
   - Password: `Admin@123`
   - Click "Login"

3. **Expected Result:**
   - Loading indicator appears
   - Success: Redirects to `/admin` (Admin Dashboard)
   - You should see dashboard metrics

4. **Login with Customer:**
   - Logout if logged in as admin
   - Login with the test user you created:
     - Email: `test@example.com`
     - Password: `Test123!`
   - Click "Login"

5. **Expected Result:**
   - Success: Redirects to `/dashboard` (Customer Dashboard)
   - You should see your policies, payments, and notifications

### Test 3: Policy Management (Admin)

1. **Login as Admin** (admin@insurance.com / Admin@123)

2. **Navigate to Policies:**
   - Go to http://localhost:4200/policies

3. **Create New Policy:**
   - Click "Create Policy" button
   - Fill in the form:
     - Name: `Premium Health Insurance`
     - Type: `Health`
     - Premium: `500`
     - Coverage Details: `Full medical coverage including hospitalization`
     - Terms: `1 year term with renewal option`
   - Click "Save"

4. **Expected Result:**
   - Success message appears
   - New policy appears in the list
   - Policy has Edit and Delete buttons

5. **Edit Policy:**
   - Click "Edit" on the policy you just created
   - Change Premium to `550`
   - Click "Save"
   - Verify the premium updated

6. **Delete Policy:**
   - Click "Delete" on a policy
   - Confirm deletion
   - Policy should be removed from list (soft deleted)

### Test 4: Policy Purchase (Customer)

1. **Login as Customer** (test@example.com / Test123!)

2. **Navigate to Policies:**
   - Go to http://localhost:4200/policies

3. **Purchase Policy:**
   - Click "Purchase" on any active policy
   - Expected Result: Success message "Policy purchase initiated! Please complete payment."

4. **View Dashboard:**
   - Go to http://localhost:4200/dashboard
   - You should see the purchased policy in "My Policies" section
   - Status should be "PendingPayment"

### Test 5: Claims Management

1. **Create Claim (Customer):**
   - Login as customer
   - Navigate to dashboard
   - Create a support ticket with type "Claim"
   - Fill in claim details:
     - Title: `Medical Claim - Hospital Visit`
     - Description: `Emergency room visit on Jan 15`
     - Claim Amount: `1000`
   - Submit claim

2. **Review Claim (Admin):**
   - Login as admin
   - Navigate to http://localhost:4200/claims
   - You should see the claim in the list
   - Click on the claim to view details

3. **Approve Claim:**
   - Click "Approve" button
   - Confirm approval
   - Expected Result: Claim status changes to "Approved"

4. **Reject Claim:**
   - Create another claim as customer
   - Login as admin and navigate to claims
   - Click "Reject" button
   - Enter rejection reason: `Insufficient documentation`
   - Submit
   - Expected Result: Claim status changes to "Rejected"

### Test 6: Admin Dashboard

1. **Login as Admin**

2. **Navigate to Admin Dashboard:**
   - Go to http://localhost:4200/admin

3. **Overview Tab:**
   - Should display metrics:
     - Total Users
     - Total Policies
     - Total Tickets
     - Total Claims
     - Total Payments
     - Pending Claims
     - Active Policies

4. **Users Tab:**
   - Click "Users" tab
   - Should see list of all users
   - Try toggling user status (Active/Inactive)
   - Try creating a Claims Specialist:
     - Click "Create Claims Specialist"
     - Fill in form
     - Submit

5. **Reports Tab:**
   - Click "Reports" tab
   - Should see three report sections:
     - Ticket Report
     - Claim Report
     - Payment Report
   - Each should display data in table format

---

## Step 4: Browser Console Testing

### Check Network Requests

1. Open Browser DevTools (F12)
2. Go to Network tab
3. Perform any action (e.g., login)
4. Look for API calls:
   - Should see requests to `http://localhost:5000/api/...`
   - Check request headers for `Authorization: Bearer {token}`
   - Check response status codes (200, 201, 400, 401, etc.)

### Check Console for Errors

1. Open Console tab in DevTools
2. Look for any errors (red text)
3. Common issues:
   - CORS errors → Backend not running or CORS not configured
   - 401 Unauthorized → Token expired or invalid
   - 404 Not Found → Endpoint doesn't exist
   - Network error → Backend service not running

### Check Local Storage

1. Open Application tab in DevTools
2. Go to Local Storage → http://localhost:4200
3. Should see:
   - `auth_token`: JWT token string
   - `current_user`: JSON object with user info

---

## Step 5: API Testing with Swagger

### Test Endpoints Directly

1. **Open Swagger UI:**
   - IdentityService: http://localhost:5001/swagger
   - PolicyService: http://localhost:5003/swagger

2. **Test Registration:**
   - Expand POST `/auth/register`
   - Click "Try it out"
   - Enter request body:
     ```json
     {
       "name": "API Test User",
       "email": "apitest@example.com",
       "password": "Test123!"
     }
     ```
   - Click "Execute"
   - Should return 200 with token

3. **Test Login:**
   - Expand POST `/auth/login`
   - Click "Try it out"
   - Enter credentials
   - Click "Execute"
   - Copy the token from response

4. **Test Protected Endpoint:**
   - Click "Authorize" button at top of Swagger UI
   - Enter: `Bearer {paste_token_here}`
   - Click "Authorize"
   - Now try GET `/policies/my-policies`
   - Should return your policies

---

## Common Issues & Solutions

### Issue 1: "Cannot connect to API"
**Symptoms:** Network errors in console, no API responses  
**Solution:**
- Verify API Gateway is running on port 5000
- Check `Frontend/src/environments/environment.ts` has correct apiUrl
- Ensure no firewall blocking localhost connections

### Issue 2: "401 Unauthorized"
**Symptoms:** All API calls return 401  
**Solution:**
- Clear localStorage and login again
- Check token in localStorage is valid
- Verify backend JWT configuration matches

### Issue 3: "CORS Error"
**Symptoms:** "Access-Control-Allow-Origin" error in console  
**Solution:**
- Add CORS configuration to API Gateway
- Ensure frontend URL is allowed in backend CORS policy

### Issue 4: "Database connection failed"
**Symptoms:** Backend services crash on startup  
**Solution:**
- Verify SQL Server is running
- Check connection strings in appsettings.json
- Run database migrations: `dotnet ef database update`

### Issue 5: "Module not found" in Frontend
**Symptoms:** Angular compilation errors  
**Solution:**
- Delete `node_modules` folder
- Delete `package-lock.json`
- Run `npm install` again
- Restart dev server

### Issue 6: "Port already in use"
**Symptoms:** Backend service won't start  
**Solution:**
- Check if another instance is running
- Kill the process using the port
- Or change port in launchSettings.json

---

## Success Criteria

✅ **Frontend:**
- Welcome page loads with navigation
- Registration form submits and redirects
- Login form authenticates and redirects based on role
- Dashboard displays user data
- Policy management works (view, create, edit, delete)
- Claims review works (approve, reject)
- Admin dashboard shows metrics

✅ **Backend:**
- All 7 services running without errors
- API Gateway routes requests correctly
- JWT authentication works
- Database operations succeed
- CORS allows frontend requests

✅ **Integration:**
- Frontend can call all backend endpoints
- Auth token is sent with requests
- Responses are displayed correctly
- Error messages show appropriately
- Loading states work

---

## Next Steps After Testing

1. **If Everything Works:**
   - Congratulations! Your full-stack application is working
   - Review the documentation files for deployment
   - Consider adding more features from the enhancement list

2. **If Issues Found:**
   - Check the Common Issues section above
   - Review browser console for specific errors
   - Check backend logs for error messages
   - Verify all services are running on correct ports

3. **Production Preparation:**
   - Update environment.ts for production API URL
   - Configure production database
   - Set up CI/CD pipeline
   - Add monitoring and logging
   - Implement real email service
   - Integrate real payment gateway

---

## Quick Test Checklist

- [ ] Backend services all running
- [ ] Frontend loads at localhost:4200
- [ ] Can register new user
- [ ] Can login as admin
- [ ] Can login as customer
- [ ] Admin can create policy
- [ ] Customer can purchase policy
- [ ] Customer can create claim
- [ ] Admin can approve/reject claim
- [ ] Admin dashboard shows metrics
- [ ] Notifications appear
- [ ] Logout works
- [ ] Route guards prevent unauthorized access

---

## Support

If you encounter issues not covered here:
1. Check API_INTEGRATION_SUMMARY.md for endpoint details
2. Review QUICK_START_GUIDE.md for setup instructions
3. Check ARCHITECTURE_OVERVIEW.md for system design
4. Review backend logs for error details
5. Check browser console for frontend errors

**Happy Testing!** 🚀
