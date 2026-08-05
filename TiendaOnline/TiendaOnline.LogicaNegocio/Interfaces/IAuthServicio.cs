using TiendaOnline.Dominio.DTO;

namespace TiendaOnline.LogicaNegocio.Interfaces;

public interface IAuthServicio
{
    Task<RespuestaLoginDto?> LoginAsync(LoginDto login);

    Task<bool> RegistrarAsync(RegistroUsuarioDto registro);
}