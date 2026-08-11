using TiendaOnline.Dominio.Entidades;

namespace TiendaOnline.Dominio.InterfacesLN;

public interface IJwtServicio
{
    string GenerarToken(
        Usuario usuario,
        string nombreRol,
        DateTime fechaExpiracion);
}
