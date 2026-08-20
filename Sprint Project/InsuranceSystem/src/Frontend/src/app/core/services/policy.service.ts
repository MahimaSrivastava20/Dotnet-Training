import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../../environments/environment';
import { ApiResponse, Policy, CustomerPolicy } from '../models/api-response.model';

@Injectable({
  providedIn: 'root'
})
export class PolicyService {
  private apiUrl = `${environment.apiUrl}/policies`;

  constructor(private http: HttpClient) {}

  getAllPolicies(): Observable<ApiResponse<Policy[]>> {
    return this.http.get<ApiResponse<Policy[]>>(this.apiUrl);
  }

  getPolicyById(id: string): Observable<ApiResponse<Policy>> {
    return this.http.get<ApiResponse<Policy>>(`${this.apiUrl}/${id}`);
  }

  createPolicy(policy: Partial<Policy>): Observable<ApiResponse<Policy>> {
    return this.http.post<ApiResponse<Policy>>(this.apiUrl, policy);
  }

  updatePolicy(id: string, policy: Partial<Policy>): Observable<ApiResponse<Policy>> {
    return this.http.put<ApiResponse<Policy>>(`${this.apiUrl}/${id}`, policy);
  }

  deletePolicy(id: string): Observable<ApiResponse> {
    return this.http.delete<ApiResponse>(`${this.apiUrl}/${id}`);
  }

  purchasePolicy(policyId: string): Observable<ApiResponse<CustomerPolicy>> {
    return this.http.post<ApiResponse<CustomerPolicy>>(`${this.apiUrl}/purchase`, {
      policyId
    });
  }

  renewPolicy(customerPolicyId: string): Observable<ApiResponse<CustomerPolicy>> {
    return this.http.post<ApiResponse<CustomerPolicy>>(`${this.apiUrl}/renew/${customerPolicyId}`, {});
  }

  getMyPolicies(): Observable<ApiResponse<CustomerPolicy[]>> {
    return this.http.get<ApiResponse<CustomerPolicy[]>>(`${this.apiUrl}/my-policies`);
  }

  deductClaim(customerPolicyId: string, amount: number): Observable<ApiResponse> {
    return this.http.post<ApiResponse>(`${this.apiUrl}/customer/${customerPolicyId}/deduct-claim`, { amount });
  }
}
