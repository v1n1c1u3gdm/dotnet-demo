import { Component, inject, input, signal } from '@angular/core';
import { FormsModule } from '@angular/forms'
import { AccountService } from '../services/account.service';
import { BsDropdownModule } from 'ngx-bootstrap/dropdown';
import { Router, RouterLink, RouterLinkActive } from '@angular/router';
import { ToastrService } from 'ngx-toastr';
import { TitleCasePipe } from '@angular/common';

@Component({
  selector: 'app-nav',
  imports: [FormsModule, BsDropdownModule, RouterLink, RouterLinkActive, TitleCasePipe],
  templateUrl: './nav.component.html',
  styleUrl: './nav.component.css'
})
export class NavComponent {
  protected model: any = { username: "", password: "" };
  protected _accountService = inject(AccountService);
  private _router = inject(Router);
  private _toastr = inject(ToastrService);

  public logout() {
    this._accountService.logout();
    this._router.navigateByUrl("/");
  }

  public login() {
    if (!this._accountService.isLoggedIn()) {
      this._accountService.login(this.model).subscribe({
        next: apiUser => {
          localStorage.setItem("user", JSON.stringify(apiUser));
          this._accountService.currentUser = apiUser;
          this._accountService.requestedUser.set(apiUser);
          this._router.navigateByUrl("/members");
        },
        error: e => this._toastr.error(e.error)
      });
    }
  }

  public isLoggedIn(): boolean {
    return this._accountService.isLoggedIn();
  }
}