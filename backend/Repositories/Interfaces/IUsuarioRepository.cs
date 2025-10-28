using AuditoriaRecepcion.Models;

namespace AuditoriaRecepcion.Repositories.Interfaces
{
    public interface IUsuarioRepository : IRepository<Usuario>
    {
        Task<Usuario> GetByUsernameAsync(string username);
        Task<Usuario> GetByEmailAsync(string email);
        Task<IEnumerable<Usuario>> GetByRolAsync(string rol);
        Task<bool> ExisteUsernameAsync(string username, int? excludeId = null);
        Task<bool> ExisteEmailAsync(string email, int? excludeId = null);
        Task<IEnumerable<Usuario>> GetActivosAsync();
    }
}