import { Component, inject, input, signal } from '@angular/core';
import { FormsModule } from '@angular/forms'
import { AccountService } from '../services/account.service';
import { BsDropdownModule } from 'ngx-bootstrap/dropdown';
import { RouterLink } from '@angular/router';

@Component({
  selector: 'app-nav',
  imports: [FormsModule, BsDropdownModule, RouterLink],
  templateUrl: './nav.component.html',
  styleUrl: './nav.component.css'
})
export class NavComponent {
  protected _accountService = inject(AccountService);
  protected model: any = { username: "", password: "" };

  public logout() {
    this._accountService.logout();
  }

  public login() {
    if (!this._accountService.isLoggedIn()) {
      this._accountService.login(this.model);
    }
  }

  public isLoggedIn(): boolean {
    return this._accountService.isLoggedIn();
  }
}