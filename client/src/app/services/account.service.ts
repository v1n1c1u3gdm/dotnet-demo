import { HttpClient } from '@angular/common/http';
import { inject, Injectable } from '@angular/core';
import { IUser } from '../models/User';
import { map } from 'rxjs';

@Injectable({
  providedIn: 'root'
})

export class AccountService {
  private _client = inject(HttpClient);
  private _currentUser: IUser = { username: "", token: "" };
  private _baseUrl = 'https://localhost:5001/api/account/'

  public login(model: IUser) {
    return this._client.post<IUser>(this._baseUrl + "login", model)
      .subscribe(response => {
        this._currentUser = response;
        localStorage.setItem("user", JSON.stringify(response));
      });
  }

  public logout() {
    localStorage.removeItem("user");
    this._currentUser = { username: "", token: "" };
  }

  public isLoggedin(): boolean {
    if (localStorage.getItem("user") && this._currentUser) return true
    else return false;
  }
}
