import { inject, Injectable } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import { map } from 'rxjs/operators';
import { RegistrarMatriculaCarreraRequest, MatriculaResponse, PlanPagoItem, BecaItem } from '../models/matricula.model';
import { environment } from '../../../environments/environment';

@Injectable({
  providedIn: 'root'
})
export class MatriculaService {
  private http = inject(HttpClient);

  // URLs centralizadas desde el environment
  private apiUrl = `${environment.apiUrl}/Matriculas`;
  private maestrosUrl = `${environment.apiUrl}/Maestros`;
  private alumnosUrl = `${environment.apiUrl}/Alumnos`;

  // --- MÉTODOS DE MATRÍCULAS (CARRERA Y PROGRAMA) ---

  getMatriculasByAlumno(idAlumno: number): Observable<MatriculaResponse[]> {
    return this.http.get<MatriculaResponse[]>(`${this.apiUrl}/alumno/${idAlumno}`);
  }

  getMatriculadosByPromocion(idPromocion: number, tipoFiltro: string = 'Todos'): Observable<MatriculaResponse[]> {
    const params = new HttpParams().set('tipoFiltro', tipoFiltro);

    return this.http.get<any[]>(`${this.apiUrl}/promocion/${idPromocion}`, { params }).pipe(
      map((data: any[]): MatriculaResponse[] =>
        (data ?? []).map((m): MatriculaResponse => ({
          // Mapeo defensivo: SOPORTA JSON en minúsculas (PostgreSQL) o camelCase (C#)
          idAlumno: m.idalumno ?? m.idAlumno ?? m.IdAlumno ?? 0,
          idRegistro: m.idregistro ?? m.idRegistro ?? m.IdRegistro ?? 0,
          idMatricula: m.idmatricula ?? m.idMatricula ?? m.IdMatricula ?? 0,
          idPromocion: idPromocion,
          idGrupo: m.idgrupo ?? m.idGrupo ?? m.IdGrupo ?? 0,
          esMatricula: m.esmatricula ?? m.esMatricula ?? m.EsMatricula ?? false,
          nombreCompleto: m.nombrecompleto ?? m.nombreCompleto ?? m.NombreCompleto ?? '',
          grupoCodigo: m.grupocodigo ?? m.grupoCodigo ?? m.GrupoCodigo ?? '',
          seccion: m.seccion ?? m.Seccion ?? '',
          matriculaFecha: m.matriculafecha ?? m.matriculaFecha ?? m.MatriculaFecha ?? '',
          matriculaPor: m.matriculadopor ?? m.matriculadoPor ?? m.MatriculadoPor ?? ''
        }))
      )
    );
  }

  registrarMatriculaCarrera(request: RegistrarMatriculaCarreraRequest): Observable<any> {
    return this.http.post<any>(`${this.apiUrl}/carrera`, request);
  }

  obtenerMatriculasPrograma(): Observable<any[]> {
    return this.http.get<any[]>(`${this.apiUrl}/programa`);
  }

  registrarMatriculaPrograma(request: any): Observable<any> {
    return this.http.post<any>(`${this.apiUrl}/programa`, request);
  }

  // --- MÉTODOS DE MAESTROS (Combos de la Cabecera) ---

  getProductos(): Observable<any[]> {
    return this.http.get<any[]>(`${this.maestrosUrl}/productos`);
  }

  getPromociones(): Observable<any[]> {
    return this.http.get<any[]>(`${this.maestrosUrl}/promociones`).pipe(
      map(data => (data ?? []).map(p => ({
        // Mapeo defensivo garantizado para las llaves de SQL
        id: p.idpromocion ?? p.Idpromocion ?? p.IdPromocion ?? p.id ?? 0,
        nombre: p.nombre ?? p.Nombre ?? p.descripcion ?? p.Descripcion ?? 'Sin nombre',
        codigo: p.codigo ?? p.Codigo ?? ''
      })))
    );
  }

  getGruposByPromocion(idPromocion: number): Observable<any[]> {
    return this.http.get<any[]>(`${this.maestrosUrl}/promociones/${idPromocion}/grupos`);
  }

  getCursos(): Observable<any[]> {
    return this.http.get<any[]>(`${this.maestrosUrl}/cursos`);
  }

  getCursosByPromocion(idPromocion: number): Observable<any[]> {
    return this.http.get<any[]>(`${this.maestrosUrl}/promociones/${idPromocion}/cursos`);
  }

  getPlanesPago(idPromocion: number, idGrupo: number, idSeccion: number = -1): Observable<PlanPagoItem[]> {
    const params = new HttpParams()
      .set('idPromocion', idPromocion.toString())
      .set('idGrupo', idGrupo.toString())
      .set('idSeccion', idSeccion.toString());

    return this.http.get<PlanPagoItem[]>(`${this.maestrosUrl}/planes-pago`, { params });
  }

  getBecas(idActor: number, idPromocion: number): Observable<BecaItem[]> {
    const params = new HttpParams()
      .set('idActor', idActor.toString())
      .set('idPromocion', idPromocion.toString());

    return this.http.get<BecaItem[]>(`${this.maestrosUrl}/becas`, { params });
  }

  // --- MÉTODOS DE ALUMNOS ---

  searchAlumnos(termino: string): Observable<any[]> {
    const params = new HttpParams().set('termino', termino);
    return this.http.get<any[]>(`${this.alumnosUrl}/buscar`, { params });
  }

  getAlumnoById(idAlumno: number): Observable<any> {
    return this.http.get<any>(`${this.alumnosUrl}/${idAlumno}`);
  }
}
