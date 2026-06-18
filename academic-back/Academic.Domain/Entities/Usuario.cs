using System;

namespace Academic.Domain.Entities;

public class Usuario
{
    public int idusuario { get; set; }
    public int? idactor { get; set; }
    public string? apellidopaterno { get; set; }
    public string? apellidomaterno { get; set; }
    public string? nombres { get; set; }
    public string nombre { get; set; } = string.Empty;
    public string login { get; set; } = string.Empty;
    public string clave { get; set; } = string.Empty;
    public string email { get; set; } = string.Empty;
    public bool activo { get; set; }
    public int usuariocreacion { get; set; }
    public DateTime fechacreacion { get; set; }
    public int usuariomodificacion { get; set; }
    public DateTime fechamodificacion { get; set; }

    public bool requierecambioclave { get; set; } = true;
}