import { CommonModule, NgIf } from '@angular/common';
import { HttpClient } from '@angular/common/http';
import { Component, Inject, inject, OnInit } from '@angular/core';
import { NavComponent } from "./nav/nav.component";
import { AccountService } from './services/account.service';

@Component({
  selector: 'app-root',
  imports: [CommonModule, NavComponent, NgIf],
  templateUrl: './app.component.html',
  styleUrl: './app.component.css'
})

export class AppComponent implements OnInit {
  title = 'client';
  protected _accountService = inject(AccountService);
  private _client = Inject(HttpClient);
  users: any;

  ngOnInit(): void {
    // if(this._accountService.isLoggedin()){
    //   this.users = localStorage
    }

  getUsers(){
    this._client.get("https://localhost:5001/api/Users").subscribe();

  }
}
