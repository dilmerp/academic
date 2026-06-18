namespace Academic.Application.Interfaces;

public interface IJwtProvider
{
    /// <summary>
    /// Genera un token JWT para un usuario autenticado.
    /// </summary>
    /// <param name="idUsuario">ID único del usuario.</param>
    /// <param name="nombreCompleto">Nombre completo del usuario.</param>
    /// <param name="rol">Rol o permisos del usuario.</param>
    /// <returns>El token en formato String.</returns>
    string Generate(int idUsuario, string nombreCompleto, string rol);
}