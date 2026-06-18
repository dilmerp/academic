import {inject,Injectable} from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../../environments/environment';

@Injectable({
  providedIn: 'root'
})
export class AlumnoService {
  private http = inject(HttpClient);
  // URLs de tu API en .NET Core

  // private apiUrl = 'https://localhost:7241/api/Alumnos';

  private apiUrl = `${environment.apiUrl}/Alumnos`;
  getAlumnos(): Observable<any[]> {
    return this.http.get<any[]>(this.apiUrl);
  }
}

