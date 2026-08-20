# PowerShell script to launch all Insurance System Microservices and Ocelot Gateway
Write-Host "==========================================================" -ForegroundColor Cy
Write-Host " Starting Insurance Policy & Support Microservices System " -ForegroundColor Cy
Write-Host "==========================================================" -ForegroundColor Cy

$root = $PSScriptRoot

Write-Host "1. Launching Identity Microservice on http://localhost:5001..." -ForegroundColor Green
Start-Process dotnet -ArgumentList "run --project `"$root\src\Services\IdentityService\Identity.API`"" -WindowStyle Normal

Write-Host "2. Launching Policy Microservice on http://localhost:5002..." -ForegroundColor Green
Start-Process dotnet -ArgumentList "run --project `"$root\src\Services\PolicyService\Policy.API`"" -WindowStyle Normal

Write-Host "3. Launching Support Ticket Microservice on http://localhost:5003..." -ForegroundColor Green
Start-Process dotnet -ArgumentList "run --project `"$root\src\Services\TicketService\Ticket.API`"" -WindowStyle Normal

Start-Sleep -Seconds 3

Write-Host "4. Launching Ocelot API Gateway on http://localhost:5000..." -ForegroundColor Yellow
Start-Process dotnet -ArgumentList "run --project `"$root\src\Gateway\OcelotGateway`"" -WindowStyle Normal

Write-Host "==========================================================" -ForegroundColor Cy
Write-Host " All microservices launching! " -ForegroundColor Green
Write-Host " - Ocelot API Gateway Swagger UI : http://localhost:5000/swagger" -ForegroundColor White
Write-Host " - Identity API Swagger UI       : http://localhost:5001/swagger" -ForegroundColor White
Write-Host " - Policy API Swagger UI         : http://localhost:5002/swagger" -ForegroundColor White
Write-Host " - Ticket API Swagger UI         : http://localhost:5003/swagger" -ForegroundColor White
Write-Host "==========================================================" -ForegroundColor Cy
