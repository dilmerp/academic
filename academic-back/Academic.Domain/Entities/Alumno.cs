namespace Academic.Domain.Entities;

public class Alumno
{
    public int Id { get; set; }
    public string Codigo { get; set; } = string.Empty;
    public string Documento { get; set; } = string.Empty;
    public string Nombres { get; set; } = string.Empty;
    public string ApellidoPaterno { get; set; } = string.Empty;
    public string ApellidoMaterno { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;

    // Propiedad calculada de dominio
    public string NombreCompleto => $"{Nombres} {ApellidoPaterno} {ApellidoMaterno}".Trim();
}