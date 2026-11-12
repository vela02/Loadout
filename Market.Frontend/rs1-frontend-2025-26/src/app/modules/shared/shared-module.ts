import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import {FitPaginatorBarComponent} from './components/fit-paginator-bar/fit-paginator-bar.component';
import {materialModules} from './material-modules';
import {FormsModule, ReactiveFormsModule} from '@angular/forms';
import {TranslatePipe} from '@ngx-translate/core';
import { FitConfirmDialogComponent } from './components/fit-confirm-dialog/fit-confirm-dialog.component';
import {DialogHelperService} from './services/dialog-helper.service';
import { FitLoadingBarComponent } from './components/fit-loading-bar/fit-loading-bar.component';
import { FitTableSkeletonComponent } from './components/fit-table-skeleton/fit-table-skeleton.component';
import { LicenseKeyPipe } from './pipes/license-key.pipe';
import { CopyToClipboardDirective } from './directives/copy-to-clipboard.directive';
import { QrCodeComponent } from './components/qr-code/qr-code.component';
import { RichTextEditorComponent } from './components/rich-text-editor/rich-text-editor.component';



@NgModule({
  declarations: [
    FitPaginatorBarComponent,
    FitConfirmDialogComponent,
    FitLoadingBarComponent,
    FitTableSkeletonComponent,
    LicenseKeyPipe,
    CopyToClipboardDirective,
    QrCodeComponent,
    RichTextEditorComponent
  ],
  imports: [
    CommonModule,
    ReactiveFormsModule,
    FormsModule,
    TranslatePipe,
    ...materialModules
  ],
  providers: [
    DialogHelperService
  ],
  exports:[
    FitPaginatorBarComponent,
    CommonModule,
    ReactiveFormsModule,
    TranslatePipe,
    FormsModule,
    FitLoadingBarComponent,
    FitTableSkeletonComponent,
    LicenseKeyPipe,
    CopyToClipboardDirective,
    QrCodeComponent,
    RichTextEditorComponent,
    materialModules
  ]
})
export class SharedModule { }
