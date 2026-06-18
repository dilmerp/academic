import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable, tap } from 'rxjs';
import { environment } from '../../../environments/environment';

@Injectable({
  providedIn: 'root'
})
export class AuthService {
  private apiUrl = `${environment.apiUrl}/Auth`;

  constructor(private http: HttpClient) {}

  login(credentials: { login: string; clave: string }): Observable<any> {
    return this.http.post<any>(`${this.apiUrl}/login`, credentials).pipe(
      tap(response => {
        if (response && response.token) {
          localStorage.setItem('token', response.token);
          localStorage.setItem('nombreCompleto', response.nombreCompleto);
          localStorage.setItem('login', response.login);
        }
      })
    );
  }

/*   actualizarClave(data: { login: string; nuevaClave: string }): Observable<any> {
    return this.http.post<any>(`${this.apiUrl}/actualizar-clave`, data).pipe(
      tap(() => {
        // Opcional: actualizar estado local
        localStorage.setItem('requiereCambioClave', 'false');
      })
    );
  } */

  actualizarClave(data: { login: string; nuevaClave: string }): Observable<any> {
    const payload = {
      Login: data.login,
      NuevaClave: data.nuevaClave
    };

    console.log('Payload enviado:', payload);

    return this.http.post<any>(`${this.apiUrl}/actualizar-clave`, payload, {
      headers: { 'Content-Type': 'application/json' }
    }).pipe(
      tap(response => {
        console.log('Respuesta del backend:', response);
      })
    );
  }

  logout(): void {
    localStorage.removeItem('token');
    localStorage.removeItem('nombreCompleto');
    localStorage.removeItem('login');
    localStorage.removeItem('requiereCambioClave');
  }

  isAuthenticated(): boolean {
    return !!localStorage.getItem('token');
  }
}
