import { Component, inject, signal, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { AuthService } from '../../core/services/auth.service';
import { TicketService } from '../../core/services/ticket.service';
import { Ticket, Comment } from '../../core/models/api-response.model';

@Component({
  selector: 'app-support',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './support.html'
})
export class SupportComponent implements OnInit {
  authService = inject(AuthService);
  private ticketService = inject(TicketService);

  supportQueries = signal<Ticket[]>([]);
  loading = signal<boolean>(true);
  error = signal<string>('');
  success = signal<string>('');

  // Customer: Create Query form
  showCreateForm = signal<boolean>(false);
  newQuery = { title: '', description: '' };

  // View Query Details
  viewingQuery = signal<Ticket | null>(null);
  queryComments = signal<Comment[]>([]);
  newCommentText = signal<string>('');
  loadingComments = signal<boolean>(false);

  // Admin Contact Info
  adminPhone = '+1 (800) 555-0199';
  adminEmail = 'support@smartsure.com';

  ngOnInit() {
    this.loadQueries();
  }

  loadQueries() {
    this.loading.set(true);
    this.ticketService.getAllTickets().subscribe({
      next: (res) => {
        // Filter only 'Support' tickets
        const tickets = res.data || [];
        const supportTickets = tickets.filter(t => t.type === 'Support');
        // Sort by CreatedAt descending
        supportTickets.sort((a, b) => new Date(b.createdAt).getTime() - new Date(a.createdAt).getTime());
        this.supportQueries.set(supportTickets);
        this.loading.set(false);
      },
      error: (err) => {
        this.error.set(err.error?.message || 'Failed to load support queries');
        this.loading.set(false);
      }
    });
  }

  submitQuery() {
    if (!this.newQuery.title || !this.newQuery.description) return;

    this.loading.set(true);
    this.ticketService.createTicket({
      title: this.newQuery.title,
      description: this.newQuery.description,
      type: 'Support'
    }).subscribe({
      next: () => {
        this.success.set('Your query has been submitted successfully.');
        this.showCreateForm.set(false);
        this.newQuery = { title: '', description: '' };
        this.loadQueries();
        setTimeout(() => this.success.set(''), 3000);
      },
      error: (err) => {
        this.error.set(err.error?.message || 'Failed to submit query');
        this.loading.set(false);
        setTimeout(() => this.error.set(''), 3000);
      }
    });
  }

  openQueryDetails(query: Ticket) {
    this.viewingQuery.set(query);
    this.loadComments(query.ticketId);
  }

  closeQueryDetails() {
    this.viewingQuery.set(null);
    this.queryComments.set([]);
    this.newCommentText.set('');
  }

  loadComments(ticketId: string) {
    this.loadingComments.set(true);
    this.ticketService.getComments(ticketId).subscribe({
      next: (res) => {
        this.queryComments.set(res.data || []);
        this.loadingComments.set(false);
      },
      error: () => this.loadingComments.set(false)
    });
  }

  addComment() {
    if (!this.newCommentText().trim() || !this.viewingQuery()) return;
    
    this.loadingComments.set(true);
    this.ticketService.addComment(this.viewingQuery()!.ticketId, this.newCommentText()).subscribe({
      next: (res) => {
        if (res.data) {
          this.queryComments.update(comments => [...comments, res.data!]);
        }
        this.newCommentText.set('');
        this.loadingComments.set(false);
      },
      error: () => this.loadingComments.set(false)
    });
  }

  answerQuery() {
    // Admin answers by adding a comment, then keeping it open or resolving it
    if (!this.newCommentText().trim() || !this.viewingQuery()) return;
    
    this.addComment();
    this.success.set('Response sent to customer.');
    setTimeout(() => this.success.set(''), 3000);
  }

  dismissQuery(ticketId: string) {
    this.updateStatus(ticketId, 'Resolved');
  }

  updateStatus(ticketId: string, status: string) {
    this.loading.set(true);
    this.ticketService.updateTicketStatus(ticketId, status).subscribe({
      next: () => {
        this.success.set(`Query marked as ${status}.`);
        if (this.viewingQuery()?.ticketId === ticketId) {
          this.closeQueryDetails();
        }
        this.loadQueries();
        setTimeout(() => this.success.set(''), 3000);
      },
      error: (err) => {
        this.error.set(err.error?.message || 'Failed to update status');
        this.loading.set(false);
        setTimeout(() => this.error.set(''), 3000);
      }
    });
  }
}
