import { inject, Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../../environments/environment';

@Injectable({
  providedIn: 'root'
})
export class ProfesorService {
  private http = inject(HttpClient);

  // URLs de tu API en .NET Core
  // private apiUrl = 'https://localhost:7241/api/Profesores';
  // private maestrosUrl = 'https://localhost:7241/api/Maestros';

  private apiUrl = `${environment.apiUrl}/Profesores`;
  private maestrosUrl = `${environment.apiUrl}/Maestros`;

  // --- MÉTODOS DE PROFESORES ---

  getProfesores(): Observable<any[]> {
    return this.http.get<any[]>(this.apiUrl);
  }

  getProfesorById(id: number): Observable<any> {
    return this.http.get<any>(`${this.apiUrl}/${id}`);
  }

  createProfesor(profesor: any): Observable<any> {
    return this.http.post<any>(this.apiUrl, profesor);
  }

  updateProfesor(id: number, profesor: any): Observable<any> {
    return this.http.put<any>(`${this.apiUrl}/${id}`, profesor);
  }

  deleteProfesor(id: number): Observable<any> {
    return this.http.delete<any>(`${this.apiUrl}/${id}`);
  }

  // --- MÉTODOS DE MAESTROS (Dropdowns) ---

  getTenores(): Observable<any[]> {
    return this.http.get<any[]>(`${this.maestrosUrl}/tenores`);
  }

  getEstadosCiviles(): Observable<any[]> {
    return this.http.get<any[]>(`${this.maestrosUrl}/estados-civiles`);
  }

  getTiposDocumento(): Observable<any[]> {
    return this.http.get<any[]>(`${this.maestrosUrl}/documentos-tipos`);
  }

  getCursos(): Observable<any[]> {
    return this.http.get<any[]>(`${this.maestrosUrl}/cursos`);
  }
}
