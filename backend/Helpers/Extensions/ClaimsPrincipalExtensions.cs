using System.Security.Claims;

namespace AuditoriaRecepcion.Helpers.Extensions
{
    public static class ClaimsPrincipalExtensions
    {
        public static int GetUserId(this ClaimsPrincipal principal)
        {
            var userIdClaim = principal.FindFirst("UserId")?.Value;
            return int.TryParse(userIdClaim, out var userId) ? userId : 0;
        }

        public static string GetUsername(this ClaimsPrincipal principal)
        {
            return principal.FindFirst(ClaimTypes.Name)?.Value;
        }

        public static string GetRole(this ClaimsPrincipal principal)
        {
            return principal.FindFirst(ClaimTypes.Role)?.Value;
        }

        public static bool IsInRole(this ClaimsPrincipal principal, params string[] roles)
        {
            var userRole = principal.GetRole();
            return roles.Contains(userRole);
        }

        public static bool IsAdministrador(this ClaimsPrincipal principal)
        {
            return principal.IsInRole("Administrador");
        }

        public static bool IsJefeLogistica(this ClaimsPrincipal principal)
        {
            return principal.IsInRole("JefeLogistica");
        }
    }
}