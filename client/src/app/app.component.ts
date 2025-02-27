import { CommonModule } from '@angular/common';
import { Component, inject, OnInit } from '@angular/core';
import { NavComponent } from "./nav/nav.component";
import { AccountService } from './services/account.service';
import { RouterOutlet } from '@angular/router';

@Component({
  selector: 'app-root',
  imports: [RouterOutlet, CommonModule, NavComponent],
  templateUrl: './app.component.html',
  styleUrl: './app.component.css'
})

export class AppComponent implements OnInit {

  title = 'client';
  protected _accountService = inject(AccountService);

  ngOnInit(): void {
    this.setCurrentUser();
  }

  setCurrentUser(): any {
    if (this._accountService.isLoggedIn()) {
      let userStr = localStorage.getItem("user");
      if (userStr) {
        let user = JSON.parse(userStr);
        this._accountService.currentUser = user;
        this._accountService.requestedUser.set(user);
      }
    }
  }
}
