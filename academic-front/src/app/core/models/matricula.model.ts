export interface MatriculaResponse {
  idAlumno: number;
  idRegistro: number;
  idMatricula: number;
  idPromocion: number;
  idGrupo: number;
  esMatricula: boolean;
  nombreCompleto: string;
  grupoCodigo: string;
  seccion: string;
  matriculaFecha: string;
  matriculaPor: string;
}

export interface AlumnoCarrera {
  idAlumno: number;
  idBeca?: number | null; // Se agregó la beca (es opcional)
  idRegistro: number;
  idMatricula: number;
  idDocumento: number;     // Agregado para validación de homonimia
  numeroDocumento: string; // Agregado para validación de homonimia
}

export interface CursoCarrera {
  idCurso: number;
  idSeccion: number;
}

export interface RegistrarMatriculaCarreraRequest {
  idPromocion: number;
  idGrupo: number;
  idPlanPago?: number;   // Se agregó el plan de pagos
  idTipo?: number;       // Campos requeridos por tu lógica original
  idSubTipo?: number;
  idProducto?: number;
  idModulo?: number;
  alumnos: AlumnoCarrera[];
  cursos: CursoCarrera[];
  usuarioModificacion: number;
}

export interface PlanPagoItem {
  idPlan: number;
  planNombre: string;
  idMoneda: number;
  monedaNombre: string;
}

// Nueva interfaz para mapear el catálogo de becas
export interface BecaItem {
  idBeca: number;
  beneficioNombre: string;
  descuento: number;
  descripcion: string;
}
