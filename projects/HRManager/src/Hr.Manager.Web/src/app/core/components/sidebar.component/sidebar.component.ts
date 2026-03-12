import { Component, inject, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Router, RouterModule } from '@angular/router';
import { MatIconModule } from '@angular/material/icon';
import { NavigationItem } from '../../models/navigation-item.model';
import { NAVIGATION_ITEMS } from '../../configurations/navigation.config';
import { AuthenticationService } from '../../services/authentication.service';
import { ApiService } from '../../services/api.service';
import { UserRole } from '../../models/roles.model';

@Component({
  selector: 'app-sidebar',
  standalone: true,
  imports: [CommonModule, RouterModule, MatIconModule],
  templateUrl: './sidebar.component.html',
  styleUrl: './sidebar.component.css'
})
export class SidebarComponent implements OnInit {
  navigationItems: NavigationItem[] = [];
  sidebarOpen = true;
  expandedItems = new Set<string>();
  private authService = inject(AuthenticationService);
  private apiService = inject(ApiService);
  private router = inject(Router);
  private isDepartmentManager = false;

  ngOnInit(): void {
    const userRoles = this.authService.getUserRoles();
    
    // Check if user is an Employee (not Admin or HrManager)
    const isEmployee = userRoles.includes(UserRole.Employee) && 
                       !userRoles.includes(UserRole.Admin) && 
                       !userRoles.includes(UserRole.HrManager);
    
    if (isEmployee) {
      // Check if the employee is a department manager
      this.checkIfDepartmentManager().then(isManager => {
        this.isDepartmentManager = isManager;
        this.filterNavigationItems(userRoles);
      });
    } else {
      this.filterNavigationItems(userRoles);
    }
  }

  private async checkIfDepartmentManager(): Promise<boolean> {
    const userEmail = this.authService.userEmail;
    if (!userEmail) return false;

    try {
      // Get current user's employee record
      const employee = await this.apiService.getEmployeeByEmail({ email: userEmail }).toPromise();
      if (!employee) return false;

      // Get all departments and check if any are managed by this employee
      const departments = await this.apiService.getDepartmentsWithPagination({ 
        pageNumber: 1, 
        pageSize: 100 
      }).toPromise();

      if (!departments) return false;

      // Check if any department's manager ID matches the current user's employee ID
      return departments.items.some(dept => dept.manager?.id === employee.id);
    } catch (error) {
      console.error('Error checking department manager status:', error);
      return false;
    }
  }

  private filterNavigationItems(userRoles: string[]): void {
    this.navigationItems = NAVIGATION_ITEMS.filter(item => {
      // Special handling for Reports: show to department managers
      if (item.route === '/reports' && this.isDepartmentManager) {
        return true;
      }

      const itemAccessible = item.roles.some(role => userRoles.includes(role));
      
      if (item.children && item.children.length > 0) {
        const hasAccessibleChildren = item.children.some(child =>
          child.roles.some(role => userRoles.includes(role))
        );
        if (itemAccessible || hasAccessibleChildren) {
          item.children = item.children.filter(child =>
            child.roles.some(role => userRoles.includes(role))
          );
          return true;
        }
      }
      
      return itemAccessible;
    });
  }

  toggleSidebar(): void {
    this.sidebarOpen = !this.sidebarOpen;
    this.closeAllSubMenus();
  }

  toggleSubMenu(item: NavigationItem, event: Event): void {
    event.preventDefault();
    event.stopPropagation();
    
    const itemId = item.route;
    
    if (!this.sidebarOpen) {
      this.sidebarOpen = true;
    }

    if (this.expandedItems.has(itemId)) {
      this.expandedItems.delete(itemId);
    } else {
      this.expandedItems.clear();
      this.expandedItems.add(itemId);
    }
  }

  closeAllSubMenus(): void {
    this.expandedItems.clear();
  }

  isExpanded(item: NavigationItem): boolean {
    return this.expandedItems.has(item.route);
  }

  hasChildren(item: NavigationItem): boolean {
    return !!(item.children && item.children.length > 0);
  }

  isChildRouteActive(childRoute: string): boolean {
    return this.router.isActive(childRoute, { paths: 'subset', queryParams: 'subset', fragment: 'ignored', matrixParams: 'ignored' });
  }

  logout(): void {
    this.authService.logout();
  }
}