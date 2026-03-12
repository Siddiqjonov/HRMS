import { Component, Input } from '@angular/core';
import { CommonModule } from '@angular/common';

export type SkeletonType = 'text' | 'circle' | 'rectangle' | 'card';

@Component({
  selector: 'app-skeleton-loader',
  standalone: true,
  imports: [CommonModule],
  template: `
    <div class="skeleton" [class]="getSkeletonClass()" [style.width]="width" [style.height]="height">
      <div class="skeleton-shimmer"></div>
    </div>
  `,
  styles: [`
    .skeleton {
      position: relative;
      overflow: hidden;
      background-color: #e0e0e0;
      animation: pulse 1.5s ease-in-out infinite;
    }

    .skeleton-text {
      height: 16px;
      border-radius: 4px;
      margin: 8px 0;
    }

    .skeleton-circle {
      border-radius: 50%;
    }

    .skeleton-rectangle {
      border-radius: 8px;
    }

    .skeleton-card {
      border-radius: 12px;
      height: 200px;
    }

    .skeleton-shimmer {
      position: absolute;
      top: 0;
      left: -100%;
      height: 100%;
      width: 100%;
      background: linear-gradient(
        90deg,
        transparent,
        rgba(255, 255, 255, 0.4),
        transparent
      );
      animation: shimmer 1.5s infinite;
    }

    @keyframes pulse {
      0%, 100% {
        opacity: 1;
      }
      50% {
        opacity: 0.7;
      }
    }

    @keyframes shimmer {
      0% {
        left: -100%;
      }
      100% {
        left: 100%;
      }
    }
  `]
})
export class SkeletonLoaderComponent {
  @Input() type: SkeletonType = 'text';
  @Input() width: string = '100%';
  @Input() height: string = 'auto';

  getSkeletonClass(): string {
    return `skeleton-${this.type}`;
  }
}

