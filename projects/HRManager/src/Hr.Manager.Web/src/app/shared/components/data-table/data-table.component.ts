import { AfterViewInit, Component, Input, OnChanges, SimpleChange, SimpleChanges, ViewChild, viewChild, OnInit, OnDestroy } from '@angular/core';
import { CommonModule } from '@angular/common';
import { MatTableDataSource, MatTableModule } from '@angular/material/table';
import { MatPaginator, MatPaginatorModule, PageEvent, MatPaginatorIntl } from '@angular/material/paginator';
import { MatSort, MatSortModule } from '@angular/material/sort';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { Output, EventEmitter } from '@angular/core';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatDialogContainer } from '@angular/material/dialog';
import { MatTooltipModule } from '@angular/material/tooltip';
import { DaysOfWeek } from '../../../core/models/enums/days-of-week.enum';
import { Subject } from 'rxjs';

class CustomPaginatorIntl extends MatPaginatorIntl {
  paginatorLabel: string = '';
  
  override getRangeLabel = (page: number, pageSize: number, length: number): string => {
    if (length == 0 || pageSize == 0) {
      const baseLabel = `Showing 0 of ${length}`;
      return this.paginatorLabel ? `${baseLabel} ${this.paginatorLabel}` : baseLabel;
    }
    length = Math.max(length, 0);
    const startIndex = page * pageSize;
    const endIndex = startIndex < length ? Math.min(startIndex + pageSize, length) : startIndex + pageSize;
    const baseLabel = `Showing ${startIndex + 1}-${endIndex} of ${length}`;
    return this.paginatorLabel ? `${baseLabel} ${this.paginatorLabel}` : baseLabel;
  };
}

@Component({
  selector: 'app-data-table',
  standalone: true,
  imports: [
    CommonModule,
    MatTableModule,
    MatPaginatorModule,
    MatSortModule,
    MatFormFieldModule,
    MatInputModule,
    MatButtonModule,
    MatIconModule,
    MatPaginatorModule,
    MatPaginator,
    MatSortModule,
    MatIconModule,
    MatTooltipModule,
  ],
  providers: [
    { provide: MatPaginatorIntl, useClass: CustomPaginatorIntl }
  ],
  templateUrl: './data-table.component.html',
  styleUrl: './data-table.component.css'
})
export class DataTableComponent implements AfterViewInit, OnChanges, OnInit, OnDestroy {
  @Input() dataSource = new MatTableDataSource<any>([]);
  @Output() view = new EventEmitter<any>();

  @Input() displayedColumns: { key: string; label: string }[] = [];
  @Input() length = 0;
  @Input() pageSize = 10;
  @Input() pageIndex = 0;
  @Input() showViewButton: boolean = true;
  @Input() showEditButton: boolean = true;
  @Input() showDeleteButton: boolean = true;
  @Input() customViewIcon: string = 'visibility';
  @Input() customViewTooltip: string = 'View';
  @Input() customEditIcon: string = 'edit';
  @Input() customEditTooltip: string = 'Edit';
  @Input() showApproveReject = false;
  @Input() paginatorLabel: string = '';
  @Input() showPagination: boolean = true;
  @Input() selectedItem: any = null;
  @Input() getCategoryLabel: ((category: string) => string) | null = null;

  @ViewChild(MatPaginator) paginator!: MatPaginator;
  @ViewChild(MatSort) sort!: MatSort;

  @Output() edit = new EventEmitter<any>();
  @Output() delete = new EventEmitter<any>();
  @Output() pageChange = new EventEmitter<PageEvent>();
  @Output() assignManager = new EventEmitter<any>();
  @Output() viewEmployees = new EventEmitter<any>();
  @Output() approve = new EventEmitter<any>();
  @Output() reject = new EventEmitter<any>();
  @Output() rowClick = new EventEmitter<any>();

  constructor(private paginatorIntl: MatPaginatorIntl) {}

  get columnKeys(): string[] {
    return this.displayedColumns.map(c => c.key);
  }

  ngOnInit(): void {
    this.updateRangeLabel();
  }

  ngOnDestroy(): void {
    // Cleanup if needed
  }

  private updateRangeLabel(): void {
    const customIntl = this.paginatorIntl as CustomPaginatorIntl;
    if (customIntl && 'paginatorLabel' in customIntl) {
      customIntl.paginatorLabel = this.paginatorLabel;
      customIntl.changes.next();
    }
  }

  ngAfterViewInit(): void {
    this.dataSource.sort = this.sort;
    if (this.paginator && this.showPagination) {
      this.paginator.page.subscribe((event: PageEvent) => {
        this.pageChange.emit(event);
      });
    }
  }

  ngOnChanges(changes: SimpleChanges): void {
    if (changes['paginatorLabel']) {
      this.updateRangeLabel();
    }
    setTimeout(() => {
      if (this.paginator) {
        this.paginator.length = this.length;
        this.paginator.pageSize = this.pageSize;
        this.paginator.pageIndex = this.pageIndex;
      }
    });
  }

  applyFilter(event: Event) {
    const filterValue = (event.target as HTMLInputElement).value;
    this.dataSource.filter = filterValue.trim().toLowerCase();
  }

  formatDaysOfWeek(days: DaysOfWeek): string {
    const dayNames: string[] = [];
    
    if (days & DaysOfWeek.Monday) dayNames.push('Mon');
    if (days & DaysOfWeek.Tuesday) dayNames.push('Tue');
    if (days & DaysOfWeek.Wednesday) dayNames.push('Wed');
    if (days & DaysOfWeek.Thursday) dayNames.push('Thu');
    if (days & DaysOfWeek.Friday) dayNames.push('Fri');
    if (days & DaysOfWeek.Saturday) dayNames.push('Sat');
    if (days & DaysOfWeek.Sunday) dayNames.push('Sun');
    
    return dayNames.length > 0 ? dayNames.join(', ') : 'None';
  }

  getDocumentTypeForBadge(element: any): string {
    // Extract document type from element for badge styling
    if (element.documentTypeLabel) {
      // If documentTypeLabel exists (like "Contract", "Certificate"), use it lowercase
      return element.documentTypeLabel.toLowerCase().replace(/\s+/g, '-');
    } else if (element.documentType !== undefined) {
      // If documentType enum exists, convert to string
      const typeMapping: { [key: number]: string } = {
        0: 'contract',
        1: 'certificate',
        2: 'identity',
        3: 'military'
      };
      return typeMapping[element.documentType] || 'default';
    }
    return 'default';
  }

  onRowClick(row: any): void {
    this.rowClick.emit(row);
  }
}
