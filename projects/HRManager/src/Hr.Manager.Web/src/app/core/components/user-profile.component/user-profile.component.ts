import { Component, inject, OnInit } from '@angular/core';
import { AuthenticationService } from '../../services/authentication.service';
import { MatButtonModule } from '@angular/material/button';

@Component({
  selector: 'app-user-profile',
  imports: [MatButtonModule],
  templateUrl: './user-profile.component.html',
  styleUrl: './user-profile.component.css'
})
export class UserProfileComponent implements OnInit {
  public authService = inject(AuthenticationService);
  public initials: string = '';
  public avatarColor: string = '#555555';

  ngOnInit(): void {
    this.authService.loadUserProfile(true).subscribe(profile => {
      if (profile) {
        const firstInitial = profile.firstName?.[0] ?? '';
        const lastInitial = profile.lastName?.[0] ?? '';
        this.initials = (firstInitial + lastInitial).toUpperCase();

        this.avatarColor = this.generateAvatarColor(this.initials);
      }
    });
  }

  logout(): void {
    this.authService.logout();
  }

  private generateAvatarColor(label: string): string {
    const charCodeRed = label.charCodeAt(0) || 100;
    const charCodeGreen = label.charCodeAt(1) || charCodeRed;

    const red = (Math.pow(charCodeRed, 7) % 150) + 100;   
    const green = (Math.pow(charCodeGreen, 7) % 150) + 100; 
    const blue = ((red + green) % 150) + 100;

    return `rgb(${red}, ${green}, ${blue})`;
  }
}
