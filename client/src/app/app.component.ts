import { CommonModule } from '@angular/common';
import { HttpClient } from '@angular/common/http';
import { Component, inject, OnInit } from '@angular/core';
import { NavComponent } from "./nav/nav.component";

@Component({
  selector: 'app-root',
  imports: [CommonModule, NavComponent],
  templateUrl: './app.component.html',
  styleUrl: './app.component.css'
})
export class AppComponent implements OnInit {
  title = 'client';
  client = inject(HttpClient);
  users: any;

  ngOnInit(): void {
    this.client.get('https://localhost:5001/api/Users')
      .subscribe({
        next: response => this.users = response,
        error: error => console.log(error),
        complete : () =>  {}
      })
  }
}
