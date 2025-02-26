import { Component, inject, output } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { AccountService } from '../services/account.service';

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

  register() {
    this._accountService.register(this.model);
    this._accountService.requestedUser.set(this.model);
    this.cancel();
  }

  cancel() {
    this.closeRegisteringForm.emit(false);
  }
}
