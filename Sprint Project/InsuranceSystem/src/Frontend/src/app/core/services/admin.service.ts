import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../../environments/environment';
import { ApiResponse, DashboardMetrics, User } from '../models/api-response.model';

@Injectable({
  providedIn: 'root'
})
export class AdminService {
  private apiUrl = `${environment.apiUrl}/reporting`;
  private identityUrl = `${environment.apiUrl}/identity/admin`;

  constructor(private http: HttpClient) {}

  getDashboard(): Observable<ApiResponse<DashboardMetrics>> {
    return this.http.get<ApiResponse<DashboardMetrics>>(`${this.apiUrl}/dashboard`);
  }

  getTicketReport(from?: string, to?: string): Observable<ApiResponse<any[]>> {
    let url = `${this.apiUrl}/reports/tickets`;
    const params = new URLSearchParams();
    if (from) params.append('from', from);
    if (to) params.append('to', to);
    if (params.toString()) url += `?${params.toString()}`;
    return this.http.get<ApiResponse<any[]>>(url);
  }

  getClaimReport(from?: string, to?: string): Observable<ApiResponse<any[]>> {
    let url = `${this.apiUrl}/reports/claims`;
    const params = new URLSearchParams();
    if (from) params.append('from', from);
    if (to) params.append('to', to);
    if (params.toString()) url += `?${params.toString()}`;
    return this.http.get<ApiResponse<any[]>>(url);
  }

  getPaymentReport(from?: string, to?: string): Observable<ApiResponse<any[]>> {
    let url = `${this.apiUrl}/reports/payments`;
    const params = new URLSearchParams();
    if (from) params.append('from', from);
    if (to) params.append('to', to);
    if (params.toString()) url += `?${params.toString()}`;
    return this.http.get<ApiResponse<any[]>>(url);
  }

  getAllUsers(): Observable<ApiResponse<User[]>> {
    return this.http.get<ApiResponse<User[]>>(`${this.identityUrl}/users`);
  }

  toggleUserStatus(userId: string): Observable<ApiResponse> {
    return this.http.put<ApiResponse>(`${this.identityUrl}/users/${userId}/toggle-status`, {});
  }

  createClaimsSpecialist(name: string, email: string, password: string): Observable<ApiResponse<User>> {
    return this.http.post<ApiResponse<User>>(`${this.identityUrl}/create-claims-specialist`, {
      name,
      email,
      password
    });
  }

  createSupportSpecialist(name: string, email: string, password: string): Observable<ApiResponse<User>> {
    return this.http.post<ApiResponse<User>>(`${this.identityUrl}/create-support-specialist`, {
      name,
      email,
      password
    });
  }
}
