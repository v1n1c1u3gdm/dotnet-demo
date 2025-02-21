import { CommonModule, NgIf } from '@angular/common';
import { HttpClient } from '@angular/common/http';
import { Component, inject, OnInit } from '@angular/core';
import { NavComponent } from "./nav/nav.component";

@Component({
  selector: 'app-root',
  imports: [CommonModule, NavComponent, NgIf],
  templateUrl: './app.component.html',
  styleUrl: './app.component.css'
})

export class AppComponent implements OnInit {
  title = 'client';
  client = inject(HttpClient);
  users: any;
  protected _isLoggedIn: boolean = false;

  ngOnInit(): void { }
}
