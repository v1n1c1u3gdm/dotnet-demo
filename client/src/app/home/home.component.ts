import { Component, input } from '@angular/core';
import { RegisterComponent } from "../register/register.component";

@Component({
  selector: 'app-home',
  imports: [RegisterComponent],
  templateUrl: './home.component.html',
  styleUrl: './home.component.css'
})
export class HomeComponent {
  protected _registerMode: boolean = false;
  
  protected closeRegister(event: boolean){
    this._registerMode = event;
  }

  protected registerToggle(){
    this._registerMode = !this._registerMode;
  }
}