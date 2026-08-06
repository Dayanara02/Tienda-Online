import { Component } from '@angular/core';

@Component({
  selector: 'app-login',
  imports: [],
  templateUrl: './login.html',
  styleUrl: './login.css'
})
export class Login {
  mostrarContrasena = false;

  cambiarVisibilidadContrasena(): void {
    this.mostrarContrasena = !this.mostrarContrasena;
  }
}