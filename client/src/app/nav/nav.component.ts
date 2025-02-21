import { Component, inject } from '@angular/core';
import { FormsModule } from '@angular/forms'
import { AccountService } from '../services/account.service';
import { BsDropdownModule } from 'ngx-bootstrap/dropdown';

@Component({
  selector: 'app-nav',
  imports: [FormsModule, BsDropdownModule],
  templateUrl: './nav.component.html',
  styleUrl: './nav.component.css'
})
export class NavComponent {
  model: any = {};
  private _accountService = inject(AccountService);
  protected _isLoggedIn: boolean = false;

  logout() {
    this._isLoggedIn = false;
  }

  login() {
    console.log(this.model);

    this._accountService.login(this.model).subscribe({
      next: response => {
        this._isLoggedIn = true;
        console.log(response);
      },
      error: e => {
        this._isLoggedIn = false;
        console.log(e);
      }
    });
  }
}