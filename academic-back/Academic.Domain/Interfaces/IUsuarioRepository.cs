using System.Threading;
using System.Threading.Tasks;
using Academic.Domain.Entities;

namespace Academic.Domain.Interfaces;

public interface IUsuarioRepository
{
    Task<Usuario> GetByLoginAsync(string login);

    // Método transaccional alineado al estándar
    Task<bool> UpdateAsync(Usuario usuario, CancellationToken cancellationToken);
}