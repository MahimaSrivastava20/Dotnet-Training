import { Component, signal, OnInit } from '@angular/core';
import { RouterModule } from '@angular/router';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { AuthService } from '../../core/services/auth.service';
import { AdminService } from '../../core/services/admin.service';
import { DashboardMetrics, User } from '../../core/models/api-response.model';
import { Router } from '@angular/router';


@Component({
  selector: 'app-admin-dashboard',
  imports: [RouterModule, CommonModule, FormsModule],
  templateUrl: './admin-dashboard.html',
  styleUrl: './admin-dashboard.css',
})

export class AdminDashboard implements OnInit {
  //routing
goToTesting() {
  this.router.navigate(['/testing']);
}
  
  
  //----

  //signalValue = signal(0); --ts
  

  //onchange--
  // onChange(event:Event) {
  //   const input = event.target as HTMLInputElement;
  //   alert(input.value);
  // }


  //--------buttin list
  // signalValue = signal("0");
  //   onChange(event: Event) {
  //      const input = event.target as HTMLInputElement;
  //       this.signalValue.set(input.value);
  //    }



  //two way data binding

//   categories: string[] = ['Student', 'Employee', 'Admin'];
//   selectedCategory: string = '';

//   onSelectChange() {
//     // automatically updates due to two-way binding
//   }

//   submit() {
//     const data = {
//       name: 'Abhishek',
//       category: this.selectedCategory
//     };

//     // Simulate API call
//     
//   }


//routing
// goToTesting() {
//   this.router.navigate(['/testing']);
// }

//function
// goToPage(){
//   this.router.navigate(['/helpp']);
// }


  metrics = signal<DashboardMetrics | null>(null);
  users = signal<User[]>([]);
  ticketReport = signal<any[]>([]);
  claimReport = signal<any[]>([]);
  paymentReport = signal<any[]>([]);
  loading = signal(true);
  error = signal('');
  success = signal('');
  
  activeTab = signal<'overview' | 'users' | 'reports'>('overview');
  
  // For creating specialists
  showSpecialistForm = signal(false);
  specialistType = signal<'claims' | 'support'>('claims');
  specialistForm = {
    name: '',
    email: '',
    password: ''
  };
constructor(
  public authService: AuthService,
  private adminService: AdminService,
  private router: Router
) {}

  ngOnInit() {
    this.loadDashboard();
    this.loadUsers();
  }

  loadDashboard() {
    this.loading.set(true);
    this.error.set('');

    this.adminService.getDashboard().subscribe({
      next: (response) => {
        if (response.success && response.data) {
          this.metrics.set(response.data);
        }
        this.loading.set(false);
      },
      error: (err) => {
        this.error.set('Failed to load dashboard');
        this.loading.set(false);
      }
    });
  }

  loadUsers() {
    this.adminService.getAllUsers().subscribe({
      next: (response) => {
        if (response.success && response.data) {
          this.users.set(response.data);
        }
      },
      error: (err) => {
        console.error('Failed to load users:', err);
      }
    });
  }

  loadReports() {
    this.adminService.getTicketReport().subscribe({
      next: (response) => {
        if (response.success && response.data) {
          this.ticketReport.set(response.data);
        }
      },
      error: (err) => console.error('Failed to load ticket report:', err)
    });

    this.adminService.getClaimReport().subscribe({
      next: (response) => {
        if (response.success && response.data) {
          this.claimReport.set(response.data);
        }
      },
      error: (err) => console.error('Failed to load claim report:', err)
    });

    this.adminService.getPaymentReport().subscribe({
      next: (response) => {
        if (response.success && response.data) {
          this.paymentReport.set(response.data);
        }
      },
      error: (err) => console.error('Failed to load payment report:', err)
    });
  }

  toggleUserStatus(userId: string) {
    this.adminService.toggleUserStatus(userId).subscribe({
      next: (response) => {
        if (response.success) {
          this.success.set('User status updated');
          this.loadUsers();
        } else {
          this.error.set(response.message || 'Failed to update user status');
        }
      },
      error: (err) => {
        this.error.set(err.error?.message || 'Failed to update user status');
      }
    });
  }

  openSpecialistForm(type: 'claims' | 'support') {
    this.specialistType.set(type);
    this.specialistForm = { name: '', email: '', password: '' };
    this.showSpecialistForm.set(true);
  }

  closeSpecialistForm() {
    this.showSpecialistForm.set(false);
  }

  createSpecialist() {
    this.loading.set(true);
    this.error.set('');
    this.success.set('');

    const request = this.specialistType() === 'claims'
      ? this.adminService.createClaimsSpecialist(
          this.specialistForm.name,
          this.specialistForm.email,
          this.specialistForm.password
        )
      : this.adminService.createSupportSpecialist(
          this.specialistForm.name,
          this.specialistForm.email,
          this.specialistForm.password
        );

    request.subscribe({
      next: (response) => {
        if (response.success) {
          this.success.set('Specialist created successfully');
          this.closeSpecialistForm();
          this.loadUsers();
        } else {
          this.error.set(response.message || 'Failed to create specialist');
        }
        this.loading.set(false);
      },
      error: (err) => {
        this.error.set(err.error?.message || 'Failed to create specialist');
        this.loading.set(false);
      }
    });
  }

  switchTab(tab: 'overview' | 'users' | 'reports') {
    this.activeTab.set(tab);
    if (tab === 'reports') {
      this.loadReports();
    }
  }

  logout() {
    this.authService.logout();
  }
}

