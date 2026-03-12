import { inject, Injectable } from '@angular/core';
import { MatSnackBar } from '@angular/material/snack-bar';

@Injectable({
  providedIn: 'root'
})
export class NotificationService {

  private snackBar = inject(MatSnackBar)

  success(message: string, duration: number = 5000) {
    this.snackBar.open(message, '✖', {
      duration,
      panelClass: ['toast-success'],
      verticalPosition: 'top',
      horizontalPosition: 'right'
    });
  }

  private error(message: string, duration: number = 5000) {
    this.snackBar.open(message, '✖', {
      duration,
      panelClass: ['toast-error'],
      verticalPosition: 'top',
      horizontalPosition: 'right'
    });
  }

  info(message: string, duration: number = 3000) {
    this.snackBar.open(message, '✖', {
      duration,
      panelClass: ['toast-info'],
      verticalPosition: 'top',
      horizontalPosition: 'right'
    });
  }

  warning(message: string, duration: number = 3000) {
    this.snackBar.open(message, '✖', {
      duration,
      panelClass: ['toast-warning'],
      verticalPosition: 'top',
      horizontalPosition: 'right'
    });
  }

  handleError(error: any, fallbackMessage: string = 'An error occurred'): void {

    if (error?.error?.errors && typeof error.error.errors === 'object') {
      for (const key in error.error.errors) {
        const messages = error.error.errors[key];
        if (Array.isArray(messages)) {
          messages.forEach((msg) => this.error(msg));
        }
      }
      return;
    }
  
    let message =
      error?.error?.detail ||
      error?.error?.message ||
      (typeof error?.error === 'string' ? error.error : null) ||
      error?.message ||
      fallbackMessage;
  
    this.error(message);
  }  
}
