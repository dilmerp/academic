export interface Alumno {
    id: number;
    codigo: string;
    usuario: string;
    nombreCompleto: string;
    genero: string | boolean; // Permite string del DTO ("M"/"F") o boolean del Form
    estadoCivil: string;
    emailPrincipal: string;
    fechaNacimiento: string;
    nacionalidad: string;
    ubicacionResidencia: string;
    direccionCompleta: string;
    tipoDocumento: string;
    numeroDocumento: string;
    telefonoContacto: string;
    estado: string;
    auditoriaCreacion: string;
}
