using TiendaOnline.Dominio.DTO;

namespace TiendaOnline.Dominio.InterfacesLN;

public interface IAuthServicio
{
    Task<RespuestaLoginDto?> LoginAsync(LoginDto login);

    Task<bool> RegistrarAsync(RegistroUsuarioDto registro);
}