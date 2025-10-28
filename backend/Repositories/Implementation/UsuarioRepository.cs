using AuditoriaRecepcion.Data;
using AuditoriaRecepcion.Models;
using AuditoriaRecepcion.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace AuditoriaRecepcion.Repositories.Implementation
{
    public class UsuarioRepository : BaseRepository<Usuario>, IUsuarioRepository
    {
        public UsuarioRepository(AuditoriaRecepcionContext context) : base(context)
        {
        }

        public async Task<Usuario> GetByUsernameAsync(string username)
        {
            return await _dbSet.FirstOrDefaultAsync(u => u.NombreUsuario == username);
        }

        public async Task<Usuario> GetByEmailAsync(string email)
        {
            return await _dbSet.FirstOrDefaultAsync(u => u.Email == email);
        }

        public async Task<IEnumerable<Usuario>> GetByRolAsync(string rol)
        {
            return await _dbSet
                .Where(u => u.Rol == rol && u.Activo)
                .OrderBy(u => u.NombreCompleto)
                .ToListAsync();
        }

        public async Task<bool> ExisteUsernameAsync(string username, int? excludeId = null)
        {
            var query = _dbSet.Where(u => u.NombreUsuario == username);

            if (excludeId.HasValue)
                query = query.Where(u => u.UsuarioID != excludeId.Value);

            return await query.AnyAsync();
        }

        public async Task<bool> ExisteEmailAsync(string email, int? excludeId = null)
        {
            var query = _dbSet.Where(u => u.Email == email);

            if (excludeId.HasValue)
                query = query.Where(u => u.UsuarioID != excludeId.Value);

            return await query.AnyAsync();
        }

        public async Task<IEnumerable<Usuario>> GetActivosAsync()
        {
            return await _dbSet
                .Where(u => u.Activo)
                .OrderBy(u => u.NombreCompleto)
                .ToListAsync();
        }
    }
}