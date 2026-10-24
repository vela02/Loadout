import { Component, inject, OnInit } from '@angular/core';
import { FormBuilder, Validators } from '@angular/forms';
import { MatDialogConfig, MatDialogRef } from '@angular/material/dialog';
import { TranslateService } from '@ngx-translate/core';
import { PromotionsApiService } from '../../../../../api-services/promotions/promotions-api.service';
import {
  CreatePromotionCommand,
  GetPromotionOptionsQueryDto,
  PromotionGameDto,
  PromotionOptionDto,
  PromotionTargetType
} from '../../../../../api-services/promotions/promotions-api.models';
import { ToasterService } from '../../../../../core/services/toaster.service';

interface TargetTypeOption {
  type: PromotionTargetType;
  icon: string;
}

/** Shared dialog options, used wherever the "new promotion" form is opened from */
export const PROMOTION_DIALOG_CONFIG: MatDialogConfig = {
  width: '720px',
  maxWidth: '95vw',
  panelClass: 'loadout-dialog',
  autoFocus: false
};

/** yyyy-MM-dd in local time, the format <input type="date"> works with */
function toDateInput(date: Date): string {
  const pad = (n: number) => n.toString().padStart(2, '0');
  return `${date.getFullYear()}-${pad(date.getMonth() + 1)}-${pad(date.getDate())}`;
}

@Component({
  selector: 'app-promotion-create-dialog',
  standalone: false,
  templateUrl: './promotion-create-dialog.component.html',
  styleUrl: './promotion-create-dialog.component.scss'
})
export class PromotionCreateDialogComponent implements OnInit {
  private dialogRef = inject(MatDialogRef<PromotionCreateDialogComponent>);
  private api = inject(PromotionsApiService);
  private toaster = inject(ToasterService);
  private translate = inject(TranslateService);
  private fb = inject(FormBuilder);

  targetTypes: TargetTypeOption[] = [
    { type: 'Games', icon: 'sports_esports' },
    { type: 'Category', icon: 'category' },
    { type: 'Genre', icon: 'theater_comedy' },
    { type: 'Publisher', icon: 'business' },
    { type: 'Developer', icon: 'code' },
    { type: 'Tag', icon: 'sell' }
  ];

  quickDiscounts = [10, 25, 50, 75];

  options: GetPromotionOptionsQueryDto | null = null;
  isLoadingOptions = true;
  isSaving = false;
  errorMessage: string | null = null;

  selectedGameIds = new Set<number>();
  gameSearch = '';

  form = this.fb.group({
    targetType: this.fb.nonNullable.control<PromotionTargetType>('Genre'),
    targetId: this.fb.control<number | null>(null),
    targetText: this.fb.control<string | null>(null),
    name: ['', [Validators.required, Validators.maxLength(150)]],
    discountPercentage: [25, [Validators.required, Validators.min(1), Validators.max(100)]],
    startDate: [toDateInput(new Date()), Validators.required],
    endDate: [toDateInput(new Date(Date.now() + 7 * 24 * 60 * 60 * 1000)), Validators.required],
    description: ['', Validators.maxLength(500)]
  });

  ngOnInit(): void {
    this.api.getOptions().subscribe({
      next: (options) => {
        this.options = options;
        this.isLoadingOptions = false;
      },
      error: (err) => {
        this.isLoadingOptions = false;
        console.error('Load promotion options error:', err);
      }
    });
  }

  // === Target ===

  get targetType(): PromotionTargetType {
    return this.form.controls.targetType.value;
  }

  get isIdTarget(): boolean {
    return this.targetType === 'Category' || this.targetType === 'Tag';
  }

  selectTargetType(type: PromotionTargetType): void {
    this.form.patchValue({ targetType: type, targetId: null, targetText: null });
  }

  /** Category / tag options (selected by id) */
  get idOptions(): PromotionOptionDto[] {
    if (!this.options) return [];
    return this.targetType === 'Category' ? this.options.categories : this.options.tags;
  }

  /** Genre / publisher / developer options (selected by name) */
  get textOptions(): string[] {
    if (!this.options) return [];
    switch (this.targetType) {
      case 'Genre':
        return this.options.genres;
      case 'Publisher':
        return this.options.publishers;
      case 'Developer':
        return this.options.developers;
      default:
        return [];
    }
  }

