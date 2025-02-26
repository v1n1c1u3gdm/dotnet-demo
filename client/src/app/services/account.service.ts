import { HttpClient } from '@angular/common/http';
import { inject, Injectable, signal } from '@angular/core';
import { IUser } from '../models/User';

@Injectable({
  providedIn: 'root'
})

export class AccountService {
  private readonly _client = inject(HttpClient);
  private readonly _baseUrl = 'https://localhost:5001/api/account/';
  public currentUser: IUser = { username: "", token: "" };
  public requestedUser = signal<any>({ username: "", password: "" });

  public login(model: any) {
    return this._client.post<IUser>(this._baseUrl + "login", this.requestedUser())
      .subscribe(
        {
          next: apiUser => {
            localStorage.setItem("user", JSON.stringify(apiUser));
            this.currentUser = apiUser;
            this.requestedUser.set(apiUser);
          },
          error: e => console.log(e)
        });
  }

  public logout() {
    localStorage.removeItem("user");
    this.currentUser = { username: "", token: "" };
    this.requestedUser.set(null);
  }

  public register(model: any) {
    return this._client.post<IUser>(this._baseUrl + "register", this.requestedUser())
      .subscribe({
        next: apiUser => {
          localStorage.setItem("user", JSON.stringify(apiUser));
          this.currentUser = apiUser;
          this.requestedUser.set(apiUser);
        },
        error: e => console.log(e)
      });
  }

  public isLoggedIn(): boolean {
    if (localStorage.getItem("user") && this.currentUser && this.requestedUser()) return true
    else return false;
  }
}
