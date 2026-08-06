using TiendaOnline.Dominio.Model;

namespace TiendaOnline.LogicaNegocio.Interfaces;

public interface IJwtServicio
{
    string GenerarToken(
        Usuario usuario,
        string nombreRol,
        DateTime fechaExpiracion);
}
