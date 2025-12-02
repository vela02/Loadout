import {BaseComponent} from './baseComponent';

export abstract class BaseFormComponent<TModel> extends BaseComponent {
  model!: TModel;
  isEditMode = false;

  /**
   * Djeca implementiraju load ako edit-mode.
   */
  protected abstract loadData(): void;

  /**
   * Djeca implementiraju save logiku.
   */
  protected abstract save(): void;

  initForm(isEdit: boolean) {
    this.isEditMode = isEdit;

    if (isEdit) {
      this.loadData();
    }
  }

  onSubmit() {
    this.save();
  }
}
