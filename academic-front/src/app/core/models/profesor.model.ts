export interface Profesor {
    // --- Campos devueltos por el DTO del Backend (Para el Listado) ---
    id: number;
    codigo: string;
    usuario: string;
    nombreCompleto: string;
    genero: string | boolean; // Permite string del DTO ("M"/"F") o boolean del Form
    estadoCivil: string;
    emailPrincipal: string;
    emailAdicional: string;
    fechaNacimiento: string;
    nacionalidad: string;
    ubicacionResidencia: string;
    direccionCompleta: string;
    tipoDocumento: string;
    numeroDocumento: string;
    telefonoContacto: string;
    estado: string;
    auditoriaCreacion: string;

    // --- Campos extras para el Alta/Edición (basados en VB6 y SPs) ---
    paterno?: string;            // txtApePaterno
    materno?: string;            // txtApeMaterno
    nombres?: string;            // txtNombres
    idTenor?: number;            // lblIdTenor
    idEstadoCivil?: number;      // lblIdEstadoCivil
    emailOpcional?: string;      // txtEmailOpcional
    idPaisNacimiento?: number;   // lblIdPaisNacimiento
    idUbigeoNacimiento?: string; // lblIdDistritoNacimiento
    idPaisResidencia?: number;   // lblIdPaisResidencia
    idUbigeoResidencia?: string; // lblIdDistritoResidencia
    idNacionalidad?: number;     // lblIdNacionalidad
    idTipoDocumento?: number;    // lblIdTipoDocumento
    ruc?: string;                // txtNumeroRuc
    idCategoria?: number;        // lblIdCategoria
    idModalidadPago?: number;    // lblIdModalidadPago
    tarifa?: number;             // txtTarifa

    // --- Colecciones para Sub-componentes ---
    cursos?: ProfesorCurso[];
    disponibilidad?: ProfesorDisponibilidad[];
}

/**
 * Interfaz para los cursos del profesor (cProfesorCurso)
 */
export interface ProfesorCurso {
    idCurso: number;
    nombreCurso?: string;
    tarifa: number;
}

/**
 * Interfaz para disponibilidad (cProfesorDisponibilidad)
 */
export interface ProfesorDisponibilidad {
    idDisponibilidad?: number;
    fechaInicio: string;
    fechaFin: string;
}
