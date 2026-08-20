export interface ApiResponse<T = any> {
  success: boolean;
  message?: string;
  data?: T;
}

export interface AuthResponse {
  token: string;
  role: string;
  userId: string;
  name: string;
  expiresAt: string;
}

export interface User {
  userId: string;
  name: string;
  email: string;
  role: string;
  isActive: boolean;
  createdAt: string;
}

export interface Policy {
  policyId: string;
  name: string;
  type: string;
  premium: number;
  coverageAmount: number;
  coverageDetails: string;
  terms: string;
  isActive: boolean;
  createdAt: string;
}

export interface CustomerPolicy {
  customerPolicyId: string;
  policyId: string;
  policyName: string;
  policyType: string;
  premium: number;
  coverageAmount: number;
  remainingCoverageAmount: number;
  startDate: string;
  endDate: string;
  status: string;
  createdAt: string;
}

export interface Ticket {
  ticketId: string;
  title: string;
  description: string;
  type: string;
  status: string;
  customerId: string;
  assignedTo?: string;
  policyId?: string;
  createdAt: string;
  updatedAt: string;
  claimDetails?: ClaimDetails;
  comments: Comment[];
}

export interface ClaimDetails {
  claimId: string;
  claimAmount: number;
  documents: string;
  approvalStatus: string;
  rejectionReason?: string;
}

export interface Comment {
  commentId: string;
  userId: string;
  userName: string;
  message: string;
  createdAt: string;
}

export interface Payment {
  paymentId: string;
  amount: number;
  status: string;
  customerId: string;
  policyId: string;
  createdAt: string;
}

export interface Notification {
  notificationId: string;
  userId: string;
  message: string;
  type: string;
  isRead: boolean;
  createdAt: string;
}

export interface DashboardMetrics {
  totalUsers: number;
  totalPolicies: number;
  totalTickets: number;
  totalClaims: number;
  totalPayments: number;
  pendingClaims: number;
  activePolicies: number;
  totalQueries: number;
}
