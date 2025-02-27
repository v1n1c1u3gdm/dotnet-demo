import { Component, inject, output } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { AccountService } from '../services/account.service';
import { ToastrService } from 'ngx-toastr';
import { Router } from '@angular/router';

@Component({
  selector: 'app-register',
  imports: [FormsModule],
  templateUrl: './register.component.html',
  styleUrl: './register.component.css'
})
export class RegisterComponent {
  protected model: any = {};
  protected closeRegisteringForm = output<boolean>();
  protected _accountService = inject(AccountService);
  private _toastr = inject(ToastrService);
  private _router = inject(Router);

  register() {
    this._accountService.register(this.model).subscribe({
      next: apiUser => {
        localStorage.setItem("user", JSON.stringify(apiUser));
        this._accountService.currentUser = apiUser;
        this._accountService.requestedUser.set(apiUser);
      },
      error: e => this._toastr.error(e.error)
    });

    this.cancel();
  }

  cancel() {
    this.closeRegisteringForm.emit(false);
  }
}
