import { Component, inject, OnInit, Output, EventEmitter } from '@angular/core';
import { NavigationItem } from '../../models/navigation-item.model';
import { AuthenticationService } from '../../services/authentication.service';
import { Router, RouterModule } from '@angular/router';
import { NAVIGATION_ITEMS } from '../../configurations/navigation.config';
import { MatIcon } from "@angular/material/icon";

@Component({
  selector: 'app-navigation-menu',
  imports: [MatIcon, RouterModule],
  templateUrl: './navigation-menu.component.html',
  styleUrl: './navigation-menu.component.css'
})
export class NavigationMenuComponent implements OnInit {
  @Output() menuItemClicked = new EventEmitter<void>();
  
  navigationItems: NavigationItem[] = [];
  hoveredItem: NavigationItem | null = null;
  clickedItems: Set<NavigationItem> = new Set();
  private hoverTimeout: any = null;
  private authService = inject(AuthenticationService)
  private router = inject(Router)


  ngOnInit(): void {
    const userRoles = this.authService.getUserRoles();
    this.navigationItems = NAVIGATION_ITEMS.filter(item => {
      // Check if item itself is accessible
      const itemAccessible = item.roles.some(role => userRoles.includes(role));
      
      // Check if any children are accessible
      if (item.children && item.children.length > 0) {
        const hasAccessibleChildren = item.children.some(child =>
          child.roles.some(role => userRoles.includes(role))
        );
        // Include parent if it's accessible OR if it has accessible children
        if (itemAccessible || hasAccessibleChildren) {
          // Filter children to only show accessible ones
          item.children = item.children.filter(child =>
            child.roles.some(role => userRoles.includes(role))
          );
          return true;
        }
      }
      
      return itemAccessible;
    });
  }

  isActive(route: string): boolean {
    return this.router.url === route;
  }

  hasActiveChild(item: NavigationItem): boolean {
    if (!item.children) return false;
    return item.children.some(child => this.isActive(child.route));
  }

  onMenuItemClick(): void {
    this.menuItemClicked.emit();
  }

  onItemMouseEnter(item: NavigationItem): void {
    if (this.hoverTimeout) {
      clearTimeout(this.hoverTimeout);
      this.hoverTimeout = null;
    }
    if (item.children && item.children.length > 0) {
      this.hoveredItem = item;
    }
  }

  onItemMouseLeave(): void {
    // Add a small delay to prevent closing when moving to dropdown
    this.hoverTimeout = setTimeout(() => {
      // Clear hover state (used on desktop)
      this.hoveredItem = null;
    }, 100);
  }

  onItemClick(item: NavigationItem, event: Event): void {
    // Only handle click for items with children
    if (item.children && item.children.length > 0) {
      // Check if we're on a small screen (mobile) - use window width
      const isMobile = window.innerWidth <= 800;
      
      if (isMobile) {
        // On mobile: prevent navigation and toggle dropdown
        event.preventDefault();
        event.stopPropagation();
        // Clear hover state when clicking (for mobile devices)
        if (this.hoveredItem === item) {
          this.hoveredItem = null;
        }
        // Toggle dropdown on mobile
        if (this.clickedItems.has(item)) {
          this.clickedItems.delete(item);
        } else {
          this.clickedItems.add(item);
        }
      } else {
        // On desktop: let the link navigate normally, don't toggle clickedItems
        // Hover will handle the dropdown display
      }
    }
  }

  hasChildren(item: NavigationItem): boolean {
    return !!(item.children && item.children.length > 0);
  }

  isItemExpanded(item: NavigationItem): boolean {
    // Check if we're on a small screen (mobile)
    const isMobile = window.innerWidth <= 800;
    
    if (isMobile) {
      // On mobile: use clicked state
      return this.clickedItems.has(item);
    } else {
      // On desktop: use hover state only
      return this.hoveredItem === item;
    }
  }
}