  // === Individual games ===

  get filteredGames(): PromotionGameDto[] {
    const games = this.options?.games ?? [];
    const term = this.gameSearch.trim().toLowerCase();
    return term ? games.filter(g => g.title.toLowerCase().includes(term)) : games;
  }

  isSelected(gameId: number): boolean {
    return this.selectedGameIds.has(gameId);
  }

  toggleGame(gameId: number): void {
    if (this.selectedGameIds.has(gameId)) {
      this.selectedGameIds.delete(gameId);
    } else {
      this.selectedGameIds.add(gameId);
    }
  }

  clearSelection(): void {
    this.selectedGameIds.clear();
  }

  // === Summary ===

  /** Games the promotion would apply to right now (mirrors the backend matching) */
  get matchingGames(): PromotionGameDto[] {
    const games = this.options?.games ?? [];
    const { targetId, targetText } = this.form.getRawValue();

    switch (this.targetType) {
      case 'Games':
        return games.filter(g => this.selectedGameIds.has(g.id));
      case 'Category':
        return targetId == null ? [] : games.filter(g => g.categoryId === targetId);
      case 'Tag':
        return targetId == null ? [] : games.filter(g => g.tagIds.includes(targetId));
      case 'Genre':
        return targetText ? games.filter(g => g.genre === targetText) : [];
      case 'Publisher':
        return targetText ? games.filter(g => g.publisher === targetText) : [];
      case 'Developer':
        return targetText ? games.filter(g => g.developer === targetText) : [];
    }
  }

  get discount(): number {
    return Number(this.form.controls.discountPercentage.value) || 0;
  }

  /** First matching game with a price, to show what the discount looks like */
  get example(): { title: string; oldPrice: string; newPrice: string } | null {
    const game = this.matchingGames.find(g => g.price > 0);
    if (!game || this.discount <= 0 || this.discount > 100) return null;

    return {
      title: game.title,
      oldPrice: game.price.toFixed(2),
      newPrice: (game.price * (1 - this.discount / 100)).toFixed(2)
    };
  }

  setDiscount(value: number): void {
    this.form.controls.discountPercentage.setValue(value);
  }

  get datesInvalid(): boolean {
    const { startDate, endDate } = this.form.getRawValue();
    return !!startDate && !!endDate && endDate < startDate;
  }

  get canSubmit(): boolean {
    return this.form.valid && !this.datesInvalid && this.matchingGames.length > 0 && !this.isSaving;
  }

  hasError(controlName: string, errorType: string): boolean {
    const control = this.form.get(controlName);
    return !!control && control.touched && control.hasError(errorType);
  }

  // === Actions ===

  submit(): void {
    this.form.markAllAsTouched();
    if (!this.canSubmit) return;

    const value = this.form.getRawValue();
    const command: CreatePromotionCommand = {
      name: value.name!.trim(),
      description: value.description?.trim() || null,
      discountPercentage: this.discount,
      // whole days: from the start of the first day to the end of the last day
      startDate: `${value.startDate}T00:00:00`,
      endDate: `${value.endDate}T23:59:59`,
      targetType: this.targetType,
      targetId: this.isIdTarget ? value.targetId : null,
      targetText: !this.isIdTarget && this.targetType !== 'Games' ? value.targetText : null,
      gameIds: this.targetType === 'Games' ? [...this.selectedGameIds] : null
    };

    this.isSaving = true;
    this.errorMessage = null;

    this.api.create(command).subscribe({
      next: () => {
        this.toaster.success(this.translate.instant('PROMOTIONS.DIALOGS.SUCCESS_CREATE'));
        this.dialogRef.close(true);
      },
      error: (err) => {
        this.isSaving = false;
        this.errorMessage =
          err?.error?.message ||
          err?.error?.detail ||
          this.translate.instant('PROMOTIONS.DIALOGS.ERROR_CREATE');
        console.error('Create promotion error:', err);
      }
    });
  }

  onCancel(): void {
    this.dialogRef.close(false);
  }
}
