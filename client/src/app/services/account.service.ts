import { HttpClient } from '@angular/common/http';
import { inject, Injectable } from '@angular/core';

@Injectable({
  providedIn: 'root'
})

export class AccountService {
  private _client = inject(HttpClient);
  private _baseUrl = 'https://localhost:5001/api/account/'

  public login(model: any) {
    return this._client.post(this._baseUrl + "login", model);
  }
}
