import { Component, inject, Input, SimpleChange, SimpleChanges } from '@angular/core';
import { MatIconModule } from '@angular/material/icon';
import { NotificationService } from '../../../core/services/notification.service';

@Component({
  selector: 'app-error-display',
  standalone: true,
  imports: [MatIconModule],
  template: '', 
  styleUrl: './error-display.component.css'
})
export class ErrorDisplayComponent {
  @Input() message: string | null = null;

  private notification = inject(NotificationService);

  ngOnChanges(changes: SimpleChanges): void {
    if (changes['message'] && this.message) {
      this.notification.handleError(this.message);
    }
  }
}
