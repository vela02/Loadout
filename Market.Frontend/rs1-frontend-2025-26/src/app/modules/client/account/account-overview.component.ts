import { Component, inject, OnInit } from '@angular/core';
import { UsersApiService } from '../../../api-services/users/users-api.service';
import { GetProfileDto } from '../../../api-services/users/users-api.models';
import { OrderStatusHelper } from '../../../api-services/orders/order-status.helper';
import { OrderStatusType } from '../../../api-services/orders/orders-api.models';

@Component({
  selector: 'app-account-overview',
  standalone: false,
  templateUrl: './account-overview.component.html',
  styleUrl: './account-overview.component.scss'
})
export class AccountOverviewComponent implements OnInit {
  private api = inject(UsersApiService);

  profile: GetProfileDto | null = null;
  isLoading = true;
  hasError = false;

  ngOnInit(): void {
    this.loadProfile();
  }

  loadProfile(): void {
    this.isLoading = true;
    this.hasError = false;

    this.api.getProfile().subscribe({
      next: (profile) => {
        this.profile = profile;
        this.isLoading = false;
      },
      error: (err) => {
        console.error('Load profile error:', err);
        this.hasError = true;
        this.isLoading = false;
      }
    });
  }

  /** First letter of the username for the avatar */
  get initial(): string {
    return this.profile?.username?.charAt(0).toUpperCase() ?? '?';
  }

  statusLabel(statusId: number | null): string {
    return OrderStatusHelper.getLabel(statusId as OrderStatusType);
  }

  statusClass(statusId: number | null): string {
    return OrderStatusHelper.getColorClass(statusId as OrderStatusType);
  }

  statusIcon(statusId: number | null): string {
    return OrderStatusHelper.getIcon(statusId as OrderStatusType);
  }
}
