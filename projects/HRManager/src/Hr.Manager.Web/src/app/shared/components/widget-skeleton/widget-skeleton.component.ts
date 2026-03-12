import { Component, Input } from '@angular/core';
import { CommonModule } from '@angular/common';
import { MatCardModule } from '@angular/material/card';
import { SkeletonLoaderComponent } from '../skeleton-loader/skeleton-loader.component';

export type WidgetSkeletonType = 'list' | 'stats' | 'clock' | 'chart' | 'calendar';

@Component({
  selector: 'app-widget-skeleton',
  standalone: true,
  imports: [CommonModule, MatCardModule, SkeletonLoaderComponent],
  template: `
    <mat-card>
      @if (title) {
        <mat-card-header>
          <mat-card-title>{{ title }}</mat-card-title>
          @if (subtitle) {
            <mat-card-subtitle>{{ subtitle }}</mat-card-subtitle>
          }
        </mat-card-header>
      } @else {
        <mat-card-header>
          <app-skeleton-loader type="text" width="150px" height="24px"></app-skeleton-loader>
        </mat-card-header>
      }
      <mat-card-content>
        @switch (type) {
          @case ('list') {
            <div class="skeleton-list">
              @for (item of [1,2,3,4]; track item) {
                <div class="skeleton-list-item">
                  <app-skeleton-loader type="text" width="60%"></app-skeleton-loader>
                  <app-skeleton-loader type="text" width="40%"></app-skeleton-loader>
                </div>
              }
            </div>
          }
          @case ('stats') {
            <div class="skeleton-stats">
              @for (stat of [1,2,3,4]; track stat) {
                <div class="skeleton-stat-item">
                  <app-skeleton-loader type="text" width="80px" height="32px"></app-skeleton-loader>
                  <app-skeleton-loader type="text" width="60px" height="16px"></app-skeleton-loader>
                </div>
              }
            </div>
          }
          @case ('clock') {
            <div class="skeleton-clock">
              <app-skeleton-loader type="circle" width="120px" height="120px"></app-skeleton-loader>
              <div class="skeleton-clock-details">
                <app-skeleton-loader type="text" width="100%" height="20px"></app-skeleton-loader>
                <app-skeleton-loader type="text" width="80%" height="20px"></app-skeleton-loader>
              </div>
            </div>
          }
          @case ('chart') {
            <div class="skeleton-chart">
              <app-skeleton-loader type="rectangle" width="100%" height="200px"></app-skeleton-loader>
            </div>
          }
          @case ('calendar') {
            <div class="skeleton-calendar">
              @for (week of [1,2,3]; track week) {
                <div class="skeleton-calendar-week">
                  @for (day of [1,2,3,4,5,6,7]; track day) {
                    <app-skeleton-loader type="rectangle" width="40px" height="40px"></app-skeleton-loader>
                  }
                </div>
              }
            </div>
          }
        }
      </mat-card-content>
    </mat-card>
  `,
  styles: [`
    mat-card {
      height: 100%;
    }

    mat-card-content {
      padding: 1.5rem !important;
    }

    .skeleton-list {
      display: flex;
      flex-direction: column;
      gap: 1rem;
    }

    .skeleton-list-item {
      display: flex;
      flex-direction: column;
      gap: 0.5rem;
      padding: 0.75rem 0;
      border-bottom: 1px solid #f0f0f0;
    }

    .skeleton-stats {
      display: grid;
      grid-template-columns: repeat(auto-fit, minmax(120px, 1fr));
      gap: 1rem;
    }

    .skeleton-stat-item {
      display: flex;
      flex-direction: column;
      align-items: center;
      gap: 0.5rem;
      padding: 1rem;
    }

    .skeleton-clock {
      display: flex;
      flex-direction: column;
      align-items: center;
      gap: 1.5rem;
      padding: 2rem 0;
    }

    .skeleton-clock-details {
      display: flex;
      flex-direction: column;
      gap: 0.75rem;
      width: 100%;
      max-width: 300px;
    }

    .skeleton-chart {
      padding: 1rem 0;
    }

    .skeleton-calendar {
      display: flex;
      flex-direction: column;
      gap: 0.5rem;
    }

    .skeleton-calendar-week {
      display: flex;
      gap: 0.5rem;
      justify-content: space-between;
    }
  `]
})
export class WidgetSkeletonComponent {
  @Input() type: WidgetSkeletonType = 'list';
  @Input() title?: string;
  @Input() subtitle?: string;
}

