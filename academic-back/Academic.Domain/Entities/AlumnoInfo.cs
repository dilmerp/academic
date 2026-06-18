namespace Academic.Domain.Entities;

public record AlumnoInfo
{
    public int IdActor { get; init; }
    public string Codigo { get; init; }
    public string Usuario { get; init; }
    public string Paterno { get; init; }
    public string Materno { get; init; }
    public string Nombres { get; init; }
    public string NombreCompleto { get; init; }
    public string TenorCodigo { get; init; }
    public bool Genero { get; init; }
    public string CivilNombre { get; init; }
    public string EMail { get; init; }
    public string EmailOpcional { get; init; }
    public string FechaNacimiento { get; init; }
    public string PaisNacimiento { get; init; }
    public string DistritoNombreNacimiento { get; init; }
    public string Telefono { get; init; }
    public string Celular { get; init; }
    public string Direccion { get; init; }
    public string Urbanizacion { get; init; }
    public string PaisResidencia { get; init; }
    public string DistritoNombreResidencia { get; init; }
    public string DocumentoCodigo { get; init; }
    public string NumeroDocumento { get; init; }
    public string Ruc { get; init; }
    public bool Trabaja { get; init; }
    public string NacionalidadNombre { get; init; }
    public string EstadoNombre { get; init; }
    public string UsuarioCreacion { get; init; }
    public string UsuarioModificacion { get; init; }
}