import { HttpClient, HttpHeaders } from '@angular/common/http';
import { inject, Injectable, signal } from '@angular/core';
import { IUser } from '../models/User';
import { Observable } from 'rxjs';

@Injectable({
  providedIn: 'root'
})

export class AccountService {
  private readonly _client = inject(HttpClient);
  private readonly _baseUrl = 'https://localhost:5001/api/account/';
  public currentUser: IUser = { username: "", token: "" };
  public requestedUser = signal<any>({ username: "", password: "" });
  private readonly _cfgHttp = {
    headers: new HttpHeaders({ 'Content-Type': 'application/json' })
  }
  public login(model: any) : Observable<IUser> {
    return this._client.post<IUser>(this._baseUrl + "login", model, this._cfgHttp);
  }

  public logout() {
    localStorage.removeItem("user");
    this.currentUser = { username: "", token: "" };
    this.requestedUser.set(null);
  }

  public register(model: any) : Observable<IUser> {
    return this._client.post<IUser>(this._baseUrl + "register", model, this._cfgHttp);
  }

  public isLoggedIn(): boolean {
    if (localStorage.getItem("user") && this.currentUser && this.requestedUser()) return true
    else return false;
  }
}
