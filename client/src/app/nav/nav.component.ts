import { Component, inject } from '@angular/core';
import { FormsModule } from '@angular/forms'
import { AccountService } from '../services/account.service';
import { BsDropdownModule } from 'ngx-bootstrap/dropdown';
import { IUser } from '../models/User';

@Component({
  selector: 'app-nav',
  imports: [FormsModule, BsDropdownModule],
  templateUrl: './nav.component.html',
  styleUrl: './nav.component.css'
})
export class NavComponent {
  model: any = {};
  private _accountService = inject(AccountService);

  logout() {
    this._accountService.logout();
  }

  login() {
    if(!this._accountService.isLoggedin()){
      this._accountService.login(this.model);
    }
  }

  isLoggedIn(): boolean{
    return this._accountService.isLoggedin();
  }
}